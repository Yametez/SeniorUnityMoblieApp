using UnityEngine;
using System.Collections.Generic;
using CoinGame;

public class CoinDrag : MonoBehaviour
{
    public UIManager uiManager;
    public int coinValue;
    public CoinManager manager;
    
    private Dictionary<int, int> sortedCoins = new Dictionary<int, int>()
    {
        {1, 0},
        {5, 0},
        {10, 0}
    };

    public void AddSortedCoin(int coinValue)
    {
        sortedCoins[coinValue]++;
        if (uiManager != null)
        {
            uiManager.UpdateCoinCount(coinValue, sortedCoins[coinValue]);
        }
    }
} 