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
        
        private void OnEnable()
        {
            Message<StageCleared>.Add(StageClear);
        }

        private void OnDisable()
        {
            Message<StageCleared>.Remove(StageClear);
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
    }
}