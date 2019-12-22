using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace LATwo
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        protected string currentLevel = "";

        [SerializeField]
        protected string menuScene = "Menu", playerPreloadScene = "Player_Setup", firstLevel = "Level_0";
        [SerializeField]
        protected AudioSource sfxAudio, bgm_A, bgm_B;
        [SerializeField]
        protected float fadeTime;

        protected BGMState bgmState = BGMState.None;
        protected HashSet<AudioClip> clips = new HashSet<AudioClip>();

        public enum BGMState
        {
            A, B, None
        }

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
            if (!instance)
                return;
            SceneManager.UnloadSceneAsync(instance.currentLevel);
            SceneManager.LoadScene(level, LoadSceneMode.Additive); //fix
            instance.currentLevel = level;
        }

        public static void TransitionBGM(AudioClip track) { if (instance) instance.M_TransitionBGM(track); } 

        private void M_TransitionBGM(AudioClip track)
        {
            if(bgmState == BGMState.None)
            {
                bgm_A.clip = track;
                bgm_A.Play();
                bgmState = BGMState.A;
                return;
            }
            else
            {
                //toggle
                var isA = bgmState == BGMState.A;
                bgmState = (isA) ? BGMState.B : BGMState.A;
                var audioA = (isA) ? bgm_A : bgm_B;
                var audioB = (isA) ? bgm_B : bgm_A;
                StartCoroutine(CrossFade(audioA, audioB));
                //assign the clip.
                audioB.clip = track;
                audioB.volume = 0f; //mute audio
                audioB.Play();
            }
        }

        private void LateUpdate()
        {
            clips.Clear();
        }

        IEnumerator CrossFade(AudioSource a, AudioSource b)
        {
            for(float t = 0f; t < fadeTime; t+= Time.deltaTime)
            {
                float normT = t / fadeTime;
                a.volume = 1f - normT;
                b.volume = normT;
                yield return null;
            }

            a.volume = 0f;
            b.volume = 1f;
            a.Stop();
        }

        public static void OpenMenu()
        {
            instance.currentLevel = "";
            SceneManager.LoadScene(instance.menuScene);
        }

        //yeet
        public static void PlaySFX(AudioClip clip)
        {
            if (instance.clips.Contains(clip))
                return;
            instance.clips.Add(clip);
            instance.sfxAudio.PlayOneShot(clip);
        }
    }
}