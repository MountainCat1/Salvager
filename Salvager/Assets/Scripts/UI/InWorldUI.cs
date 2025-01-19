using System;
using System.Linq;
using System.Net.NetworkInformation;
using Managers;
using UnityEngine;
using Zenject;

namespace UI
{
    
    /// <summary>
    /// This script is used for UI elements which are supposed to be hidden when not visible from the perspective of player units.
    /// </summary>
    public class InWorldUI : MonoBehaviour
    {
        [Inject] private ITeamManager _teamManager;
        [Inject] private ICreatureManager _creatureManager;
        
        
        
        [SerializeField] private Creature hostCreature;
        [SerializeField] private GameObject toggableUI;
        
        private void Update()
        {
            // TODO: optimize this
            var playerUnits = _creatureManager.GetCreatures().Where(x => x.Team == Teams.Player).ToList();
            
            if(playerUnits.Any(x => x.Controller.CanSee(hostCreature)))
            {
                toggableUI.SetActive(true);
            }
            else
            {
                toggableUI.SetActive(false);
            }
        }
    }
}