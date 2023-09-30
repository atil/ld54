using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "AllLevels", menuName = "Torreng/GameLevels", order = 0)]
    public class GameLevels : ScriptableObject
    {
        [SerializeField] private List<GameLevelData> _levels;
        public List<GameLevelData> Levels => _levels;

        [Header("Intermission texts")]
        [SerializeField, TextArea(2, 10)] private string _levelSuccesText;
        public string LevelSuccessText => _levelSuccesText;

        [SerializeField, TextArea(2, 10)] private string _levelFailUndervalueText;
        public string LevelFailUndervalueText => _levelFailUndervalueText;

        [SerializeField, TextArea(2, 10)] private string _levelFailPoliceText;
        public string LevelFailPoliceText => _levelFailPoliceText;

        [SerializeField, TextArea(2, 10)] private string _gameWinText;
        public string GameWinText => _gameWinText;

    }
}
