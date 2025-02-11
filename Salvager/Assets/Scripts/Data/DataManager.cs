using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Managers;
using Zenject;

namespace Data
{
    public interface IDataManager
    {
        void SaveData();
        void SaveData(GameData gameData);
        GameData LoadData();
        GameData GetData();
        void DeleteData();
    }

    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WriteEndObject();
        }

        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = serializer.Deserialize<Dictionary<string, float>>(reader);
            return new Vector2(obj["x"], obj["y"]);
        }
    }
    
    public class Vector2IntConverter : JsonConverter<Vector2Int>
    {
        public override void WriteJson(JsonWriter writer, Vector2Int value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WriteEndObject();
        }

        public override Vector2Int ReadJson(JsonReader reader, Type objectType, Vector2Int existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = serializer.Deserialize<Dictionary<string, int>>(reader);
            return new Vector2Int(obj["x"], obj["y"]);
        }
    }


    public class DataManager : IDataManager
    {
        private static readonly string SaveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        private GameData _gameData;

        public void SaveData()
        {
            Debug.Log("Saving data...");
            SaveData(_gameData);
        }

        public void SaveData(GameData gameData)
        {
            try
            {
                // Serialize with Type Handling and Vector2 support
                string json = JsonConvert.SerializeObject(gameData, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Converters = new List<JsonConverter> { new Vector2Converter(), new Vector2IntConverter() } // Add Vector2 support
                });

                Debug.Log($"Saving game data to: {SaveFilePath}\n{json}");

                File.WriteAllText(SaveFilePath, json);
                Debug.Log("Game data saved successfully.");

                _gameData = gameData;
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to save data: {e.Message}");
            }
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

                // Deserialize with Type Handling and Vector2 support
                GameData gameData;


                try
                {
                    gameData = JsonConvert.DeserializeObject<GameData>(json, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        Converters = new List<JsonConverter> { new Vector2Converter() } // Add Vector2 support
                    });
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load data: {e.Message}");
                    return null;
                }
                

                // Load sprites for items
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

        public GameData GetData()
        {
            return _gameData ?? throw new Exception("No game data loaded!");
        }

        public void DeleteData()
        {
            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
                Debug.Log("Save file deleted.");
            }
            else
            {
                Debug.LogWarning("Save file not found! Skipping delete.");
            }
        }
    }
}