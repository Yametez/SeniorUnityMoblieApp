using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace CoinGame
{
    public class CoinManager : MonoBehaviour
    {
        [System.Serializable]
        public class CoinType
        {
            public int value;
            public GameObject prefab;
            public int count;
            public Transform sortingArea;
        }

        [System.Serializable]
        public class LevelConfig
        {
            public int maxCoins;              // จำนวนเหรียญสูงสุดในแต่ละเลเวล
            public float countdownTime;        // เวลานับถอยหลัง (0 = ไม่มีการนับถอยหลัง)
            public bool isShuffledSortingArea; // true = สลับตำแหน่งพื้นที่แยกเหรียญ
        }

        [Header("Level Settings")]
        public LevelConfig[] levels = new LevelConfig[3] {
            new LevelConfig { maxCoins = 8, countdownTime = 0, isShuffledSortingArea = false },    // Level 1
            new LevelConfig { maxCoins = 13, countdownTime = 15, isShuffledSortingArea = false },   // Level 2
            new LevelConfig { maxCoins = 17, countdownTime = 25, isShuffledSortingArea = true }     // Level 3
        };

        public int currentLevel { get; private set; } = 0;
        private float countdownTimer;
        private bool isCountingDown;

        [Header("UI Elements")]
        public Text countdownText;        // Text สำหรับแสดงเวลานับถอยหลัง
        public Transform[] sortingAreas;  // พื้นที่แยกเหรียญทั้ง 3 จุด

        public CoinType[] coinTypes; // เก็บข้อมูลเหรียญแต่ละประเภท (1, 5, 10 บาท)
        public Transform coinPileArea; // พื้นที่กองเหรียญ
        public Text timerText;
        public Text scoreText;
        public Text coin10ScoreText; // เพิ่มตัวแปรสำหรับแสดงคะแนนแต่ละประเภท
        public Text coin5ScoreText;
        public Text coin1ScoreText;
        
        private float gameTimer;  // เวลารวมทั้งหมด
        private bool isGameActive;
        private List<GameObject> activeCoinPile = new List<GameObject>();
        private int coin10Count = 0;
        private int coin5Count = 0;
        private int coin1Count = 0;
        private int totalScore = 0;
        private int totalCoinsInGame = 0;

        public ResultPanel resultPanel; // เพิ่มตัวแปรนี้ที่ด้านบนของคลาส

        private int totalAccumulatedScore = 0;  // คะแนนสะสม
        private float totalGameTime = 0;  // เวลารวมทุกเลเวล

        public GameObject countdownObject; // เพิ่มตัวแปรอ้างอิงถึง Countdown object

        void Start()
        {
            // เพิ่มการ reset time scale
            Time.timeScale = 1f;
            
            Debug.Log("Start called");
            if (resultPanel != null)
            {
                resultPanel.gameObject.SetActive(false);
            }

            // ลบเหรียญ prefab ที่วางไว้ใน Scene ออกก่อน
            GameObject[] existingCoins = GameObject.FindGameObjectsWithTag("Coin");
            foreach (var coin in existingCoins)
            {
                Destroy(coin);
            }

            // เริ่มเกมที่เลเวล 1
            gameTimer = 0;  // รีเซ็ตเวลาเฉพาะตอนเริ่มเกมครั้งแรกเท่านั้น
            StartLevel(0);
        }

        void Update()
        {
            if (isGameActive)
            {
                gameTimer += Time.deltaTime;
                UpdateTimerDisplay();

                if (isCountingDown)
                {
                    countdownTimer -= Time.deltaTime;
                    if (countdownTimer <= 0)
                    {
                        // เมื่อหมดเวลา ให้ไปเลเวลถัดไปหรือจบเกม
                        if (currentLevel < 2)
                        {
                            StartLevel(currentLevel + 1);
                        }
                        else
                        {
                            ShowFinalResults();
                        }
                    }
                    UpdateCountdownDisplay();
                }

                // เช็คว่าจบเลเวลหรือยัง
                CheckLevelCompletion();
            }
        }

        public void StartLevel(int level)
        {
            currentLevel = level;
            LevelConfig config = levels[currentLevel];

            // รีเซ็ตเฉพาะส่วนที่จำเป็นสำหรับเลเวลใหม่
            ResetForNewLevel();

            // จัดการการแสดง/ซ่อน countdown
            if (countdownObject != null)
            {
                // แสดง countdown เฉพาะเลเวล 2 และ 3
                countdownObject.SetActive(level > 0);
            }

            // ตั้งค่าการนับถอยหลัง
            if (config.countdownTime > 0)
            {
                countdownTimer = config.countdownTime;
                isCountingDown = true;
            }
            else
            {
                isCountingDown = false;
            }

            // สลับตำแหน่งพื้นที่แยกเหรียญถ้าจำเป็น
            if (config.isShuffledSortingArea)
            {
                ShuffleSortingAreas();
            }

            // แก้ไขจาก SpawnCoinsForCurrentLevel() เป็น ResetGame()
            ResetGame(); // สร้างเหรียญใหม่ตามการตั้งค่าของเลเวล
        }

        private void ResetForNewLevel()
        {
            // ไม่ต้องรีเซ็ต gameTimer
            isGameActive = true;
            coin10Count = 0;
            coin5Count = 0;
            coin1Count = 0;
            totalCoinsInGame = 0;

            // เคลียร์เหรียญเก่า
            foreach (var coin in activeCoinPile)
            {
                if (coin != null)
                {
                    Destroy(coin);
                }
            }
            activeCoinPile.Clear();

            UpdateUI();
        }

        private void ShuffleSortingAreas()
        {
            // สลับตำแหน่งพื้นที่แยกเหรียญ
            Vector3[] positions = sortingAreas.Select(area => area.position).ToArray();
            for (int i = positions.Length - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                Vector3 temp = positions[i];
                positions[i] = positions[randomIndex];
                positions[randomIndex] = temp;
            }

            // อัพเดทตำแหน่งใหม่
            for (int i = 0; i < sortingAreas.Length; i++)
            {
                sortingAreas[i].position = positions[i];
            }
        }

        private void UpdateCountdownDisplay()
        {
            if (countdownText != null)
            {
                countdownText.text = $"{Mathf.CeilToInt(countdownTimer)}";
            }
        }

        public void ResetGame()
        {
            // รีเซ็ตค่าทั้งหมด ยกเว้น gameTimer
            isGameActive = true;
            coin10Count = 0;
            coin5Count = 0;
            coin1Count = 0;
            totalScore = 0;
            totalCoinsInGame = 0;
            
            // เคลียร์เหรียญเก่า
            foreach (var coin in activeCoinPile)
            {
                if (coin != null)
                {
                    Destroy(coin);
                }
            }
            activeCoinPile.Clear();

            UpdateUI();
            
            // สร้างเหรียญใหม่
            SpawnCoinsForLevel();
        }

        public void StartNewGame()
        {
            // รีเซ็ตค่าทั้งหมด
            gameTimer = 0;
            totalAccumulatedScore = 0;
            currentLevel = 0;
            isGameActive = true;
            
            // รีเซ็ตค่าการนับเหรียญ
            coin10Count = 0;
            coin5Count = 0;
            coin1Count = 0;
            
            // รีเซ็ต UI แสดงจำนวนเหรียญ
            if (coin10ScoreText) coin10ScoreText.text = "0";
            if (coin5ScoreText) coin5ScoreText.text = "0";
            if (coin1ScoreText) coin1ScoreText.text = "0";
            
            // รีเซ็ตคะแนนรวม UI
            if (scoreText) scoreText.text = "ยอดรวม: 0 บาท";
            
            // รีเซ็ตเวลา UI
            if (timerText) timerText.text = "เวลา: 00:00";

            // ซ่อน result panel
            if (resultPanel != null)
            {
                resultPanel.gameObject.SetActive(false);
            }

            // รีเซ็ตสถานะเกม
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetGameState();
            }

            // เริ่มเลเวล 1
            StartLevel(0);
        }

        private void SpawnCoinsForLevel()
        {
            // สร้างเหรียญตามจำนวนที่กำหนดในแต่ละเลเวล
            int totalCoinsForThisLevel = 0;
            foreach (var coinType in coinTypes)
            {
                coinType.count = Random.Range(2, 4);
                totalCoinsForThisLevel += coinType.count;

                if (totalCoinsForThisLevel > levels[currentLevel].maxCoins)
                {
                    coinType.count -= (totalCoinsForThisLevel - levels[currentLevel].maxCoins);
                    totalCoinsForThisLevel = levels[currentLevel].maxCoins;
                }

                totalCoinsInGame += coinType.count;
                SpawnCoins(coinType);
            }
        }

        void SpawnCoins(CoinType coinType)
        {
            List<Vector3> usedPositions = new List<Vector3>();
            float minDistance = 50f; // เพิ่มระยะห่างให้มากขึ้นเพื่อให้เห็นการกระจายชัดเจน
            
            // ลดขนาดพื้นที่ให้พอดีกับกรอบสีม่วง
            float areaWidth = 150f;   // ลดลงจาก 200f
            float areaHeight = 150f;  // ลดลงจาก 200f
            
            for (int i = 0; i < coinType.count; i++)
            {
                Vector3 randomPos;
                bool validPosition = false;
                int maxAttempts = 30; // ป้องกันการวนลูปไม่รู้จบ
                int attempts = 0;

                // วนลูปจนกว่าจะได้ตำแหน่งที่ไม่ซ้ำกับเหรียญอื่น
                do {
                    // จำกัดพื้นที่การสุ่มให้แคบลง
                    float randomX = Random.Range(-areaWidth/3, areaWidth/3);
                    float randomY = Random.Range(-areaHeight/3, areaHeight/3);
                    
                    // ใช้ตำแหน่งสัมพัทธ์กับ coinPileArea
                    randomPos = new Vector3(
                        coinPileArea.position.x + randomX,
                        coinPileArea.position.y + randomY,
                        -0.1f
                    );
                    
                    // ตรวจสอบว่าตำแหน่งนี้ห่างจากเหรียญอื่นเพียงพอ
                    validPosition = true;
                    foreach (Vector3 usedPos in usedPositions)
                    {
                        if (Vector2.Distance(new Vector2(randomPos.x, randomPos.y), 
                                          new Vector2(usedPos.x, usedPos.y)) < minDistance)
                        {
                            validPosition = false;
                            break;
                        }
                    }
                    
                    attempts++;
                    if (attempts >= maxAttempts) break;
                    
                } while (!validPosition);

                // เก็บตำแหน่งที่ใช้แล้ว
                usedPositions.Add(randomPos);
                
                GameObject newCoin = Instantiate(coinType.prefab, randomPos, Quaternion.identity);
                newCoin.transform.localScale = new Vector3(25f, 25f, 1f);
                
                // ตั้งให้ coin เป็นลูกของ coinPileArea เพื่อให้ตำแหน่งสัมพัทธ์ถูกต้อง
                newCoin.transform.SetParent(coinPileArea);
                
                activeCoinPile.Add(newCoin);
                
                CoinDrag dragComponent = newCoin.GetComponent<CoinDrag>();
                if (dragComponent == null)
                {
                    dragComponent = newCoin.AddComponent<CoinDrag>();
                    dragComponent.coinValue = coinType.value;
                }
                dragComponent.manager = this;
                
                Debug.Log($"Spawned coin at position: {randomPos}"); // เพิ่ม Debug log

                // กำหนด sorting order ให้เหรียญอยู่ด้านบนพื้นหลัง
                SpriteRenderer spriteRenderer = newCoin.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingOrder = 100;  // ค่าบวกจะอยู่ด้านบน -2
                }
            }
        }

        void UpdateTimerDisplay()
        {
            if (timerText != null)
            {
                int minutes = (int)(gameTimer / 60);
                int seconds = (int)(gameTimer % 60);
                timerText.text = $"เวลา: {minutes:00}:{seconds:00}";
            }
        }

        public void CheckGameCompletion()
        {
            if (activeCoinPile.Count == 0)
            {
                isGameActive = false;
                // Show completion UI
                ShowResults();
            }
        }

        void UpdateUI()
        {
            UpdateTimerDisplay();
            // Update other UI elements
        }

        void ShowResults()
        {
            Debug.Log("ShowResults called");
            
            // คำนวณคะแนนรวมใหม่จากจำนวนเหรียญแต่ละประเภท
            totalScore = (coin10Count * 10) + (coin5Count * 5) + (coin1Count * 1);
            UpdateScoreDisplay(); 
            
            if (resultPanel != null)
            {
                // อัพเดท UI ก่อนแสดงผล
                UpdateScoreDisplay();
                
                // แสดงผลด้วยค่าที่คำนวณใหม่
                resultPanel.gameObject.SetActive(true);
                resultPanel.ShowResults(gameTimer, coin10Count, coin5Count, coin1Count, totalScore);
                Debug.Log($"Results shown - Time: {gameTimer:F2}, Score: {totalScore}");
            }
            else
            {
                Debug.LogError("ResultPanel is null!");
            }
        }

        // เพิ่มฟังก์ชันนี้เพื่อทดสอบ
        public void TestShowResults()
        {
            Debug.Log("Testing ShowResults");
            ShowResults();
        }

        public void RemoveCoin(GameObject coin)
        {
            if (activeCoinPile.Contains(coin))
            {
                activeCoinPile.Remove(coin);
                Debug.Log($"Coin removed. Remaining coins: {activeCoinPile.Count}");
                
                // เช็คเงื่อนไขจบเกมทันทีที่ลบเหรียญ
                if (activeCoinPile.Count == 0)
                {
                    Debug.Log("No coins left in pile! Showing results...");
                    isGameActive = false;
                    ShowResults();
                }
            }
        }

        public void AddCoinToScore(GameObject coin, int coinValue)
        {
            Debug.Log($"Adding coin value: {coinValue}");
            
            // เช็คว่าเกมยังทำงานอยู่
            if (!isGameActive) return;
            
            // เช็คว่าเหรียญนี้มาจาก activeCoinPile เท่านั้น
            if (!activeCoinPile.Contains(coin))
            {
                Debug.Log("Coin not from active pile, ignoring...");
                return;
            }
            
            switch (coinValue)
            {
                case 10:
                    coin10Count++;
                    totalAccumulatedScore += 10;  // เพิ่มคะแนนสะสม
                    if (coin10ScoreText) coin10ScoreText.text = coin10Count.ToString();
                    break;
                case 5:
                    coin5Count++;
                    totalAccumulatedScore += 5;   // เพิ่มคะแนนสะสม
                    if (coin5ScoreText) coin5ScoreText.text = coin5Count.ToString();
                    break;
                case 1:
                    coin1Count++;
                    totalAccumulatedScore += 1;   // เพิ่มคะแนนสะสม
                    if (coin1ScoreText) coin1ScoreText.text = coin1Count.ToString();
                    break;
            }
            
            Debug.Log($"After add - Coin10: {coin10Count}, Coin5: {coin5Count}, Coin1: {coin1Count}, Total: {totalCoinsInGame}");
            UpdateScoreDisplay();
        }

        public void SubtractScore(int coinValue)
        {
            // เพิ่มการเช็คว่าเกมยังทำงานอยู่
            if (!isGameActive) return;

            switch (coinValue)
            {
                case 10:
                    if (coin10Count > 0)
                    {
                        coin10Count--;
                        totalAccumulatedScore -= 10;
                    }
                    if (coin10ScoreText) coin10ScoreText.text = coin10Count.ToString();
                    break;
                case 5:
                    if (coin5Count > 0)
                    {
                        coin5Count--;
                        totalAccumulatedScore -= 5;
                    }
                    if (coin5ScoreText) coin5ScoreText.text = coin5Count.ToString();
                    break;
                case 1:
                    if (coin1Count > 0)
                    {
                        coin1Count--;
                        totalAccumulatedScore -= 1;
                    }
                    if (coin1ScoreText) coin1ScoreText.text = coin1Count.ToString();
                    break;
            }
            UpdateScoreDisplay();
        }

        void UpdateScoreDisplay()
        {
            if (scoreText) 
            {
                // แสดงคะแนนสะสมแทนคะแนนในเลเวลปัจจุบัน
                scoreText.text = $"ยอดรวม: {totalAccumulatedScore} บาท";
            }
        }

        // เพิ่มฟังก์ชันใหม่เพื่อหน่วงเวลาก่อนแสดงผล
        private IEnumerator DelayedShowResults()
        {
            // รอให้การอัพเดทคะแนนเสร็จสมบูรณ์
            yield return new WaitForSeconds(0.1f);
            
            // ปิดเกมก่อนคำนวณคะแนนสุดท้าย
            isGameActive = false;
            
            // คำนวณคะแนนรวมครั้งสุดท้าย
            totalScore = (coin10Count * 10) + (coin5Count * 5) + (coin1Count * 1);
            
            ShowResults();
        }

        // เพิ่มเมธอดสำหรับเช็คสถานะเกม
        public bool IsGameActive()
        {
            return isGameActive;
        }

        private void GameOver()
        {
            Debug.Log("Game Over!");
            isGameActive = false;
            ShowResults();
        }

        private void CheckLevelCompletion()
        {
            int totalSorted = coin10Count + coin5Count + coin1Count;
            
            if (totalSorted == totalCoinsInGame && totalCoinsInGame > 0)
            {
                // เก็บคะแนนและเวลาของเลเวลปัจจุบัน
                totalAccumulatedScore += totalScore;
                totalGameTime += gameTimer;

                if (currentLevel < 2)  // ถ้ายังไม่ถึงเลเวล 3
                {
                    // เล่นเสียงจบเลเวล
                    AudioManager.Instance?.PlayLevelComplete();
                    StartLevel(currentLevel + 1);
                }
                else
                {
                    // เล่นเสียงจบเกม
                    AudioManager.Instance?.PlayGameOver();
                    ShowFinalResults();
                }
            }
        }

        private void ShowFinalResults()
        {
            // หยุดการนับเวลา
            isGameActive = false;

            // ซ่อน countdown
            if (countdownObject != null)
            {
                countdownObject.SetActive(false);
            }

            if (resultPanel != null)
            {
                resultPanel.gameObject.SetActive(true);
                resultPanel.ShowResults(gameTimer, coin10Count, coin5Count, coin1Count, totalAccumulatedScore);
            }
        }
    }
} 