using Items;
using ScriptableActions;
using UnityEngine;
using Zenject;

public class SpawnPickupAction : ScriptableAction
{
    [SerializeField] private PickupAction pickupPrefab;
    [SerializeField] private Transform positionTransform;
    [SerializeField] private ItemBehaviour item;

    [Inject] private DiContainer _container;

    public override void Execute()
    {
        base.Execute();

        var pickupGo = _container.InstantiatePrefab(pickupPrefab, positionTransform.position, positionTransform.rotation, null);
        var pickup = pickupGo.GetComponent<PickupAction>();

        pickup.SetItem(item);
    }
}