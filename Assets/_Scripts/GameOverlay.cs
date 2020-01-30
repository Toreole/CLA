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
        protected TextMeshProUGUI prompt;
        [SerializeField]
        protected float promptTime = 3f, promptDist = 75f;
        [SerializeField]
        protected GameObject pauseScreen;

        Vector2 startPos;

        private void Awake()
        {
            RectTransform rect = prompt.transform as RectTransform;
            startPos = rect.anchoredPosition;
        }

        private void OnEnable()
        {
            Message<StageCleared>.Add(StageClear);
            Message<PromptText>.Add(ShowPrompt);
            Message<PauseGame>.Add(Pause);
            Message<ContinueGame>.Add(Continue);
        }

        private void OnDisable()
        {
            Message<StageCleared>.Remove(StageClear);
            Message<PromptText>.Remove(ShowPrompt);
            Message<PauseGame>.Remove(Pause);
            Message<ContinueGame>.Remove(Continue);
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
            StopAllCoroutines();
            this.prompt.text = prompt.text;
            this.prompt.color = prompt.color;
            StartCoroutine(PromptMove());
        }

        IEnumerator PromptMove()
        {
            prompt.alpha = 1f;
            RectTransform rect = prompt.transform as RectTransform;
            for(float t = 0f; t<promptTime; t+= Time.deltaTime)
            {
                var normT = t / promptTime;
                prompt.alpha = 1f - normT;
                Vector2 pos = startPos;
                pos.y += normT * promptDist;
                rect.anchoredPosition = pos;
                yield return null;
            }
            rect.anchoredPosition = startPos;
            prompt.alpha = 0f;
        }

        void Pause(PauseGame cock)
        {
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
        }

        void Continue(ContinueGame balls)
        {
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}