using Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;
using Zenject;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Inject] private IDataManager _dataManager;
        
        [SerializeField] private SceneReference gameScene;
        
        [SerializeField] private Button loadGameButton;

        private void Start()
        {
            loadGameButton.interactable = _dataManager.LoadData() != null;
        }

        public void LoadGame()
        {
            SceneManager.LoadScene(gameScene);
        }

        public void StartNewGame()
        {
            _dataManager.DeleteData();
            SceneManager.LoadScene(gameScene);
        }
        
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}