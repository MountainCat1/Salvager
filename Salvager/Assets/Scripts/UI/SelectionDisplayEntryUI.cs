using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionDisplayEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI creatureNameText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image creatureImage;
    
    private Creature _creature;
    
    public void SetCreature(Creature creature)
    {
        creatureNameText.text = creature.name;
        healthSlider.maxValue = creature.Health.MaxValue;
        healthSlider.value = creature.Health.CurrentValue;
        creature.Health.ValueChanged += OnHealthChanged;
        creatureImage.sprite = creature.GetComponentInChildren<SpriteRenderer>().sprite;
    }

    private void OnHealthChanged()
    {
        if(_creature == null)
        {
            return;
        }
        
        healthSlider.value = _creature.Health.CurrentValue;
    }
}
