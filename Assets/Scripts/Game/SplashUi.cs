using JamKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class SplashUi : UiBase
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private GameLevels _allLevels;

        void Start()
        {
            Camera.backgroundColor = Globals.SplashSceneCameraBackgroundColor;
            FadeIn();
        }

        public void OnClickedPlayButton()
        {
            _playButton.interactable = false;
            FadeOut(null, () => SceneManager.LoadScene("Game"));
        }
    }
}