using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using Managers;
using Zenject;
using Object = UnityEngine.Object;

namespace Data
{
    
    public interface IDataManager
    {
        void SaveData();
        void SaveData(GameData gameData);
        GameData LoadData();
    }

    public class DataManager : IDataManager
    {
        [Inject] private IItemManager _itemManager;
        
        private static readonly string SaveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        
        private GameData _gameData;
        
        public void SaveData()
        {
            Debug.Log("Saving data...");
            
            SaveData(_gameData);
        }

        public void SaveData(GameData gameData)
        {
            string json = JsonUtility.ToJson(gameData);
            Debug.Log($"Saving game data to: {SaveFilePath}\n{json}");

            try
            {
                File.WriteAllText(SaveFilePath, json);
                Debug.Log("Game data saved successfully.");
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to save data: {e.Message}");
            }
            
            _gameData = gameData;
        }

        public GameData LoadData()
        {
            _gameData = null;
            
            if (!File.Exists(SaveFilePath))
            {
                Debug.LogWarning("Save file not found! Skipping load.");
                return null;
            }

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                var gameData = JsonUtility.FromJson<GameData>(json);
                
                // Load sprites for items
                var inventories = new List<InventoryData> {gameData.Inventory};
                inventories.AddRange(gameData.Creatures.Select(c => c.Inventory));
                foreach (var inventory in inventories)
                {
                    foreach (var item in inventory.Items)
                    {
                        var itemPrefab = _itemManager.GetItemPrefab(item.Identifier);
                        if (itemPrefab == null)
                        {
                            Debug.LogError($"Item prefab not found for {item.Identifier}");
                            continue;
                        }
                        
                        item.Icon = itemPrefab.Icon;
                    }
                }
                
                
                
                
                Debug.Log("Game data loaded successfully.");
                
                _gameData = gameData;
                
                return gameData;
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to load data: {e.Message}");
            }
            
            return null;
        }
    }
}
