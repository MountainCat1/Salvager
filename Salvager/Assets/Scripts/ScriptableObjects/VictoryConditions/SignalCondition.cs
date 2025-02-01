using Managers;
using UnityEngine;
using Zenject;

namespace VictoryConditions
{
    [CreateAssetMenu(fileName = "VictoryCondition", menuName = "Custom/VictoryConditions/Signal")]
    public class SignalCondition : VictoryCondition
    {
        [SerializeField] private Signal signal;
        [SerializeField] private int count = 1;
        
        [Header("Available variables: \n<count>\n<current_count>")]
        [SerializeField] private string description;
        
        [Inject] private ISignalManager _signalManager;

        public override string GetDescription()
        {
            return description
                .Replace("<count>", count.ToString())
                .Replace("<current_count>", _signalManager.GetSignalCount(signal).ToString());
        }

        public override bool Check()
        {
            return _signalManager.GetSignalCount(signal) >= count;
        }
    }
}