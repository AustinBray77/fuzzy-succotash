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
        int CardMargin = 25;
        int Rows = (int)(Screen.height / (LevelCard.CARD_HEIGHT + 2 * CardMargin));
        int CardsPerRow = (int)(Screen.width / (LevelCard.CARD_WIDTH + 2 * CardMargin));

        _levelCards = new LevelCard[Rows * CardsPerRow];

        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < CardsPerRow; j++)
            {
                int OneDimensionalIndex = i * CardsPerRow + j;
                LevelCard currentCard = Instantiate(MenuController.Instance.Prefabs.LevelCard, transform).GetComponent<LevelCard>();

                //*** FOR THE FUTURE WHEN A LEVEL CONTROLLER TO LEVEL DATA FUNCTION IS CREATED ***//
                //currentCard.GenerateFromLevel(LevelHandler.Instance.LevelReferences[OneDimensionalIndex].GetData());

                RectTransform cardTransform = currentCard.GetComponent<RectTransform>();

                //Sets anchor to the top left
                cardTransform.anchorMin = new Vector2(0, 0);
                cardTransform.anchorMax = new Vector2(1, 1);
                cardTransform.pivot = new Vector2(0, 0);

                float xPos = j * (LevelCard.CARD_WIDTH + 2 * CardMargin) + CardMargin;
                float yPos = i * (LevelCard.CARD_HEIGHT + 2 * CardMargin) + CardMargin;

                cardTransform.anchoredPosition = new Vector2(xPos, yPos);

                _levelCards[OneDimensionalIndex] = currentCard;
            }
    }
}
