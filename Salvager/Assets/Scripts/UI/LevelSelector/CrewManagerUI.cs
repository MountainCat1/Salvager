using Data;
using Managers;
using TMPro;
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

        var gameData = _dataManager.LoadData();

        foreach (var crewMember in gameData.Creatures)
        {
            var crewMemberUI = _diContainer.InstantiatePrefab(crewEntryUI, crewListParent).GetComponent<CrewEntryUI>();
            crewMemberUI.GetComponentInChildren<TextMeshProUGUI>().text = crewMember.Name;
        }
    }

}