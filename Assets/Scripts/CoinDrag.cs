using UnityEngine;
using System.Collections;
using CoinGame;

public class CoinDrag : MonoBehaviour
{
    public int coinValue;
    public CoinManager manager;
    private bool isDragging = false;
    private Vector3 offset;

    void Start()
    {
        if (manager == null)
        {
            manager = GameObject.FindObjectOfType<CoinManager>();
            if (manager == null)
            {
                Debug.LogError("CoinManager not found in scene!");
            }
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 newPosition = GetMouseWorldPosition() + offset;
            transform.position = new Vector3(newPosition.x, newPosition.y, 0);
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            if (manager != null)
            {
                CheckCoinPlacement();
            }
            else
            {
                Debug.LogError("CoinManager not found!");
            }
        }
    }

    void CheckCoinPlacement()
    {
        if (manager == null || manager.coinTypes == null) return;

        foreach (var coinType in manager.coinTypes)
        {
            if (coinType == null || coinType.sortingArea == null) continue;

            if (coinValue == coinType.value)
            {
                float distance = Vector2.Distance(transform.position, coinType.sortingArea.position);
                if (distance < 31f)
                {
                    manager.AddCoinToScore(coinValue);
                    Destroy(gameObject);
                    return;
                }
            }
            else
            {
                float wrongDistance = Vector2.Distance(transform.position, coinType.sortingArea.position);
                if (wrongDistance < 29f)
                {
                    manager.SubtractScore(coinType.value);
                    return;
                }
            }
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
} 