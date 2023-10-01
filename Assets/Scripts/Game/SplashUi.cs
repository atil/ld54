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

        private enum SplashState
        {
            Splash,
            Story
        }
        private SplashState _state = SplashState.Splash;
        private string[] _storyLines;

        void Start()
        {
            Camera.backgroundColor = Globals.SplashSceneCameraBackgroundColor;
            FadeIn();
        }

        public void OnClickedPlayButton()
        {
            _playButton.interactable = false;
            FadeOut(null, () =>
            {
                _state = SplashState.Story;
                _splashItself.SetActive(false);
                _storyRoot.SetActive(true);
                _storyLines = _storyText.text.Split('\n');
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
                int waitDurationAfterLine = int.Parse(line[0].ToString());
                string lineWithoutNumber = line.Substring(1, line.Length - 1);

                foreach (char ch in lineWithoutNumber)
                {
                    _storyText.text += ch;
                    const float DurationPerChar = 0.025f;
                    yield return new WaitForSeconds(DurationPerChar);
                }
                yield return new WaitForSeconds(waitDurationAfterLine);
                
                if (i != _storyLines.Length - 1)
                {
                    _storyText.text += "\n\n"; 
                }
            }
            _clickToProceedButton.gameObject.SetActive(true);
        }

        public void OnClickedProceedButton()
        {
            _clickToProceedButton.interactable = false;
            FadeOut(null, () => SceneManager.LoadScene("Game"));
        }

        public void Update()
        {
            if (_state == SplashState.Story && Input.anyKeyDown)
            {
                JamKit.Stop(_showTextCoroutine);
                OnClickedProceedButton();
            }
        }
    }
}