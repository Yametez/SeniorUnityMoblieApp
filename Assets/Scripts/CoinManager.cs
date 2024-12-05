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
        
        private float gameTimer;
        private bool isGameActive;
        private List<GameObject> activeCoinPile = new List<GameObject>();

        void Start()
        {
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
                coinType.count = Random.Range(3, 10);
                SpawnCoins(coinType);
            }

            UpdateUI();
        }

        void SpawnCoins(CoinType coinType)
        {
            for (int i = 0; i < coinType.count; i++)
            {
                Vector3 randomPos = coinPileArea.position + Random.insideUnitSphere * 2f;
                randomPos.z = 0; // 2D game
                
                GameObject newCoin = Instantiate(coinType.prefab, randomPos, Quaternion.identity);
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
    }
} 