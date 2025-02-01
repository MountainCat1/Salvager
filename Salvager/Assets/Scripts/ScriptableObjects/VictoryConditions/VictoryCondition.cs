using UnityEngine;

namespace VictoryConditions
{
    public abstract class VictoryCondition : ScriptableObject
    {
        public abstract string GetDescription();
        public abstract bool Check();
    }
}