using JamKit;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class EndUi : UiBase
    {
        [SerializeField] private Button _playButton;

        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private GameLevels _allLevels;
        [SerializeField] private Image _background;
        [SerializeField] private Sprite _winSprite;
        [SerializeField] private Sprite _failSprite;

        int _currentLevelIndex = 0;
        bool _hasWon = false;

        void Start()
        {
            Camera.backgroundColor = Globals.EndSceneCameraBackgroundColor;
            FadeIn();

            _currentLevelIndex = PlayerPrefs.GetInt("ld54_currentlevelindex", 0);
            GameResultType resultType = (GameResultType)PlayerPrefs.GetInt("ld54_lastresulttype", 0);
            switch (resultType)
            {
                case GameResultType.None:
                    Debug.LogError("Shouldn't happen");
                    _resultText.text = "you broke it";
                    break;
                case GameResultType.Success:
                    JamKit.Play("Win");
                    _currentLevelIndex++;
                    _background.sprite = _winSprite;
                    _hasWon = _currentLevelIndex == _allLevels.Levels.Count;
                    if (_hasWon)
                    {
                        _resultText.text = _allLevels.GameWinText;
                    }
                    else
                    {
                        _resultText.text = _allLevels.LevelSuccessText;
                    }

                    break;
                case GameResultType.FailUndervalue:
                    JamKit.Play("Fail");
                    _background.gameObject.SetActive(false);
                    _resultText.text = _allLevels.LevelFailUndervalueText;
                    break;
                case GameResultType.FailPolice:
                    JamKit.Play("Fail");
                    _background.sprite = _failSprite;
                    _resultText.text = _allLevels.LevelFailPoliceText;
                    break;
            }

            PlayerPrefs.SetInt("ld54_currentlevelindex", _hasWon ? 0 : _currentLevelIndex);
        }

        public void OnClickedPlayButton()
        {
            string nextSceneName = _hasWon ? "Splash" : "Game";
            JamKit.Play("ButtonClick");
            _playButton.interactable = false;
            FadeOut(null, () => SceneManager.LoadScene(nextSceneName));
        }
    }
}