using UnityEngine;

namespace Managers
{
    public abstract class VictoryCondition : ScriptableObject
    {
        public abstract string GetDescription();
        public abstract bool Check();
    }
}