using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Star : MonoBehaviour
{
    public const float STAR_SIZE = 25;


    [SerializeField] private Sprite _complete;
    [SerializeField] private Sprite _incomplete;
    [SerializeField] private Sprite _locked;

    private Button _mainButton;

    private IEnumerator LoadLevel(int level, int stage)
    {
        LevelHandler.Instance.LoadLevel(level, stage, (float)AnimationManager.Instance.fadeTime); //Create a better way to get the extra time later
        yield return StartCoroutine(MenuController.Instance.OpenScreen("Game"));
    }

    public void Initialize(int level, int stage)
    {
        _mainButton = GetComponent<Button>();

        void onClick()
        {
            StartCoroutine(LoadLevel(level, stage));
        }

        _mainButton.onClick.AddListener(onClick);

        SetStarState(LevelHandler.Instance.LevelReferences[level].GetStageState(stage));
    }

    public void SetStarState(StageState state)
    {
        switch (state)
        {
            case StageState.Locked:
                _mainButton.image.sprite = _locked;
                _mainButton.enabled = false;
                return;
            case StageState.Unlocked:
                _mainButton.image.sprite = _incomplete;
                _mainButton.enabled = true;
                return;
            case StageState.Completed:
                _mainButton.image.sprite = _complete;
                _mainButton.enabled = true;
                return;
            default:
                Debug.LogWarning("Invalid state passed into SetStarState()");
                return;
        }
    }
}