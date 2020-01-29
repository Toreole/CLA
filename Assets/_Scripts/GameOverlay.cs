using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace LATwo
{
    public class GameOverlay : MonoBehaviour
    {
        [SerializeField]
        protected GameObject stageClearText;
        [SerializeField]
        protected string menuScene = "Menu";
        [SerializeField]
        protected GameObject promptPrefab;

        private void OnEnable()
        {
            Message<StageCleared>.Add(StageClear);
            Message<PromptText>.Add(ShowPrompt);
        }

        private void OnDisable()
        {
            Message<StageCleared>.Remove(StageClear);
            Message<PromptText>.Remove(ShowPrompt);
        }

        void StageClear(StageCleared st)
        {
            stageClearText.SetActive(true);
            StartCoroutine(DoStageClear(st.bufferTime));
            IEnumerator DoStageClear(float t)
            {
                yield return new WaitForSeconds(t);
                stageClearText.SetActive(false);
            }
        }

        public void GoToMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(menuScene);
        }

        void ShowPrompt(PromptText prompt)
        {

        }
    }
}