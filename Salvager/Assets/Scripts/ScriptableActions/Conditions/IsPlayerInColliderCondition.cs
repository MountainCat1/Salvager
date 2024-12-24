using DefaultNamespace;
using UnityEngine;

namespace ScriptableActions.Conditions
{
    public class IsPlayerInColliderCondition : ConditionBase
    {
        [SerializeField] private PlayerDetector playerDetector;
        
        protected override bool Check()
        {
            return playerDetector.PlayerInside;
        }
    }
}