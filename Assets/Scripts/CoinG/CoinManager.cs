using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

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

        public CoinType[] coinTypes; // เก็บข้อมูลเหรียญแต่ละประเภท (1, 5, 10 บาท)
        public Transform coinPileArea; // พื้นที่กองเหรียญ
        public Text timerText;
        public Text scoreText;
        public Text coin10ScoreText; // เพิ่มตัวแปรสำหรับแสดงคะแนนแต่ละประเภท
        public Text coin5ScoreText;
        public Text coin1ScoreText;
        
        private float gameTimer;
        private bool isGameActive;
        private List<GameObject> activeCoinPile = new List<GameObject>();
        private int coin10Count = 0;
        private int coin5Count = 0;
        private int coin1Count = 0;
        private int totalScore = 0;
        private int totalCoinsInGame = 0;

        public ResultPanel resultPanel; // เพิ่มตัวแปรนี้ที่ด้านบนของคลาส

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

            ResetGame(); // เปลี่ยนจาก StartNewGame เป็น ResetGame
        }

        void Update()
        {
            if (isGameActive)
            {
                gameTimer += Time.deltaTime;
                UpdateTimerDisplay();
                
                // Debug logs
                Debug.Log($"Active coins: {activeCoinPile.Count}");
                int totalSorted = coin10Count + coin5Count + coin1Count;
                Debug.Log($"Sorted coins - 10B: {coin10Count}, 5B: {coin5Count}, 1B: {coin1Count}");
                Debug.Log($"Total sorted: {totalSorted}, Total in game: {totalCoinsInGame}");
                
                // เพิ่มการหน่วงเวลาเล็กน้อยก่อนเช็คเงื่อนไขจบเกม
                if (totalSorted == totalCoinsInGame && totalCoinsInGame > 0)
                {
                    StartCoroutine(DelayedShowResults());
                }
            }
        }

        // เพิ่มฟังก์ชันใหม่
        public void ResetGame()
        {
            // รีเซ็ตค่าทั้งหมด
            gameTimer = 0;
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

            // อัพเดท UI
            UpdateUI();
            
            // สร้างเหรียญใหม่
            foreach (var coinType in coinTypes)
            {
                coinType.count = Random.Range(2, 5);
                totalCoinsInGame += coinType.count;
                SpawnCoins(coinType);
            }
        }

        public void StartNewGame()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetGameState();
            }
            ResetGame();
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
            int minutes = (int)(gameTimer / 60);
            int seconds = (int)(gameTimer % 60);
            timerText.text = $"เวลา: {minutes:00}:{seconds:00}";
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
                    totalScore += 10;
                    if (coin10ScoreText) coin10ScoreText.text = coin10Count.ToString();
                    break;
                case 5:
                    coin5Count++;
                    totalScore += 5;
                    if (coin5ScoreText) coin5ScoreText.text = coin5Count.ToString();
                    break;
                case 1:
                    coin1Count++;
                    totalScore += 1;
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
                        totalScore -= 10;
                    }
                    if (coin10ScoreText) coin10ScoreText.text = coin10Count.ToString();
                    break;
                case 5:
                    if (coin5Count > 0)
                    {
                        coin5Count--;
                        totalScore -= 5;
                    }
                    if (coin5ScoreText) coin5ScoreText.text = coin5Count.ToString();
                    break;
                case 1:
                    if (coin1Count > 0)
                    {
                        coin1Count--;
                        totalScore -= 1;
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
                totalScore = (coin10Count * 10) + (coin5Count * 5) + (coin1Count * 1);
                scoreText.text = $"ยอดรวม: {totalScore} บาท";
                Debug.Log($"Score updated: {totalScore}");
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
    }
} 