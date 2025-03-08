using Managers;
using UnityEngine;
using Zenject;

namespace UI
{
    public class InGameResourcesDisplayUI : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI moneyText;
        [SerializeField] private TMPro.TextMeshProUGUI fuelText;
        [SerializeField] private TMPro.TextMeshProUGUI juiceText;

        [Inject] private ICrewManager _crewManager;

        private void Start()
        {
            _crewManager.Changed += UpdateResources;

            if (_crewManager.Resources != null)
                UpdateResources();
        }

        private void UpdateResources()
        {
            moneyText.text = $"{_crewManager.Resources.Money:F2}$";
            fuelText.text = $"Fuel: {_crewManager.Resources.Fuel:0}u";
            juiceText.text = $"Juice: {_crewManager.Resources.Juice:0}u";
        }
    }
}