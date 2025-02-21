using Data;
using UnityEngine;

namespace Items
{
    public class MiscItem : ItemBehaviour
    {
        [field: SerializeField] public override ItemData ItemData { get; protected set; }
    }
}