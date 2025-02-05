using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LevelEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelNameText;

        public Level Level => _level;
        
        private Level _level;
        private Action<Level> _selectLevel;
        
        public void Initialize(Level level, Action<Level> selectLevel)
        {
            levelNameText.text = level.Name;
            
            _level = level;
            _selectLevel = selectLevel;
            
            if(level.Type == LevelType.EndNode)
                levelNameText.color = Color.red;
            else if (level.Type == LevelType.StartNode)
                levelNameText.color = Color.green;
        }

        public void SelectLevel()
        {
            _selectLevel?.Invoke(_level);
        }
    }
}