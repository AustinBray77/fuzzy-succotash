using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelSelectScreen : MonoBehaviour, IScreen
{
    private class Page
    {
        public Page(LevelCard[] cards, GameObject _object)
        {
            Cards = cards;
            Object = _object;
        }

        public GameObject Object { get; private set; }
        public LevelCard[] Cards { get; private set; }

        public void ReloadCards()
        {
            foreach (LevelCard card in Cards)
            {
                card.ReloadData();
            }
        }
    }

    [SerializeField] private List<GameObject> _screenElements;
    private Page[] _pages;
    private int _currentLevelPage;
    public GameObject[] ScreenElements { get => _screenElements.ToArray(); }
    public string Name { get => "LevelSelect"; }
    private const float s_cardMargin = 12.5f;

    [SerializeField] private Button _nextPageButton;
    [SerializeField] private Button _previousPageButton;

    public void Initialize()
    {
        GeneratePages();

        _nextPageButton.onClick.AddListener(NextPage);
        _previousPageButton.onClick.AddListener(PreviousPage);

        _screenElements.Add(_nextPageButton.gameObject);
        _screenElements.Add(_previousPageButton.gameObject);
    }

    public void Load()
    {
        Functions.SetActiveAllObjects(ScreenElements, true);

        ReloadPages();

        _pages[0].Object.SetActive(true);
        _currentLevelPage = 0;
        _previousPageButton.gameObject.SetActive(false);
    }

    public void Unload()
    {
        Functions.SetActiveAllObjects(ScreenElements, false);
        _pages[_currentLevelPage].Object.SetActive(false);
    }

    public void GeneratePages()
    {
        int rows = (int)(Screen.height / (LevelCard.CARD_HEIGHT + 2 * s_cardMargin));
        int cardsPerRow = (int)((Screen.width - 100) / (LevelCard.CARD_WIDTH + 2 * s_cardMargin));

        int cardsPerPage = (rows * cardsPerRow);
        int numberofPages = Mathf.CeilToInt(LevelHandler.Instance.LevelReferences.Count / (float)cardsPerPage);
        _pages = new Page[numberofPages];

        for (int i = 0; i < numberofPages; i++)
        {
            int startIndex = i * cardsPerPage;
            int endIndex = Mathf.Min((i + 1) * cardsPerPage - 1, LevelHandler.Instance.LevelReferences.Count - 1);
            GameObject pageObject = new GameObject("Page " + i, typeof(RectTransform));

            RectTransform pageRect = pageObject.GetComponent<RectTransform>();

            //Makes so the page fills the whole screen
            pageRect.anchorMin = Vector2.zero;
            pageRect.anchorMax = Vector2.one;
            pageRect.offsetMin = Vector2.zero;
            pageRect.offsetMax = Vector2.zero;

            LevelCard[] levelCards = GenerateLevelCards(pageObject.transform, startIndex, endIndex, rows, cardsPerRow);
            _pages[i] = new Page(levelCards, pageObject);
            _pages[i].Object.transform.parent = transform;
            _screenElements.Add(_pages[i].Object);
        }
    }

    public LevelCard[] GenerateLevelCards(Transform page, int start, int end, int rows, int cardsPerRow)
    {
        LevelCard[] levelCards = new LevelCard[rows * cardsPerRow];

        int row = 0, column = 0;

        for (int i = start; i <= end; i++, column++)
        {
            if (column >= cardsPerRow)
            {
                column = 0;
                row++;
            }

            LevelCard currentCard = Instantiate(MenuController.Instance.Prefabs.LevelCard, transform).GetComponent<LevelCard>();

            currentCard.GenerateFromLevel(LevelHandler.Instance.LevelReferences[i].Data);

            RectTransform cardTransform = currentCard.GetComponent<RectTransform>();

            //Sets anchor to the top left
            cardTransform.anchorMin = new Vector2(0, 1);
            cardTransform.anchorMax = new Vector2(0, 1);
            cardTransform.pivot = new Vector2(0, 0);

            float xPos = column * (LevelCard.CARD_WIDTH + 2 * s_cardMargin) + s_cardMargin + 50;
            float yPos = -((row + 1) * (LevelCard.CARD_HEIGHT + 2 * s_cardMargin) + s_cardMargin);

            cardTransform.anchoredPosition = new Vector2(xPos, yPos);

            currentCard.transform.parent = page;
            levelCards[i] = currentCard;
        }

        return levelCards;
    }

    public void NextPage()
    {
        _pages[_currentLevelPage].Object.SetActive(false);
        _currentLevelPage++;
        _pages[_currentLevelPage].Object.SetActive(true);

        _nextPageButton.gameObject.SetActive(_currentLevelPage < _pages.Length);
    }

    public void PreviousPage()
    {
        _pages[_currentLevelPage].Object.SetActive(false);
        _currentLevelPage--;
        _pages[_currentLevelPage].Object.SetActive(true);

        _previousPageButton.gameObject.SetActive(_currentLevelPage > 0);
    }

    private void ReloadPages()
    {
        foreach (Page page in _pages)
        {
            page.ReloadCards();
        }
    }
}
