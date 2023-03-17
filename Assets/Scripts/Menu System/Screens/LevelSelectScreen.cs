using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectScreen : MonoBehaviour, IScreen
{
    [SerializeField] private GameObject[] _screenElements;
    private LevelCard[] _levelCards;
    private int _currentLevelPage;
    public GameObject[] ScreenElements { get => _screenElements; }
    public string Name { get => "LevelSelect"; }

    public void Initialize()
    {
        GenerateLevelCards();
    }

    public void Load()
    {
        Functions.SetActiveAllObjects(ScreenElements, true);
    }

    public void Unload()
    {
        Functions.SetActiveAllObjects(ScreenElements, false);
    }

    public void GenerateLevelCards()
    {
        float CardMargin = 12.5f;
        int Rows = (int)(Screen.height / (LevelCard.CARD_HEIGHT + 2 * CardMargin));
        int CardsPerRow = (int)(Screen.width / (LevelCard.CARD_WIDTH + 2 * CardMargin));

        _levelCards = new LevelCard[Rows * CardsPerRow];

        GameObject[] newScreenElements = new GameObject[_screenElements.Length + _levelCards.Length];
        Array.Copy(_screenElements, newScreenElements, _screenElements.Length);

        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < CardsPerRow; j++)
            {
                int OneDimensionalIndex = i * CardsPerRow + j;

                if (OneDimensionalIndex >= LevelHandler.Instance.LevelReferences.Length)
                    break;

                LevelCard currentCard = Instantiate(MenuController.Instance.Prefabs.LevelCard, transform).GetComponent<LevelCard>();

                currentCard.GenerateFromLevel(LevelHandler.Instance.LevelReferences[OneDimensionalIndex].Data);

                RectTransform cardTransform = currentCard.GetComponent<RectTransform>();

                //Sets anchor to the top left
                cardTransform.anchorMin = new Vector2(0, 1);
                cardTransform.anchorMax = new Vector2(0, 1);
                cardTransform.pivot = new Vector2(0, 0);

                float xPos = j * (LevelCard.CARD_WIDTH + 2 * CardMargin) + CardMargin;
                float yPos = -((i + 1) * (LevelCard.CARD_HEIGHT + 2 * CardMargin) + CardMargin);

                cardTransform.anchoredPosition = new Vector2(xPos, yPos);

                _levelCards[OneDimensionalIndex] = currentCard;
                newScreenElements[_screenElements.Length + OneDimensionalIndex] = currentCard.gameObject;
            }

        _screenElements = newScreenElements;

    }
}
