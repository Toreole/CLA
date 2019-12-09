using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LATwo
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField]
        protected TMPro.TextMeshProUGUI scoreDisplay;
        [SerializeField]
        protected int scoreLength = 6;

        int currentScore;

        private void OnEnable()
        {
            Message<ReturnToPool<EnemyBehaviour>>.Add(UpdateScore);
            scoreDisplay.text = GetScoreText();
        }

        private void OnDisable()
        {
            Message<ReturnToPool<EnemyBehaviour>>.Remove(UpdateScore);
        }

        //since the return to pool message is called on enemies when they die, this works
        void UpdateScore(ReturnToPool<EnemyBehaviour> enemy)
        {
            currentScore += enemy.value.Settings.pointValue;
            //update score, then update UI
            scoreDisplay.text = GetScoreText();
        }

        string GetScoreText()
        {
            string score = currentScore.ToString();
            for (int l = score.Length; l < scoreLength; l++)
                score = "0" + score;
            return score;
        }
    }
}