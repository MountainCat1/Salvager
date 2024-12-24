using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Creature creature;

        private void Start()
        {
            creature.Health.ValueChanged += OnHealthChanged;
            OnHealthChanged();
        }

        private void OnHealthChanged()
        {
            slider.maxValue = creature.Health.MaxValue;
            slider.value = creature.Health.CurrentValue;
        }
    }
}