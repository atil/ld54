using JamKit;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public enum GameResultType
    {
        None,
        Success,
        SuccessBest,
        FailUndervalue,
        FailPolice
    }

    public class GameMain : MonoBehaviour
    {
        [SerializeField] private JamKit.JamKit _jamkit;
        [SerializeField] private GameUi _ui;
        [SerializeField] private GameLevels _allLevels;

        [Header("UI")]
        [SerializeField] private GameObject _cardPrefab;
        [SerializeField] private Image _roomBackground;
        [SerializeField] private Transform _roomBackgrounLerpTargetLeft;
        [SerializeField] private Transform _roomBackgrounLerpTargetRight;
        [SerializeField] private TextMeshProUGUI _levelNameText;
        [SerializeField] private TextMeshProUGUI _roomNameText;
        [SerializeField] private Transform _roomCardsRoot;
        [SerializeField] private GameObject _roomCardInvisible;
        [SerializeField] private Transform _handCardsRoot;
        [SerializeField] private GameObject _handCardInvisible;
        [SerializeField] private Button _prevRoomButton;
        [SerializeField] private Button _nextRoomButton;
        [SerializeField] private TextMeshProUGUI _prevRoomText;
        [SerializeField] private TextMeshProUGUI _nextRoomText;
        [SerializeField] private Slider _timerSlider;
        [SerializeField] private GameObject _tooHeavyText;

        [Header("Status")]
        [SerializeField] private TextMeshProUGUI _weightCapacityText;
        [SerializeField] private TextMeshProUGUI _weightCurrentText;
        [SerializeField] private Slider _weightSlider;
        [SerializeField] private TextMeshProUGUI _moneyGoalText;
        [SerializeField] private TextMeshProUGUI _moneyCurrentText;
        [SerializeField] private Slider _moneySlider;

        private int _currentLevelIndex => 0;
        private GameLevelData CurrentLevel => _allLevels.Levels[_currentLevelIndex];
        private IReadOnlyList<Room> CurrentLevelRooms => CurrentLevel.Rooms;

        private int _currentRoomIndex = 0;
        private float _timer = 0;
        private List<CardView> _currentRoomCardViews = new();
        private List<List<CardData>> _currentLevelCards = new();

        private List<CardData> _handCards = new();
        private List<CardView> _handCardViews = new();

        private int StatusWeight => _handCards.Sum(x => x.Weight);
        private int StatusMoney => _handCards.Sum(x => x.Money);

        private bool _isShowingTooHeavy = false;
        private bool _isGameRunning = true;
        private const float RoomChangeDuration = 0.5f;

        private void Start()
        {
            _jamkit.ChangeMusicTrack("MusicGame");

            foreach (Room room in CurrentLevelRooms)
            {
                _currentLevelCards.Add(new List<CardData>(room.Cards));
            }

            _levelNameText.text = $"{CurrentLevel.LevelName}";
            _weightCapacityText.text = $"{CurrentLevel.BackpackCapacity}";
            _moneyGoalText.text = $"{CurrentLevel.MoneyGoal}";

            SetWithRoom(_currentRoomIndex);
            SetStatus();
        }

        private void Update()
        {
            if (!_isGameRunning) { return; }

            _timer += Time.deltaTime;
            _timerSlider.value = (CurrentLevel.TimeLimit - _timer) / CurrentLevel.TimeLimit;

            if (_timer >= CurrentLevel.TimeLimit)
            {
                EndGame(GameResultType.FailPolice);
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.H))
            {
                EndGame(GameResultType.Success);
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                EndGame(GameResultType.SuccessBest);
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                EndGame(GameResultType.FailUndervalue);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                EndGame(GameResultType.FailPolice);
            }
#endif
        }

        private void SetStatus()
        {
            int newWeight = StatusWeight;
            _weightCurrentText.text = $"{newWeight}";
            _weightSlider.value = (float)newWeight / (float)CurrentLevel.BackpackCapacity;

            int newMoney = StatusMoney;
            _moneyCurrentText.text = $"{newMoney}";
            _moneySlider.value = (float)newMoney / (float)CurrentLevel.MoneyGoal;
        }

        private void SetWithRoom(int roomIndex)
        {
            Room newRoom = CurrentLevelRooms[roomIndex];
            _roomBackground.sprite = newRoom.Background;
            _roomNameText.text = newRoom.Name;
            _prevRoomText.text = _currentRoomIndex == 0 ? "EXIT" : CurrentLevelRooms[_currentRoomIndex - 1].Name;
            _nextRoomText.text = _currentRoomIndex == CurrentLevelRooms.Count - 1 ? "" : CurrentLevelRooms[_currentRoomIndex + 1].Name;
            foreach (Transform t in _roomCardsRoot)
            {
                if (t == _roomCardInvisible.transform) { continue; }

                Destroy(t.gameObject);
            }
            _currentRoomCardViews.Clear();

            List<CardData> roomCards = _currentLevelCards[roomIndex];
            foreach (CardData card in roomCards)
            {
                CardView cardView = Instantiate(_cardPrefab, _roomCardsRoot).GetComponent<CardView>();
                cardView.Set(card);
                _currentRoomCardViews.Add(cardView);
            }

            _nextRoomButton.gameObject.SetActive(_currentRoomIndex != CurrentLevelRooms.Count - 1);
        }

        public void OnPrevRoomClicked()
        {
            _jamkit.Play("ButtonClick");

            if (_currentRoomIndex == 0)
            {
                GameResultType result = GameResultType.Success;
                if (StatusMoney >= CurrentLevel.MoneyGoal)
                {
                    result = StatusMoney == CurrentLevel.PossibleMoney ? GameResultType.SuccessBest : GameResultType.Success;
                }
                else
                {
                    result = GameResultType.FailUndervalue;
                }
                EndGame(result);
                return;
            }

            PlayRoomChangeAnimation(_roomBackgrounLerpTargetRight, _roomBackgrounLerpTargetLeft, _currentRoomIndex - 1);

            _roomNameText.gameObject.SetActive(false);
            _roomCardsRoot.gameObject.SetActive(false);
            _prevRoomButton.gameObject.SetActive(false);
            _nextRoomButton.gameObject.SetActive(false);
            _jamkit.RunDelayed(RoomChangeDuration, () =>
            {
                _roomNameText.gameObject.SetActive(true);
                _roomCardsRoot.gameObject.SetActive(true);
                _prevRoomButton.gameObject.SetActive(true);
                _nextRoomButton.gameObject.SetActive(true);

                _currentRoomIndex--;
                SetWithRoom(_currentRoomIndex);
            });
        }

        public void OnNextRoomClicked()
        {
            _jamkit.Play("ButtonClick");

            PlayRoomChangeAnimation(_roomBackgrounLerpTargetLeft, _roomBackgrounLerpTargetRight, _currentRoomIndex + 1);

            _roomNameText.gameObject.SetActive(false);
            _roomCardsRoot.gameObject.SetActive(false);
            _prevRoomButton.gameObject.SetActive(false);
            _nextRoomButton.gameObject.SetActive(false);
            _jamkit.RunDelayed(0.5f, () =>
            {
                _roomNameText.gameObject.SetActive(true);
                _roomCardsRoot.gameObject.SetActive(true);
                _prevRoomButton.gameObject.SetActive(true);
                _nextRoomButton.gameObject.SetActive(true);

                _currentRoomIndex++;
                SetWithRoom(_currentRoomIndex);
            });
        }

        private void PlayRoomChangeAnimation(Transform firstTip, Transform secondTip, int targetRoomIndex)
        {
            Vector3 mid = _roomBackground.transform.position;
            Vector3 src = mid;
            Vector3 target = firstTip.position;
            _roomBackground.CrossFadeAlpha(0, RoomChangeDuration / 2.0f, false);
            _jamkit.TweenDiscrete(AnimationCurve.EaseInOut(0, 0, 1, 1), RoomChangeDuration / 2.0f, _jamkit.Globals.DiscreteTickInterval,
                t =>
                {
                    _roomBackground.transform.position = Vector3.Lerp(src, target, t);
                },
                () =>
                {
                    _roomBackground.sprite = CurrentLevel.Rooms[targetRoomIndex].Background;
                    _roomBackground.CrossFadeAlpha(1, RoomChangeDuration / 2.0f, false);
                    src = secondTip.position;
                    target = mid;
                    _jamkit.TweenDiscrete(AnimationCurve.EaseInOut(0, 0, 1, 1), RoomChangeDuration / 2.0f, _jamkit.Globals.DiscreteTickInterval,
                        t =>
                        {
                            _roomBackground.transform.position = Vector3.Lerp(src, target, t);
                        },
                        () =>
                        {
                            _roomBackground.transform.position = mid;
                        });
                });

        }

        public void OnCardClicked(CardData card)
        {
            if (_isShowingTooHeavy) { return; }

            List<CardData> currentRoomCards = _currentLevelCards[_currentRoomIndex];
            if (currentRoomCards.Exists(x => x == card)) // Taking room card
            {
                bool isTooHeavy = StatusWeight + card.Weight > CurrentLevel.BackpackCapacity;
                if (isTooHeavy)
                {
                    _jamkit.Play("TooHeavy");
                    _tooHeavyText.SetActive(true);
                    _isShowingTooHeavy = true;
                    _jamkit.RunDelayed(0.3f, () =>
                    {
                        _isShowingTooHeavy = false;
                        _tooHeavyText.SetActive(false);
                    });
                    return;
                }

                _jamkit.Play("Card1");

                currentRoomCards.Remove(card);
                _handCards.Add(card);

                CardView clickedCardView = _currentRoomCardViews.Find(x => x.Card == card);
                Debug.Assert(clickedCardView != null);

                _currentRoomCardViews.Remove(clickedCardView);
                _handCardViews.Add(clickedCardView);

                MoveCardView(clickedCardView, _handCardsRoot, _handCardInvisible);
            }
            else // Putting down hand card
            {
                _jamkit.Play("Card2");

                Debug.Assert(_handCards.Exists(x => x == card));
                currentRoomCards.Add(card);
                _handCards.Remove(card);

                CardView clickedCardView = _handCardViews.Find(x => x.Card == card);
                Debug.Assert(clickedCardView != null);

                _currentRoomCardViews.Add(clickedCardView);
                _handCardViews.Remove(clickedCardView);

                MoveCardView(clickedCardView, _roomCardsRoot, _roomCardInvisible);
            }

            SetStatus();
        }

        private void MoveCardView(CardView clickedCardView, Transform targetTransform, GameObject invisibleCard)
        {
            invisibleCard.SetActive(true);
            invisibleCard.transform.SetAsLastSibling();
            clickedCardView.transform.SetParent(_ui.transform);

            _jamkit.RunDelayed(0.001f, () => // Wait for a frame to let layout to correct invisible card's position
            {
                Vector3 src = clickedCardView.transform.position;
                Vector3 target = invisibleCard.transform.position;
                const float TweenDuration = 0.3f;
                _jamkit.Tween(AnimationCurve.EaseInOut(0, 0, 1, 1), TweenDuration,
                    t =>
                    {
                        clickedCardView.transform.position = Vector3.Lerp(src, target, t);
                    },
                    () =>
                    {
                        invisibleCard.SetActive(false);
                        clickedCardView.transform.SetParent(targetTransform);
                    });
            });
        }

        public void OnCardPointerEnter(CardData card)
        {
            //Debug.Log("TODO: highlight card");
        }

        public void OnCardPointerExit(CardData card)
        {
            //Debug.Log("TODO: unhighlight card");
        }

        private void EndGame(GameResultType resultType)
        {
            _isGameRunning = false;
            float duration = _jamkit.Globals.SceneTransitionParams.Duration;
            _jamkit.FadeOutMusic(duration - 0.05f);
            _ui.FadeOut();
            PlayerPrefs.SetInt("ld54_lastresulttype", (int)resultType);
            _jamkit.RunDelayed(duration, () =>
            {
                SceneManager.LoadScene("End");
            });
        }
    }

}