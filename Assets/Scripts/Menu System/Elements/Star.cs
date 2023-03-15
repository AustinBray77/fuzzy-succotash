using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Star : MonoBehaviour
{
    public const float STAR_SIZE = 25;


    [SerializeField] private Image _complete;
    [SerializeField] private Image _incomplete;
    [SerializeField] private Image _locked;

    private Button _mainButton;

    private void Awake()
    {
        _mainButton = GetComponent<Button>();
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