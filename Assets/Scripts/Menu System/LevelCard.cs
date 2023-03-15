using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button), typeof(RectTransform))]
public class LevelCard : MonoBehaviour
{
    private Button _mainButton;
    private RectTransform _rectTransform;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Image _levelImage;
    private Star[] _stars;

    public void GenerateFromLevel(LevelData level)
    {
        _mainButton = GetComponent<Button>();
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
            RectTransform CurrentStar = Instantiate(MenuController.Instance.Prefabs.Star, transform).GetComponent<RectTransform>();

            //Set anchor of star to top left in main transform
            CurrentStar.anchorMin = new Vector2(0, 0);
            CurrentStar.anchorMax = new Vector2(1, 1);
            CurrentStar.pivot = new Vector2(0, 0);

            float xPos = i * (Star.STAR_SIZE + 5);
            float yPos = _rectTransform.sizeDelta.y - 25;

            CurrentStar.anchoredPosition = new Vector2(xPos, yPos);

            _stars[i] = CurrentStar.GetComponent<Star>();
        }
    }
}