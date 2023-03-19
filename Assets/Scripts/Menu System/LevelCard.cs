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
    public const float CARD_WIDTH = 150;

    private LevelData _levelReference;

    private const float STAR_MARGIN = 12.5f;

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
            starRectTransform.anchorMin = new Vector2(0.5f, 1);
            starRectTransform.anchorMax = new Vector2(0.5f, 1);
            starRectTransform.pivot = new Vector2(0, 0);

            float xPos = ((i % 3) - 1) * (Star.STAR_SIZE + STAR_MARGIN) - (Star.STAR_SIZE / 2);
            int row = i / 3;
            float yPos = -(150 + row * (Star.STAR_SIZE + STAR_MARGIN));

            starRectTransform.anchoredPosition = new Vector2(xPos, yPos);

            _stars[i] = currentStar;
        }
    }

    public void ReloadData()
    {

    }
}