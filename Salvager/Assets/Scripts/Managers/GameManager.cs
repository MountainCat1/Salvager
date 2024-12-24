using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Items;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Inject] IPlayerCharacterProvider _playerCharacterProvider;
        [Inject] IPhaseManager _phaseManager;
        [Inject] ISignalManager _signalManager;
        [Inject] IGameDataManager _gameDataManager;
        [Inject] IDialogManager _dialogManager;

        [SerializeField] private List<ItemBehaviour> startingItems;

        [SerializeField] private DialogData alreadyWonDialog;

        private void Start()
        {
            _phaseManager.PhaseChanged += OnTimeRunOut;
            _playerCharacterProvider.Get().Death += OnPlayerDeath;
            _signalManager.Signaled += (signal) =>
            {
                if (signal == Signals.Win)
                    OnWin();
                
                if (signal == Signals.CloseGame)
                    Application.Quit();
            };

            InitializePlayer();

            if (_gameDataManager.GameData.ILiveFinallyInPeace)
            {
                _playerCharacterProvider.Get().BaseSpeed = 0;
                _dialogManager.ShowDialog(alreadyWonDialog);
            }
        }

        private void OnWin()
        {
            _gameDataManager.GameData.ILiveFinallyInPeace = true;
            _gameDataManager.SaveData();
        }

        private void OnPlayerDeath(DeathContext ctx)
        {
            Task.Run(async () =>
            {
                await Task.Delay(5000);
                Application.Quit();
            });
        }

        private void OnTimeRunOut(int phase)
        {
            if (phase != IPhaseManager.EndPhase)
                return;

            Task.Run(async () =>
            {
                await Task.Delay(5000);
                Application.Quit();
            });
        }

        private void InitializePlayer()
        {
            var player = _playerCharacterProvider.Get();

            foreach (var startingItem in startingItems)
            {
                player.Inventory.AddItem(startingItem);
            }

            var startingWeapon = player.Inventory.Items.FirstOrDefault(x => x is Weapon) as Weapon;
            if (!startingWeapon)
            {
                Debug.LogError("No starting weapon found");
                return;
            }

            startingWeapon.Use(new ItemUseContext()
            {
                Creature = player
            });
        }
    }
}