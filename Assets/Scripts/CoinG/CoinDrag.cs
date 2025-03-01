using UnityEngine;
using System.Collections;
using CoinGame;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CoinDrag : MonoBehaviour
{
    public int coinValue;
    public CoinManager manager;
    private bool isDragging = false;
    private Vector3 offset;
    private bool isScored = false;

    public Text warningText;

    private Vector3 startPosition;

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

        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }

        startPosition = transform.position;
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
                if (distance < 50f)
                {
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayCoinDrop();
                        Debug.Log("Playing coin drop sound from CheckCoinPlacement");
                    }
                    
                    manager.AddCoinToScore(gameObject, coinValue);
                    Destroy(gameObject);
                    return;
                }
            }
            else
            {
                float wrongDistance = Vector2.Distance(transform.position, coinType.sortingArea.position);
                if (wrongDistance < 29f)
                {
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayWrongPlace();
                    }
                    AlertManager.ShowAlert($"เหรียญ {coinValue} บาท วางผิดที่!");
                    transform.position = GetStartPosition();
                    return;
                }
            }
        }
    }

    void ShowWarningMessage(string message)
    {
        if (warningText != null)
        {
            warningText.text = message;
            warningText.gameObject.SetActive(true);

            CancelInvoke("HideWarningMessage");
            Invoke("HideWarningMessage", 2f);
        }
    }

    void HideWarningMessage()
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
            Debug.Log("ข้อความเตือนถูกซ่อน");
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    Vector3 GetStartPosition()
    {
        return startPosition;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (manager != null && !isScored)
        {
            if ((coinValue == 10 && other.gameObject.name.Contains("Space10Bath")) ||
                (coinValue == 5 && other.gameObject.name.Contains("Space5Bath")) ||
                (coinValue == 1 && other.gameObject.name.Contains("Space1Bath")))
            {
                isScored = true;
                manager.AddCoinToScore(gameObject, coinValue);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (manager != null && isScored && manager.IsGameActive())
        {
            if ((coinValue == 10 && other.gameObject.name.Contains("Space10Bath")) ||
                (coinValue == 5 && other.gameObject.name.Contains("Space5Bath")) ||
                (coinValue == 1 && other.gameObject.name.Contains("Space1Bath")))
            {
                isScored = false;
                manager.SubtractScore(coinValue);
            }
        }
    }
} 