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
    }

    public class Vector2Converter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Vector2 v = (Vector2)value;
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(v.x);
            writer.WritePropertyName("y");
            writer.WriteValue(v.y);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var obj = serializer.Deserialize<Dictionary<string, float>>(reader);
            return new Vector2(obj["x"], obj["y"]);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector2);
        }
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
            try
            {
                // Serialize with Type Handling and Vector2 support
                string json = JsonConvert.SerializeObject(gameData, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Converters = new List<JsonConverter> { new Vector2Converter() } // Add Vector2 support
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
    }
}