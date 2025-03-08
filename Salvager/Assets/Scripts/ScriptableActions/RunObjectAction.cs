using System.Linq;
using UnityEngine;

namespace ScriptableActions
{
    public class RunObjectAction : ScriptableAction
    {
        [SerializeField] private GameObject objectToRun;
        
        public override void Execute()
        {
            base.Execute();
            
            var actions = objectToRun.GetComponents<ScriptableAction>();
            
            foreach (var action in actions.Where(x => x != this))
            {
                action.Execute();
            }
        }
    }
}