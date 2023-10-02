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
        [SerializeField] private TextMeshProUGUI _resultPoliceText;
        [SerializeField] private GameLevels _allLevels;
        [SerializeField] private Image _background;
        [SerializeField] private Sprite _winSprite;
        [SerializeField] private Sprite _failSprite;

        void Start()
        {
            Camera.backgroundColor = Globals.EndSceneCameraBackgroundColor;
            FadeIn();

            GameResultType resultType = (GameResultType)PlayerPrefs.GetInt("ld54_lastresulttype", 0);
            switch (resultType)
            {
                case GameResultType.None:
                    Debug.LogError("Shouldn't happen");
                    _resultText.text = "you broke it";
                    break;
                case GameResultType.Success:
                    JamKit.Play("Win");
                    _resultText.gameObject.SetActive(true);
                    _resultPoliceText.gameObject.SetActive(false);
                    _background.sprite = _winSprite;
                    _resultText.text = _allLevels.LevelSuccessText;
                    break;
                case GameResultType.SuccessBest:
                    JamKit.Play("Win");
                    _resultText.gameObject.SetActive(true);
                    _resultPoliceText.gameObject.SetActive(false);
                    _background.sprite = _winSprite;
                    _resultText.text = _allLevels.LevelSuccessBestText;
                    break;
                case GameResultType.FailUndervalue:
                    JamKit.Play("Fail");
                    _resultText.gameObject.SetActive(true);
                    _resultPoliceText.gameObject.SetActive(false);
                    _background.gameObject.SetActive(false);
                    _resultText.text = _allLevels.LevelFailUndervalueText;
                    break;
                case GameResultType.FailPolice:
                    JamKit.Play("Fail");
                    _resultText.gameObject.SetActive(false);
                    _resultPoliceText.gameObject.SetActive(true);
                    _background.sprite = _failSprite;
                    _resultText.text = _allLevels.LevelFailPoliceText;
                    break;
            }
        }

        public void OnClickedPlayButton()
        {
            JamKit.Play("ButtonClick");
            _playButton.interactable = false;
            FadeOut(null, () => SceneManager.LoadScene("Splash"));
        }
    }
}