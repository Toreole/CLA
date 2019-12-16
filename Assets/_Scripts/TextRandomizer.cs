using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LATwo
{
    public class TextRandomizer : MonoBehaviour
    {
        [SerializeField]
        protected TMPro.TextMeshProUGUI field;
        [SerializeField]
        protected string[] possiblities;

        //this is fun.
        void Start()
        {
            field.text = possiblities[Random.Range(0, possiblities.Length)];
        }
    }
}
