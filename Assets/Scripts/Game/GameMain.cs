using JamKit;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameMain : MonoBehaviour
    {
        [SerializeField] private JamKit.JamKit _jamkit;
        [SerializeField] private GameUi _ui;
        [SerializeField] private GameLevels _allLevels;

        [Header("UI")]
        [SerializeField] private GameObject _cardPrefab;
        [SerializeField] private TextMeshProUGUI _roomNameText;
        [SerializeField] private Transform _roomCardsRoot;
        [SerializeField] private Transform _handCardsRoot;
        [SerializeField] private Button _prevRoomButton;
        [SerializeField] private Button _nextRoomButton;
        [SerializeField] private TextMeshProUGUI _prevRoomText;
        [SerializeField] private TextMeshProUGUI _nextRoomText;
        [SerializeField] private Slider _timerSlider;
        [Header("Status")]
        [SerializeField] private TextMeshProUGUI _weightCapacityText;
        [SerializeField] private TextMeshProUGUI _weightCurrentText;
        [SerializeField] private Slider _weightSlider;
        [SerializeField] private TextMeshProUGUI _moneyGoalText;
        [SerializeField] private TextMeshProUGUI _moneyCurrentText;
        [SerializeField] private Slider _moneySlider;

        private int _currentRoomIndex = 0;
        private float _timer = 0;
        private List<CardView> _currentRoomCardViews = new();

        private List<CardData> _handCards = new();
        private List<CardView> _handCardViews = new();

        private const int CurrentLevelIndex = 0; // TODO: Initialize from playerprefs
        private GameLevelData CurrentLevel => _allLevels.Levels[CurrentLevelIndex];
        private IReadOnlyList<Room> CurrentRooms => CurrentLevel.Rooms;

        List<List<CardData>> _currentLevelCards = new();

        private void Start()
        {
            foreach (Room room in CurrentRooms)
            {
                _currentLevelCards.Add(new List<CardData>(room.Cards));
            }

            _weightCapacityText.text = $"{CurrentLevel.BackpackCapacity}kg";
            _moneyGoalText.text = $"{CurrentLevel.MoneyGoal}$";

            SetWithRoom(_currentRoomIndex);
            SetStatus();
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            _timerSlider.value = (CurrentLevel.TimeLimit - _timer) / CurrentLevel.TimeLimit;
        }

        private void SetStatus()
        {
            int newWeight = _handCards.Sum(x => x.Weight);
            _weightCurrentText.text = $"{newWeight}kg";
            _weightSlider.value = (float)newWeight / (float)CurrentLevel.BackpackCapacity;

            int newMoney = _handCards.Sum(x => x.Money);
            _moneyCurrentText.text = $"{newMoney}$";
            _moneySlider.value = (float)newMoney / (float)CurrentLevel.MoneyGoal;
        }

        private void SetWithRoom(int roomIndex)
        {
            Room newRoom = CurrentRooms[roomIndex];
            _roomNameText.text = newRoom.Name;
            _prevRoomText.text = _currentRoomIndex == 0 ? "Exit" : CurrentRooms[_currentRoomIndex - 1].Name;
            _nextRoomText.text = _currentRoomIndex == CurrentRooms.Count - 1 ? "" : CurrentRooms[_currentRoomIndex + 1].Name;
            foreach (Transform t in _roomCardsRoot)
            {
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

            _nextRoomButton.gameObject.SetActive(_currentRoomIndex != CurrentRooms.Count - 1);
        }

        public void OnPrevRoomClicked()
        {
            if (_currentRoomIndex == 0)
            {
                Debug.Log("TODO: Exit");
                return;
            }

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

                _currentRoomIndex--;
                SetWithRoom(_currentRoomIndex);
            });
        }

        public void OnNextRoomClicked()
        {
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

        public void OnCardClicked(CardData card)
        {
            List<CardData> currentRoomCards = _currentLevelCards[_currentRoomIndex];
            if (currentRoomCards.Exists(x => x == card)) // Room card
            {
                currentRoomCards.Remove(card);
                _handCards.Add(card);

                CardView clickedCardView = _currentRoomCardViews.Find(x => x.Card == card);
                Debug.Assert(clickedCardView != null);

                _currentRoomCardViews.Remove(clickedCardView);
                _handCardViews.Add(clickedCardView);
                clickedCardView.transform.SetParent(_handCardsRoot);
            }
            else
            {
                Debug.Assert(_handCards.Exists(x => x == card));
                currentRoomCards.Add(card);
                _handCards.Remove(card);

                CardView clickedCardView = _handCardViews.Find(x => x.Card == card);
                Debug.Assert(clickedCardView != null);

                _currentRoomCardViews.Add(clickedCardView);
                _handCardViews.Remove(clickedCardView);

                clickedCardView.transform.SetParent(_roomCardsRoot);
            }

            SetStatus();
        }

        public void OnCardPointerEnter(CardData card)
        {
            //Debug.Log("TODO: highlight card");
        }

        public void OnCardPointerExit(CardData card)
        {
            //Debug.Log("TODO: unhighlight card");
        }
    }
}