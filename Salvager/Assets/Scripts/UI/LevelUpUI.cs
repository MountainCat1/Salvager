using System;
using Managers;
using UnityEngine;
using Zenject;

namespace UI
{
    public class LevelUpUI : MonoBehaviour
    {
        [Inject] private IPlayerCharacterProvider _playerCharacterProvider;

        private void Start()
        {
            var player = _playerCharacterProvider.Get();
            player.LevelSystem.ChangedLevel += OnPlayerLevelChanged;
            
            gameObject.SetActive(false);
        }

        private void OnPlayerLevelChanged()
        {
            gameObject.SetActive(true);
        }

        public void UpgradeCharacteristic(Characteristics characteristic)
        {
            var player = _playerCharacterProvider.Get();
            
            player.LevelSystem.UpgradeCharacteristic(characteristic);

            if (player.LevelSystem.PointsToUse == 0)
            {
                gameObject.SetActive(false);
            }
        }
        
        public void UpgradeCharacteristic(int characteristic)
        {
            UpgradeCharacteristic((Characteristics) characteristic);
        }
    }
}