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

        void Start()
        {
            Debug.Log("Start called");
            StartNewGame();
        }

        void Update()
        {
            if (isGameActive)
            {
                gameTimer += Time.deltaTime;
                UpdateTimerDisplay();
            }
        }

        public void StartNewGame()
        {
            Debug.Log("StartNewGame called");
            // เคลียร์เหรียญเก่า
            foreach (var coin in activeCoinPile)
            {
                Destroy(coin);
            }
            activeCoinPile.Clear();

            // รีเซ็ตค่าต่างๆ
            gameTimer = 0;
            isGameActive = true;
            
            // สุ่มจำนวนเหรียญแต่ละประเภท
            foreach (var coinType in coinTypes)
            {
                Debug.Log($"Spawning {coinType.count} coins of value {coinType.value}");
                coinType.count = Random.Range(3, 10);
                SpawnCoins(coinType);
            }

            UpdateUI();
        }

        void SpawnCoins(CoinType coinType)
        {
            Debug.Log($"SpawnCoins called for value {coinType.value}");
            for (int i = 0; i < coinType.count; i++)
            {
                Vector3 randomPos = coinPileArea.position + Random.insideUnitSphere * 2f;
                randomPos.z = 0; // 2D game
                
                GameObject newCoin = Instantiate(coinType.prefab, randomPos, Quaternion.identity);
                newCoin.transform.localScale = new Vector3(50f, 50f, 1f);
                
                activeCoinPile.Add(newCoin);
                
                // Add drag functionality
                CoinDrag dragComponent = newCoin.AddComponent<CoinDrag>();
                dragComponent.coinValue = coinType.value;
                dragComponent.manager = this;
            }
        }

        void UpdateTimerDisplay()
        {
            int minutes = (int)(gameTimer / 60);
            int seconds = (int)(gameTimer % 60);
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
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
            // Show final score and time
            scoreText.text = $"Completed!\nTime: {timerText.text}";
        }

        public void RemoveCoin(GameObject coin)
        {
            if (activeCoinPile.Contains(coin))
            {
                activeCoinPile.Remove(coin);
                CheckGameCompletion();
            }
        }

        public void AddCoinToScore(int coinValue)
        {
            Debug.Log($"Adding coin value: {coinValue}");
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
            if (scoreText) scoreText.text = $"Score: {totalScore}";
        }
    }
} 