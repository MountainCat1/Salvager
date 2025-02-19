using System;
using System.Linq;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

[RequireComponent(typeof(TooltipTrigger))]
public class CrewEntryUI : MonoBehaviour
{
    
    [SerializeField] private Image crewMemberImage;
    [SerializeField] private TextMeshProUGUI crewMemberName;
    [SerializeField] private Toggle selectedToggle;
    [SerializeField] private GameObject noWeaponWarning;

    private CreatureData _crewMember;
    private Action<CreatureData> _onClick;
    private Action<CreatureData, bool> _onToggle;
    
    private TooltipTrigger _tooltipTrigger;

    private void Awake()
    {
        _tooltipTrigger = GetComponent<TooltipTrigger>();
    }

    public void Initialize(CreatureData crewMember, Action<CreatureData> onClick, Action<CreatureData, bool> onToggle)
    {
        // crewMemberImage.sprite = crewMember.Sprite;
        crewMemberName.text = crewMember.Name;
        _crewMember = crewMember;
        
        _onClick = onClick;
        _onToggle = onToggle;
     
        selectedToggle.isOn = crewMember.Selected;
        selectedToggle.onValueChanged.AddListener(OnToggle);
        
        if(crewMember.Inventory.Items.Any(x => x.Type == ItemType.Weapon))
            noWeaponWarning.SetActive(false);
        else
            noWeaponWarning.SetActive(true);
        
        UpdateTooltip();
    }

    private void OnToggle(bool toggle)
    {
        _onToggle?.Invoke(_crewMember, toggle);
    }

    public void Select()
    {
        Debug.Log($"Selected {_crewMember.Name}");
        _onClick?.Invoke(_crewMember);
    }
    
    private void UpdateTooltip()
    {
        _tooltipTrigger.text = $"{_crewMember.Name}\n-" +
                               $"----\n" +
                               $"Aim: ***\n" +
                               $"Endurance: **\n" +
                               $"Software: *\n" +
                               $"Engineering: **\n";
    }
}
