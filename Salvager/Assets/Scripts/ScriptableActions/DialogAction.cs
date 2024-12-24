using Managers;
using UnityEngine;
using Zenject;

namespace ScriptableActions
{
    public class DialogAction : ScriptableAction
    {
        [Inject] private IDialogManager _dialogManager;

        [SerializeField] private DialogData dialogData;

        public override void Execute()
        {
            _dialogManager.ShowDialog(dialogData);
        }
    }
}