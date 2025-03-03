using UnityEngine;
using UnityEngine.UI;

public class CGCardInput : MonoBehaviour
{
    private CGCard card;
    private CGGameManager gameManager;
    private Button button;

    void Start()
    {
        card = GetComponent<CGCard>();
        gameManager = FindObjectOfType<CGGameManager>();
        button = GetComponent<Button>();
        
        // เพิ่ม listener ให้กับปุ่ม
        button.onClick.AddListener(() => {
            gameManager.OnCardClicked(card);
        });
    }
} 