using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LevelEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelNameText;

        private Level _level;
        private Action<Level> _selectLevel;
        
        public void Initialize(Level level, Action<Level> selectLevel)
        {
            levelNameText.text = level.Name;
            
            _level = level;
            _selectLevel = selectLevel;
        }

        public void SelectLevel()
        {
            _selectLevel?.Invoke(_level);
        }
    }
}