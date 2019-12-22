using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace LATwo
{
    public class GameEndScreen : MonoBehaviour
    {
        [SerializeField]
        protected GameObject generalEndScreen;
        [SerializeField]
        protected GameObject failText, winText, continuePrompt;
        [SerializeField]
        protected GameObject endScreenA, endScreenScore;
        [SerializeField]
        protected TextMeshProUGUI highscoreListContent, finalScore;
        [SerializeField]
        protected TMP_InputField nameInput;
        [SerializeField]
        protected GameObject returnToMenuButton;

        protected bool alreadyOver = false;

        private void OnEnable()
        {
            Message<GameOver>.Add(EndGame);
        }

        private void OnDisable()
        {
            Message<GameOver>.Remove(EndGame);
        }

        void EndGame(GameOver go)
        {
            if (alreadyOver)
                return;
            alreadyOver = true;
            generalEndScreen.SetActive(true);
            endScreenA.SetActive(true);
            failText.SetActive(go.playerDied);
            winText.SetActive(!go.playerDied);
            StartCoroutine(WaitForInput());
        }

        IEnumerator WaitForInput()
        {
            yield return new WaitForSeconds(1.5f);
            continuePrompt.SetActive(true);
            for(; ; )
            {
                if (Input.anyKeyDown)
                    break;
                yield return null;
            }
            endScreenA.SetActive(false);
            endScreenScore.SetActive(true);
            UpdateScoreList();

            finalScore.text = PlayerController.CurrentScore.ToScoreString(6);

            //submit the score.
            for (; ;)
            {
                if(Input.GetKeyDown(KeyCode.Return))
                {
                    if(nameInput.text.Length == 3)
                    {
                        HighscoreKeeper.AddScore(PlayerController.CurrentScore, nameInput.text);
                        UpdateScoreList();
                        nameInput.interactable = false;
                        break;
                    }
                }
                yield return null;
            }
            returnToMenuButton.SetActive(true);
        }

        private void UpdateScoreList()
        {
            string[] texts = new string[10];
            string allText = "";
            int scores = HighscoreKeeper.GetScoreTexts(ref texts);
            for (int i = 0; i < scores; i++)
            {
                allText += (i%2 == 0) ? $"{texts[i]} " : $"| {texts[i]}\n";
            }
            highscoreListContent.text = allText;
        }

        public void BackToMenu()
        {
            GameManager.OpenMenu();
        }
    }
}