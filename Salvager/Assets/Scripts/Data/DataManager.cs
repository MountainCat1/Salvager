using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Managers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using Zenject;

namespace Data
{
    public interface IDataManager
    {
        void SaveData();
        GameData LoadData();
        GameData GetData();
        void DeleteData();
        void SetPrefabs(GameData data);
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
        [Inject] private IItemManager _itemManager;
         
        private static readonly string SaveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        private GameData _gameData;

        private JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = new CustomSerializationBinder(),
            Converters = new List<JsonConverter>
            {
                new Vector2Converter(),
                new Vector2IntConverter(), 
                new StringEnumConverter()
            } 
        };
        
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
                string json = JsonConvert.SerializeObject(gameData, Formatting.Indented, _serializerSettings);

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
                    gameData = JsonConvert.DeserializeObject<GameData>(json, _serializerSettings);
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
            if(_gameData is not null) return _gameData;

            Debug.Log("Creating new game data...");

            _gameData = new GameData();
            
            return _gameData;
        }

        // TODO: This should be in a separate manager
        public void SetPrefabs(GameData data)
        {
            var crewItems = data.Creatures.SelectMany(x => x.Inventory.Items).ToList();
            var inventoryItems = data.Inventory.Items;
            var shopItems = data.Region.Locations.Select(x => x.ShopData).Where(x => x is not null).SelectMany(x => x.inventory.Items);

            foreach (var item in crewItems)
            {
                item.Prefab = _itemManager.GetItemPrefab(item.Identifier);
            }
            
            foreach (var item in inventoryItems)
            {
                item.Prefab = _itemManager.GetItemPrefab(item.Identifier);
            }

            foreach (var item in shopItems)
            {
                item.Prefab = _itemManager.GetItemPrefab(item.Identifier);
            }
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

    public class CustomSerializationBinder : DefaultSerializationBinder
    {
        private static readonly HashSet<Type> TypesToShorten = new()
        {
            typeof(WeaponValueModifier),
            typeof(WeaponSpecialModifier),
        };

        public override Type BindToType(string assemblyName, string typeName)
        {
            // If the JSON only contains a class name, find the matching type
            Type matchedType = TypesToShorten.FirstOrDefault(t => t.Name == typeName);
            return matchedType ?? base.BindToType(assemblyName, typeName);
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            if (TypesToShorten.Contains(serializedType))
            {
                assemblyName = null; // No assembly needed
                typeName = serializedType.Name; // Use only the class name
            }
            else
            {
                base.BindToName(serializedType, out assemblyName, out typeName);
            }
        }
    }
   
}