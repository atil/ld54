using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class CardData
    {
        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private Sprite _visual;
        public Sprite Visual => _visual;

        [SerializeField] private int _weight;
        public int Weight => _weight;

        [SerializeField] private int _money;
        public int Money => _money;
    }

    [System.Serializable]
    public class Room
    {
        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private Sprite _background;
        public Sprite Background => _background;

        [SerializeField] private List<CardData> _cards;
        public IReadOnlyList<CardData> Cards => _cards;
    }

    [CreateAssetMenu(fileName = "SomeLevelData", menuName = "Torreng/GameLevelData", order = 0)]
    public class GameLevelData : ScriptableObject
    {
        [SerializeField] private string _levelName;
        public string LevelName => _levelName;

        [SerializeField] private int _backpackCapacity;
        public int BackpackCapacity => _backpackCapacity;

        [SerializeField] private int _moneyGoal;
        public int MoneyGoal => _moneyGoal;

        [SerializeField] private int _timeLimit;
        public int TimeLimit => _timeLimit;

        [SerializeField] private List<Room> _rooms;
        public IReadOnlyList<Room> Rooms => _rooms;
    }
}