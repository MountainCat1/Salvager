using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "RoomBlueprintProp", menuName = "Custom/RoomBlueprints/RoomBlueprintProp", order = 0)]
public class RoomBlueprintProp : ScriptableObject
{
    [SerializeField] public GameObject prefab = null!;
    [SerializeField] public PropPosition position = PropPosition.Anywhere;
    [SerializeField] public Vector2 offset = Vector2.zero;
    [SerializeField] public int count  = 1;
}