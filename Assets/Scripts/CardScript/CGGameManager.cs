using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CGGameManager : MonoBehaviour
{
    public CGCard cardPrefab;
    public Sprite[] cardSprites;
    public float spacingX = 220f; // ระยะห่างแนวนอน
    public float spacingY = 320f; // ระยะห่างแนวตั้ง
    public int rows = 4;
    public int cols = 4;
    public float startX = -330f; // ตำแหน่งเริ่มต้นแกน X
    public float startY = 480f;  // ตำแหน่งเริ่มต้นแกน Y

    private CGCard firstCard;
    private CGCard secondCard;
    private bool canFlip = true;
    private int matchesFound = 0;
    private int totalMatches;

    void Start()
    {
        totalMatches = (rows * cols) / 2;
        CreateCards();
    }

    void CreateCards()
    {
        List<int> cardIds = new List<int>();
        for (int i = 0; i < (rows * cols) / 2; i++)
        {
            cardIds.Add(i);
            cardIds.Add(i);
        }
        
        // สลับตำแหน่งไพ่
        for (int i = 0; i < cardIds.Count; i++)
        {
            int temp = cardIds[i];
            int randomIndex = Random.Range(i, cardIds.Count);
            cardIds[i] = cardIds[randomIndex];
            cardIds[randomIndex] = temp;
        }

        int index = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                float posX = startX + (col * spacingX);
                float posY = startY - (row * spacingY);
                
                CGCard card = Instantiate(cardPrefab, transform);
                card.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
                card.cardId = cardIds[index];
                card.cardFront = cardSprites[cardIds[index]];
                index++;
            }
        }
    }

    public void OnCardClicked(CGCard card)
    {
        if (!canFlip || card.isMatched || card.isFlipped) return;

        card.FlipCard();

        if (firstCard == null)
        {
            firstCard = card;
        }
        else
        {
            secondCard = card;
            canFlip = false;
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1f);

        if (firstCard.cardId == secondCard.cardId)
        {
            firstCard.isMatched = true;
            secondCard.isMatched = true;
            firstCard.SetInteractable(false);
            secondCard.SetInteractable(false);
            matchesFound++;

            if (matchesFound == totalMatches)
            {
                Debug.Log("Game Complete!");
            }
        }
        else
        {
            firstCard.FlipCard();
            secondCard.FlipCard();
        }

        firstCard = null;
        secondCard = null;
        canFlip = true;
    }
} 