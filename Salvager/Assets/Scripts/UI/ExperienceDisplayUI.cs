using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class ExperienceDisplayUI : MonoBehaviour
    {
        [Inject] IPlayerCharacterProvider _playerProvider;

        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI levelText;

        private void Start()
        {
            var player = _playerProvider.Get();
            player.LevelSystem.ChangedXp += OnChangedXp;

            OnChangedXp();
        }

        private void OnChangedXp()
        {
            var player = _playerProvider.Get();
            slider.value = player.LevelSystem.LevelProgress;
            levelText.text = player.LevelSystem.Level.ToString();
        }
    }
}