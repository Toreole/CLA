using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace LATwo
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        protected string currentLevel = "";

        [SerializeField]
        protected string menuScene = "Menu", playerPreloadScene = "Player_Setup", firstLevel = "Level_0";

        void Start()
        {
            if (instance)
                Destroy(gameObject);
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            SceneManager.LoadScene(menuScene); //complete this and open the menu. 
        }

        public static void StartGame()
        {
            if (instance)
                instance.M_StartGame();
            else
                Debug.LogError("No GameManager loaded");
        }

        protected void M_StartGame()
        {
            //reset the score to 0.
            PlayerController.ResetScore();
            //set the first level as the active one.
            currentLevel = firstLevel;
            SceneManager.LoadScene(playerPreloadScene);
            SceneManager.LoadScene(firstLevel, LoadSceneMode.Additive);
        }

        public static void GoToLevel(string level)
        {
            SceneManager.UnloadSceneAsync(instance.currentLevel);
            SceneManager.LoadScene(level);
            instance.currentLevel = level;
        }
    }
}