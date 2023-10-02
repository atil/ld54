using JamKit;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class SplashUi : UiBase
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private GameLevels _allLevels;
        [SerializeField] private GameObject _splashItself;
        [SerializeField] private GameObject _storyRoot;
        [SerializeField] private TextMeshProUGUI _storyText;
        [SerializeField] private Button _clickToProceedButton;

        private Coroutine _showTextCoroutine;
        private bool _isLoadingGame = false;

        private string[] _storyLines = new[]
        {
            "A BEAUTIFUL HEIST. FULL OF OPPORTUNITY.",
            "HAVE TO REACH A CERTAIN AMOUNT.",
            "WHAT'S THE POINT OTHERWISE?",
            "MUST PICK CAREFULLY.",
            "THE KNAPSACK CAN ONLY CARRY SO MUCH.",
            "BOTHER.",
            "TRIGGERED THE ALARM. FEDS ARE ON THE WAY. GOTTA GO FAST."
        };

        private float[] _storyLineWaitDurations = new[]
        {
            1.0f,
            1.0f,
            1.0f,
            1.0f,
            3.5f,
            0.5f,
            0.5f,
        };

        private bool[] _storyLinesHasExtraNewlineAfter = new[]
        {
            false,
            false,
            false,
            false,
            true,
            true,
            false,
        };

        private enum SplashState
        {
            Splash,
            Story
        }
        private SplashState _state = SplashState.Splash;

        void Start()
        {
            JamKit.StartMusic("MusicIntro", false);
            Camera.backgroundColor = Globals.SplashSceneCameraBackgroundColor;
            FadeIn();
        }

        public void OnClickedPlayButton()
        {
            JamKit.Play("ButtonClick");
            _playButton.interactable = false;
            FadeOut(null, () =>
            {
                _state = SplashState.Story;
                _splashItself.SetActive(false);
                _storyRoot.SetActive(true);
                _storyText.text = "";
                FadeIn(null, () => ShowStoryText());
            });
        }

        private void ShowStoryText()
        {
            _showTextCoroutine = JamKit.Run(ShowStoryTextCoroutine());
        }

        private IEnumerator ShowStoryTextCoroutine()
        {
            for (int i = 0; i < _storyLines.Length; i++)
            {
                string line = _storyLines[i];
                foreach (char ch in line)
                {
                    _storyText.text += ch;
                    const float DurationPerChar = 0.025f;
                    yield return new WaitForSeconds(DurationPerChar);
                }
                yield return new WaitForSeconds(_storyLineWaitDurations[i]);

                if (i != _storyLines.Length - 1)
                {
                    _storyText.text += "\n";

                    if (_storyLinesHasExtraNewlineAfter[i]) _storyText.text += "\n";
                }
            }
            _clickToProceedButton.gameObject.SetActive(true);
        }

        public void OnClickedProceedButton()
        {
            JamKit.Play("ButtonClick");
            JamKit.FadeOutMusic(0.4f);
            _clickToProceedButton.interactable = false;
            _isLoadingGame = true;
            FadeOut(null, () => SceneManager.LoadScene("Game"));
            PlayerPrefs.SetInt("ld54_isfirstrun", 1);
        }

        public void Update()
        {
            if (_state == SplashState.Story && Input.anyKeyDown && !_isLoadingGame)
            {
                bool isFirstRun = PlayerPrefs.GetInt("ld54_isfirstrun", 0) == 0;
                if (!isFirstRun)
                {
                    JamKit.Stop(_showTextCoroutine);
                    OnClickedProceedButton();
                }
            }
        }
    }
}