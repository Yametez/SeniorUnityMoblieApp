using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
            Debug.Log("Start called");
            // ซ่อน ResultPanel ก่อนเริ่มเกม
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

            StartNewGame();
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
                
                // เช็คเงื่อนไขจบเกม
                if (totalSorted == totalCoinsInGame && totalCoinsInGame > 0)
                {
                    Debug.Log("Game Complete! Showing results...");
                    isGameActive = false;
                    ShowResults();
                }
            }
        }

        public void StartNewGame()
        {
            Debug.Log("StartNewGame called");
            
            // เคลียร์เหรียญเก่า
            foreach (var coin in activeCoinPile)
            {
                if (coin != null)
                {
                    Destroy(coin);
                }
            }
            activeCoinPile.Clear();

            // รีเซ็ตค่าต่างๆ
            gameTimer = 0;
            isGameActive = true;
            coin10Count = 0;
            coin5Count = 0;
            coin1Count = 0;
            totalScore = 0;
            totalCoinsInGame = 0;
            
            // ซ่อน ResultPanel และรีเซ็ตการอ้างอิง
            if (resultPanel == null)
            {
                Debug.LogError("ResultPanel is null! Trying to find it...");
                resultPanel = FindObjectOfType<ResultPanel>();
            }
            
            if (resultPanel != null)
            {
                resultPanel.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Could not find ResultPanel!");
                return;
            }

            // สร้างเหรียญใหม่
            foreach (var coinType in coinTypes)
            {
                coinType.count = Random.Range(3, 10);
                totalCoinsInGame += coinType.count;
                SpawnCoins(coinType);
            }

            Debug.Log($"New game started with {totalCoinsInGame} coins");
            UpdateUI();
        }

        void SpawnCoins(CoinType coinType)
        {
            for (int i = 0; i < coinType.count; i++)
            {
                Vector3 randomPos = coinPileArea.position + Random.insideUnitSphere * 2f;
                randomPos.z = 0;
                
                GameObject newCoin = Instantiate(coinType.prefab, randomPos, Quaternion.identity);
                newCoin.transform.localScale = new Vector3(25f, 25f, 1f);
                
                activeCoinPile.Add(newCoin);
                
                CoinDrag dragComponent = newCoin.GetComponent<CoinDrag>();
                if (dragComponent == null)
                {
                    dragComponent = newCoin.AddComponent<CoinDrag>();
                    dragComponent.coinValue = coinType.value;
                }
                dragComponent.manager = this;
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
            if (resultPanel == null)
            {
                Debug.LogError("ResultPanel reference is missing!");
                return;
            }

            // ตรวจสอบว่า ResultPanel มี Canvas Parent หรือไม่
            Canvas parentCanvas = resultPanel.GetComponentInParent<Canvas>();
            if (parentCanvas == null)
            {
                Debug.LogError("ResultPanel must be child of a Canvas!");
                return;
            }

            // ทำให้แน่ใจว่า ResultPanel จะแสดงทับทุกอย่าง
            parentCanvas.sortingOrder = 999;
            resultPanel.transform.SetAsLastSibling();

            // เปิดใช้งานและแสดงผล
            resultPanel.gameObject.SetActive(true);
            resultPanel.ShowResults(gameTimer, coin10Count, coin5Count, coin1Count, totalScore);
            
            Debug.Log($"Results shown - Time: {gameTimer:F2}, Score: {totalScore}");
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
            if (scoreText) scoreText.text = $"ยอดรวม: {totalScore} บาท";
        }
    }
} 