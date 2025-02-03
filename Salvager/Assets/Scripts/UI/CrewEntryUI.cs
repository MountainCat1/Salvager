using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrewEntryUI : MonoBehaviour
{
    [SerializeField] private Image crewMemberImage;
    [SerializeField] private TextMeshProUGUI crewMemberName;

    
    public void Initialize(CreatureData crewMember)
    {
        // crewMemberImage.sprite = crewMember.Sprite;
        crewMemberName.text = crewMember.Name;
    }
}
