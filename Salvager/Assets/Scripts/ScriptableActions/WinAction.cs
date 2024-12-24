using Managers;
using UnityEngine;
using Zenject;

namespace ScriptableActions
{
    public class WinAction : ScriptableAction
    {
        [Inject] private IWinManager _winManager;
        
        public override void Execute()
        {
            base.Execute();

            Debug.Log("Win!");
            
            _winManager.Win();
        }
    }
}