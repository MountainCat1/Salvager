

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
    [field: SerializeField] public int Count { get; set; } = 1;
}

[System.Serializable]
[CreateAssetMenu(fileName = "RoomBlueprintProp", menuName = "Custom/RoomBlueprints/RoomBlueprintProp", order = 0)]
public class RoomBlueprintProp : ScriptableObject
{
    [SerializeField] public GameObject prefab = null!;
    [SerializeField] public PropPosition position = PropPosition.Anywhere;
    [SerializeField] public Vector2 offset = Vector2.zero;
    [SerializeField] public int count  = 1;
}