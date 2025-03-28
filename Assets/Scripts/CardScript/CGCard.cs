using UnityEngine;
using UnityEngine.UI;

public class CGCard : MonoBehaviour
{
    public int cardId;
    public bool isMatched = false;
    public bool isFlipped = false;
    
    private Image imageRenderer;
    public Sprite cardFront;
    public Sprite cardBack;
    private Button button;
    private CGAudioManager audioManager;

    void Awake()
    {
        imageRenderer = GetComponent<Image>();
        button = GetComponent<Button>();
        // ตั้งค่ารูปด้านหลังไพ่เริ่มต้น
        imageRenderer.sprite = cardBack;
    }

    void Start()
    {
        audioManager = FindObjectOfType<CGAudioManager>();
    }

    public void FlipCard()
    {
        isFlipped = !isFlipped;
        imageRenderer.sprite = isFlipped ? cardFront : cardBack;
        
        // เล่นเสียงเปิดไพ่
        if (audioManager != null)
        {
            audioManager.PlayCardFlip();
        }
    }

    public void SetInteractable(bool interactable)
    {
        button.interactable = interactable;
    }
} 