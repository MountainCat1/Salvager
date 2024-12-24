using UnityEngine;
using Zenject;

namespace Managers
{
    [System.Serializable]
    public class GameData
    {
        public bool ILiveFinallyInPeace = false;
    }

    public interface IGameDataManager
    {
        public void SaveData();
        public GameData GameData { get; }
    }

    public class GameDataManager : IGameDataManager
    {
        public GameData GameData { get; private set; }

        [Inject]
        private void Initialize()
        {
            Debug.Log("GameDataManager initialized!");

            GameData = LoadData();
            
            Application.quitting += SaveData;
        }
        

        public void SaveData()
        {
            Debug.Log("Saving data...");

            var path = Application.persistentDataPath + "/gameData.json";

            var json = JsonUtility.ToJson(GameData);
            System.IO.File.WriteAllText(path, json);

            Debug.Log("Data saved!");
        }

        public GameData LoadData()
        {
            Debug.Log("Loading data...");

            var path = Application.persistentDataPath + "/gameData.json";
            if (!System.IO.File.Exists(path))
            {
                Debug.Log("No data found!");
                return new GameData();
            }

            var json = System.IO.File.ReadAllText(path);
            var data = JsonUtility.FromJson<GameData>(json);

            Debug.Log("Data loaded!");
            
            return data;
        }
    }
}