using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelNameText;
        [SerializeField] private GameObject selectionMarker;

        public Location Location => _location;

        private Location _location;
        private Action<Location> _selectLevel;

        public void Initialize(Location location, Action<Location> selectLevel, int distanceToCurrent)
        {
            levelNameText.text = location.Name;

            _location = location;
            _selectLevel = selectLevel;

            if (location.Type == LevelType.EndNode)
            {
                levelNameText.color = Color.red;
            }
            else if (location.Visited)
            {
                levelNameText.color = Color.gray;
            }

            if (distanceToCurrent == 0)
            {
                levelNameText.fontStyle = FontStyles.Bold | FontStyles.Underline | FontStyles.UpperCase;
                selectionMarker.SetActive(true);
            }
            else if(distanceToCurrent == 1)
            {
                levelNameText.fontStyle = FontStyles.Bold;
                selectionMarker.SetActive(false);
            }
            else
            {
                levelNameText.fontStyle = FontStyles.Normal;
                GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, 0.5f);
                selectionMarker.SetActive(false);
            }
            
            levelNameText.text = location.Name + $" ({distanceToCurrent})";
        }

        public void SelectLevel()
        {
            _selectLevel?.Invoke(_location);
        }
    }
}