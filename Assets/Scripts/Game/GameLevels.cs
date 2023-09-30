using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "AllLevels", menuName = "Torreng/GameLevels", order = 0)]
    public class GameLevels : ScriptableObject
    {
        [SerializeField] private List<GameLevelData> _levels;
        public List<GameLevelData> Levels => _levels;
    }
}
