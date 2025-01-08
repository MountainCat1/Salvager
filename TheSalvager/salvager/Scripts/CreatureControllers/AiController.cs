using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using Items;
using Managers;
using Services;

public partial class AiController : CreatureController
{
    // Events
    public override void _Ready()
    {
        base._Ready();
        
        Creature.Hit += OnHit;
    }
    
    [Inject] protected ICreatureManager CreatureManager = null!;
    [Inject] protected ITeamManager TeamManager = null!;
    
    private const double MemoryTime = 20;

    private Dictionary<Creature?, DateTime> _memorizedCreatures = new();

    public override void _PhysicsProcess(double delta)
    {
        UpdateMemory();
    }

    public ICollection<Creature> GetMemorizedCreatures()
    {
        return _memorizedCreatures
            .Select(x => x.Key)
            .Where(x => x != null)
            .Cast<Creature>()
            .ToList();
    }
    
    private void UpdateMemory()
    {
        var keys = _memorizedCreatures.Keys.ToList();
        foreach (var key in keys)
        {
            if ((DateTime.Now - _memorizedCreatures[key]).TotalSeconds > MemoryTime)
                _memorizedCreatures.Remove(key);
            
            if (key == null)
                _memorizedCreatures.Remove(key);
        }

        CreatureManager.AllCreatures
            .Where(CanSee)
            .ToList()
            .ForEach(Memorize);
    }

    private void Memorize(Creature creature)
    {
        _memorizedCreatures[creature] = DateTime.Now;
    }

    // Event Handlers
    private void OnHit(HitContext ctx)
    {
        Memorize(ctx.Attacker);
    }

    // Helper Methods

    protected Creature GetNewTarget()
    {
        var targets = GetMemorizedCreatures()
            .Where(x => TeamManager.GetAttitude(Creature, x) == Attitude.Hostile)
            .Where(x => CanSee(x))
            .ToList();
    
        // Get closest target
        var target = targets
            .OrderBy(x => Creature.Position.DistanceTo(x.Position))
            .FirstOrDefault();
    
        return target;
    }

    protected bool IsInRange(Creature creature, float range)
    {
        return Creature.Position.DistanceTo(creature.Position) < range;
    }
}