using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
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
        private static readonly string SaveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        
        public void SaveData()
        {
            throw new NotImplementedException("This method is not to be used please use SaveData(GameData gameData) instead.");
            
            Debug.Log("Saving data...");

            var playerUnits = Object.FindObjectsOfType<Creature>()
                .Where(c => c.Team == Teams.Player)
                .Select(CreatureData.FromCreature)
                .ToList();

            if (playerUnits.Count == 0)
            {
                Debug.LogWarning("No player units found to save.");
                return;
            }
            
            SaveData(new GameData() { Creatures = playerUnits });
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
        }

        public GameData LoadData()
        {
            if (!File.Exists(SaveFilePath))
            {
                Debug.LogWarning("Save file not found! Skipping load.");
                return null;
            }

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                var gameData = JsonUtility.FromJson<GameData>(json);
                
                Debug.Log("Game data loaded successfully.");
                
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
