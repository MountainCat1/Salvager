using System;
using Managers;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class JuiceDisplayUI : MonoBehaviour
    {
        [Inject] IJuiceManager _juiceManager;
        
        [SerializeField] private TextMeshProUGUI juiceText;

        [SerializeField] private Color okColor = Color.green;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color alertColor = Color.red;
        [SerializeField] private float warningAmount = 20f;
        [SerializeField] private float alertAmount = 5f;
        
        private void Start()
        {
            _juiceManager.JuiceChanged += OnJuiceChanged;
            OnJuiceChanged();
        }

        private void OnJuiceChanged()
        {
            juiceText.text = $"{_juiceManager.Juice:F2}";
            
            if ((float)_juiceManager.Juice / _juiceManager.ConsumptionRate > warningAmount)
            {
                juiceText.color = okColor;
            }
            else if ((float)_juiceManager.Juice / _juiceManager.ConsumptionRate > alertAmount)
            {
                juiceText.color = warningColor;
            }
            else
            {
                juiceText.color = alertColor;
            }
        }
    }
}