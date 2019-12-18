using UnityEngine;
using System.Collections;
using TMPro;

namespace LATwo
{
    public class InputFieldUtility : MonoBehaviour
    {
        [SerializeField]
        protected TMP_InputField input;

        public void MakeTextAllUpper()
        {
            input.text = input.text.ToUpper();
        }
    }
}