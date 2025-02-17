using Data;
using Managers;
using UnityEngine;
using Zenject;

public class CrewManagerUI : MonoBehaviour
{
    [Inject] private IDataManager _dataManager;
    [Inject] private DiContainer _diContainer;
    [Inject] private ICrewManager _crewManager;

    [SerializeField] private CrewEntryUI crewEntryUI;
    [SerializeField] private Transform crewListParent;

    private void Start()
    {
        _crewManager.Changed += RefreshList;
        if (_crewManager.Crew is not null)
            RefreshList();
    }

    public void ReRollCrew()
    {
        _crewManager.ReRollCrew();
    }

    private void RefreshList()
    {
        foreach (Transform child in crewListParent)
        {
            Destroy(child.gameObject);
        }

        var crew = _crewManager.Crew;

        foreach (var crewMember in crew)
        {
            var crewMemberUI = _diContainer.InstantiatePrefab(crewEntryUI, crewListParent).GetComponent<CrewEntryUI>();
            crewMemberUI.Initialize(crewMember, OnSelect, OnToggle);
        }
    }

    private void OnToggle(CreatureData creature, bool value)
    {
        _crewManager.ToggleCreature(creature, value);
    }
    

    private void OnSelect(CreatureData creature)
    {
        _crewManager.SelectCreature(creature);
    }
}