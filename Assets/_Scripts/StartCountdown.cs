using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LATwo
{
    public class StartCountdown : MonoBehaviour
    {
        [SerializeField]
        protected TMPro.TextMeshProUGUI textField;

        IEnumerator Start()
        {
            var second = new WaitForSeconds(.5f);
            for (int i = 3; i > 0; i--)
            {
                textField.text = i.ToString();
                yield return second;
            }
            textField.text = "Let's Go!";
            Message<GameStarted>.Raise(default);
            yield return second;
            gameObject.SetActive(false);
        }
    }
}