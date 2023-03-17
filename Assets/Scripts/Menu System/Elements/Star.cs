using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Star : MonoBehaviour
{
    public const float STAR_SIZE = 25;


    [SerializeField] private Image _complete;
    [SerializeField] private Image _incomplete;
    [SerializeField] private Image _locked;

    private Button _mainButton;

    public void Initialize(int level, int stage)
    {
        _mainButton = GetComponent<Button>();

        UnityAction onClick = () =>
        {
            MenuController.Instance.OpenScreen("Game");
            LevelHandler.Instance.LoadLevel(level, stage);
        };

        _mainButton.onClick.AddListener(onClick);
    }

    public void SetStarState(int state)
    {
        switch (state)
        {
            case 0:
                _mainButton.image = _locked;
                return;
            case 1:
                _mainButton.image = _incomplete;
                return;
            case 2:
                _mainButton.image = _complete;
                return;
            default:
                Debug.LogWarning("Invalid state passed into SetStarState()");
                return;
        }
    }
}