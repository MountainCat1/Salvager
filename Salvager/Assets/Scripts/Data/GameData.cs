using System;
using System.Collections.Generic;
// ReSharper disable InconsistentNaming

[Serializable]
public class GameData
{
    public List<CreatureData> Creatures;
}


[Serializable]
public class CreatureData
{
    public string CreatureID;
    public string Name;
    public CreatureState State;
    public int XpAmount;
    public float SightRange;
    public Teams Team;
    public float InteractionRange;
    public InventoryData Inventory;

    public static CreatureData FromCreature(Creature creature)
    {
        return new CreatureData
        {
            CreatureID = creature.GetInstanceID().ToString(),
            Name = creature.name,
            State = creature.State,
            XpAmount = creature.XpAmount,
            SightRange = creature.SightRange,
            Team = creature.Team,
            InteractionRange = creature.InteractionRange,
            Inventory = InventoryData.FromInventory(creature.Inventory)
        };
    }
}

[Serializable]
public class InventoryData
{
    public List<ItemData> Items;

    public static InventoryData FromInventory(Inventory inventory)
    {
        var data = new InventoryData { Items = new List<ItemData>() };
        foreach (var item in inventory.Items)
        {
            data.Items.Add(new ItemData
            {
                Identifier = item.GetIdentifier(),
                Count = item.Count,
            });
        }

        return data;
    }
}

[Serializable]
public class ItemData
{
    public string Identifier;
    public int Count;
}