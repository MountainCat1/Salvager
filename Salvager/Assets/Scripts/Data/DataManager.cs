using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

namespace Data
{
    
    public interface IDataManager
    {
        void SaveData();
        void SaveData(GameData gameData);
        ICollection<CreatureData> LoadData();
    }

    public class DataManager : IDataManager
    {
        private static readonly string SaveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        
        public void SaveData()
        {
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
            string json = JsonUtility.ToJson(gameData, prettyPrint: true);
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

        public ICollection<CreatureData> LoadData()
        {
            if (!File.Exists(SaveFilePath))
            {
                Debug.LogWarning("Save file not found! Skipping load.");
                return null;
            }

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                var creatureListWrapper = JsonUtility.FromJson<GameData>(json);

                if (creatureListWrapper?.Creatures == null || creatureListWrapper.Creatures.Count == 0)
                {
                    Debug.LogWarning("No saved data found.");
                    return null;
                }
                
                Debug.Log("Game data loaded successfully.");
                
                return creatureListWrapper.Creatures;
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to load data: {e.Message}");
            }
            
            return null;
        }
    }
}
