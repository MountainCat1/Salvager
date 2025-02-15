using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelNameText;
        [SerializeField] private GameObject presenceMarker;

        public LocationData Location => _location;

        private LocationData _location;
        private Action<LocationData> _selectLevel;

        public void Initialize(LocationData location, Action<LocationData> selectLevel, int distanceToCurrent)
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
                presenceMarker.SetActive(true);
            }
            else if(distanceToCurrent == 1)
            {
                levelNameText.fontStyle = FontStyles.Bold;
                presenceMarker.SetActive(false);
            }
            else
            {
                levelNameText.fontStyle = FontStyles.Normal;
                GetComponentInChildren<Button>().targetGraphic.color = new Color(1f, 1f, 1f, 0.5f);
                presenceMarker.SetActive(false);
            }
            
            levelNameText.text = location.Name + $" ({distanceToCurrent})";
        }

        public void SelectLevel()
        {
            _selectLevel?.Invoke(_location);
        }
    }
}