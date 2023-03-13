using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelCard : MonoBehaviour
{
    private Image StarImage;

    private Button _mainButton;
    [SerializeField] private Text _title;
    [SerializeField] private Image _levelImage;
    private Star[] _stars;

    public void GenerateFromLevel(LevelData level)
    {

    }
}