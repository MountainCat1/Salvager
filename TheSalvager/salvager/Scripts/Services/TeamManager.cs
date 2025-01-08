using System.Collections.Generic;
using Godot;

namespace Services;

public enum Teams
{
    Passive,
    Player,
    Kingdom,
    Villagers,
    Kobolds,
    Bandits,
    Cultists
}

public enum Attitude
{
    Friendly,
    Neutral,
    Hostile
}

public interface ITeamManager
{
    Attitude GetAttitude(Teams team1, Teams team2);
    Attitude GetAttitude(Creature creature1, Creature creature2);
}

public partial class TeamManager : Node2D, ITeamManager
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
    public override void _Ready()
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

        // Player
        // Base

        // Villagers
        AddRelation(Teams.Villagers, Teams.Player, Attitude.Friendly);
        AddRelation(Teams.Villagers, Teams.Kingdom, Attitude.Friendly);

        // Bandits
        AddRelation(Teams.Bandits, Teams.Player, Attitude.Hostile);
        AddRelation(Teams.Bandits, Teams.Kingdom, Attitude.Hostile);
        AddRelation(Teams.Bandits, Teams.Villagers, Attitude.Hostile);

        // Kobolds
        AddRelation(Teams.Kobolds, Teams.Player, Attitude.Hostile);
        AddRelation(Teams.Kobolds, Teams.Kingdom, Attitude.Hostile);
        AddRelation(Teams.Kobolds, Teams.Villagers, Attitude.Hostile);
        AddRelation(Teams.Kobolds, Teams.Bandits, Attitude.Hostile);

        // Cultists
        AddRelation(Teams.Cultists, Teams.Player, Attitude.Hostile);
        AddRelation(Teams.Cultists, Teams.Kingdom, Attitude.Hostile);
        AddRelation(Teams.Cultists, Teams.Villagers, Attitude.Hostile);
        AddRelation(Teams.Cultists, Teams.Bandits, Attitude.Hostile);
        AddRelation(Teams.Cultists, Teams.Kobolds, Attitude.Hostile);
    }


    // Public Methods
    public Attitude GetAttitude(Teams team1, Teams team2)
    {
        return _relations[team1][team2];
    }

    public Attitude GetAttitude(Creature creature1, Creature creature2)
    {
        return GetAttitude(creature1.Team, creature2.Team);
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