

using System.Collections.Generic;
using UnityEngine;

public enum PropPosition
{
    Anywhere,
    Center,
    Corner,
    Edge
}

[System.Serializable]
[CreateAssetMenu(fileName = "RoomBlueprint", menuName = "Custom/RoomBlueprints/RoomBlueprint", order = 0)]
public class RoomBlueprint : ScriptableObject
{
    [field: SerializeField] public string Name { get; set; } = null!;
    [field: SerializeField] public List<RoomBlueprintProp> Props { get; set; } = new();
    [field: SerializeField] public bool StartingRoom { get; set; } = false;
    [field: SerializeField] public Creature[] Enemies { get; set; }
}