using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SelectionDisplayEntryUI : MonoBehaviour
{
    [Inject] private IPopupManagerUI _popupManagerUI;
    
    [SerializeField] private TextMeshProUGUI creatureNameText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image creatureImage;
    
    private Creature _creature;
    
    public void SetCreature(Creature creature)
    {
        _creature = creature;
        
        creatureNameText.text = creature.name;
        healthSlider.maxValue = creature.Health.MaxValue;
        healthSlider.value = creature.Health.CurrentValue;
        _creature.Health.ValueChanged += OnHealthChanged;
        creatureImage.sprite = creature.GetComponentInChildren<SpriteRenderer>().sprite;
    }

    private void OnHealthChanged()
    {
        if(!_creature)
        {
            return;
        }
        
        healthSlider.value = _creature.Health.CurrentValue;
    }
    
    public void ShowInventory()
    {
        _popupManagerUI.ShowInventory(_creature);
    }
}
