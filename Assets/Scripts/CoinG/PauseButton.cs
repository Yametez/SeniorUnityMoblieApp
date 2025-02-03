using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private Sprite playSprite;
    [SerializeField] private Sprite pauseSprite;
    private Image buttonImage;

    void Start()
    {
        buttonImage = GetComponent<Image>();
    }

    public void OnPauseButtonClick()
    {
        GameManager.Instance.TogglePause();
        buttonImage.sprite = GameManager.Instance.isPaused ? playSprite : pauseSprite;
    }
} 