using System;
using System.Collections.Generic;
using Constants;
using Data;
using TMPro;
using UnityEngine;
using Utilities;
using Zenject;

public class CrewManagerUI : MonoBehaviour
{
    [Inject] private IDataManager _dataManager;
    [Inject] private DiContainer _diContainer;

    [SerializeField] private CrewEntryUI crewEntryUI;

    [SerializeField] private Transform crewListParent;

    private void Start()
    {
        RefreshList();
    }

    public void ReRollCrew()
    {
        const int crewCount = 5;
        var crew = new List<CreatureData>();
        for (int i = 0; i < crewCount; i++)
        {
            crew.Add(GenerateCrew());
        }

        _dataManager.SaveData(new GameData()
        {
            Creatures = crew
        });

        RefreshList();
    }

    void RefreshList()
    {
        foreach (Transform child in crewListParent)
        {
            Destroy(child.gameObject);
        }

        var crew = _dataManager.LoadData();

        foreach (var crewMember in crew)
        {
            var crewMemberUI = _diContainer.InstantiatePrefab(crewEntryUI, crewListParent).GetComponent<CrewEntryUI>();
            crewMemberUI.GetComponentInChildren<TextMeshProUGUI>().text = crewMember.Name;
        }
    }

    private CreatureData GenerateCrew()
    {
        return new CreatureData()
        {
            Name = $"{Names.Human.RandomElement()} {Surnames.Human.RandomElement()}",
            InteractionRange = 1.5f,
            SightRange = 5f,
            Inventory = new InventoryData()
            {
                Items = new List<ItemData>()
                {
                    new ItemData()
                    {
                        Identifier = "Gun",
                        Count = 1
                    }
                }
            },
            Team = Teams.Player,
            State = CreatureState.Idle,
            CreatureID = Guid.NewGuid().ToString(),
            XpAmount = 0
        };
    }
}