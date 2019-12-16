using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LATwo
{
    public class HighscoreUI : MonoBehaviour
    {
        [SerializeField]
        protected TMPro.TextMeshProUGUI textField;

        private void OnEnable()
        {
            string[] scores = new string[10];
            string finalText = "";
            int i = 0;
            //woo wee
            for(; i < HighscoreKeeper.GetScoreTexts(ref scores); i++)
            {
                finalText += scores[i] + "\n";
            }
            if(i == 0)
            {
                finalText = "Looks like there are no records so far...";
            }
            textField.text = finalText;
        }
    }
}