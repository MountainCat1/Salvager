using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private SceneReference gameScene;
        
        public void LoadGame()
        {
            SceneManager.LoadScene(gameScene);
        }
        
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}