using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class LevelCard : MonoBehaviour
{
    private RectTransform _rectTransform;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Image _levelImage;
    private Star[] _stars;

    public const float CARD_HEIGHT = 200;
    public const float CARD_WIDTH = 100;

    private LevelData _levelReference;

    public void GenerateFromLevel(LevelData level)
    {
        _rectTransform = GetComponent<RectTransform>();

        _title.text = level.Title;
        _levelImage = level.Thumbnail;

        PlaceStars(level);
    }

    private void PlaceStars(LevelData level)
    {
        _stars = new Star[level.NumberOfStages];

        for (int i = 0; i < level.NumberOfStages; i++)
        {
            Star currentStar = Instantiate(MenuController.Instance.Prefabs.Star, transform).GetComponent<Star>();

            currentStar.Initialize(level.Index, i);

            RectTransform starRectTransform = currentStar.GetComponent<RectTransform>();

            //Set anchor of star to top left in main transform
            starRectTransform.anchorMin = new Vector2(0, 1);
            starRectTransform.anchorMax = new Vector2(0, 1);
            starRectTransform.pivot = new Vector2(0, 0);

            float xPos = i * (Star.STAR_SIZE + 5);
            float yPos = -(_rectTransform.sizeDelta.y - 25);

            starRectTransform.anchoredPosition = new Vector2(xPos, yPos);

            _stars[i] = currentStar;
        }
    }

    public void ReloadData()
    {

    }
}