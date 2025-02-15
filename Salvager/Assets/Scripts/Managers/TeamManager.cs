using System.Collections.Generic;
using UnityEngine;

public enum Teams
{
    Passive,
    Player,
    Undead,
    Aliens,
    Ai,
    Bandits,
}

public enum Attitude
{
    Friendly,
    Neutral,
    Hostile
}

namespace Managers
{
    interface ITeamManager
    {
        Attitude GetAttitude(Teams team1, Teams team2);
    }
    
    public class TeamManager : MonoBehaviour, ITeamManager
    {
        // Public Constants

        // Static Variables and Methods

        // Public Variables

        // Serialized Private Variables

        // Injected Dependencies (using Zenject)

        // Private Variables
        private readonly Dictionary<Teams, Dictionary<Teams, Attitude>> _relations = new();
        
        // Properties

        // Events

        // Unity Callbacks
        private void Start()
        {
            // First set all relations to neutral
            foreach (Teams team1 in System.Enum.GetValues(typeof(Teams)))
            {
                _relations[team1] = new Dictionary<Teams, Attitude>();
                foreach (Teams team2 in System.Enum.GetValues(typeof(Teams)))
                {
                    _relations[team1][team2] = Attitude.Neutral;
                }
            }

            // Then set all relations to friendly
            foreach (Teams team in System.Enum.GetValues(typeof(Teams)))
            {
                _relations[team][team] = Attitude.Friendly;
            }
            
            // Player
            AddRelation(Teams.Player, Teams.Ai, Attitude.Hostile);
            AddRelation(Teams.Player, Teams.Bandits, Attitude.Hostile);
            AddRelation(Teams.Player, Teams.Undead, Attitude.Hostile);
            AddRelation(Teams.Player, Teams.Aliens, Attitude.Hostile);
            
            // Ai
            AddRelation(Teams.Ai, Teams.Bandits, Attitude.Hostile);
            AddRelation(Teams.Ai, Teams.Undead, Attitude.Hostile);
            AddRelation(Teams.Ai, Teams.Aliens, Attitude.Hostile);
            
            // Bandits
            AddRelation(Teams.Bandits, Teams.Undead, Attitude.Hostile);
            AddRelation(Teams.Bandits, Teams.Aliens, Attitude.Hostile);
            
            // Undead
            AddRelation(Teams.Undead, Teams.Aliens, Attitude.Hostile);
            
            // Aliens
            
        }


        // Public Methods
        public Attitude GetAttitude(Teams team1, Teams team2)
        {
            return _relations[team1][team2];
        }
        
        // Virtual Methods

        // Abstract Methods

        // Private Methods
        private void AddRelation(Teams team1, Teams team2, Attitude attitude)
        {
            _relations[team1][team2] = attitude;
            _relations[team2][team1] = attitude;
        }
        
        // Event Handlers
    }
}