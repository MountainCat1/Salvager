using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrewEntryUI : MonoBehaviour
{
    [SerializeField] private Image crewMemberImage;
    [SerializeField] private TextMeshProUGUI crewMemberName;
    [SerializeField] private Toggle selectedToggle;

    private CreatureData _crewMember;
    private Action<CreatureData> _onClick;
    private Action<CreatureData, bool> _onToggle;
    
    public void Initialize(CreatureData crewMember, Action<CreatureData> onClick, Action<CreatureData, bool> onToggle)
    {
        // crewMemberImage.sprite = crewMember.Sprite;
        crewMemberName.text = crewMember.Name;
        _crewMember = crewMember;
        
        _onClick = onClick;
        _onToggle = onToggle;
     
        selectedToggle.isOn = crewMember.Selected;
        selectedToggle.onValueChanged.AddListener(OnToggle);
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
}
