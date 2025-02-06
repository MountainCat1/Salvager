using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrewEntryUI : MonoBehaviour
{
    [SerializeField] private Image crewMemberImage;
    [SerializeField] private TextMeshProUGUI crewMemberName;

    private CreatureData _crewMember;
    private Action<CreatureData> _onClick;
    
    public void Initialize(CreatureData crewMember, Action<CreatureData> onClick)
    {
        // crewMemberImage.sprite = crewMember.Sprite;
        crewMemberName.text = crewMember.Name;
        _crewMember = crewMember;
        
        _onClick = onClick;
    }

    public void Select()
    {
        Debug.Log($"Selected {_crewMember.Name}");
        _onClick?.Invoke(_crewMember);
    }
}
