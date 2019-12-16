using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LATwo
{
    public class NeonButton : MonoBehaviour, UnityEngine.EventSystems.IPointerDownHandler
    {
        [SerializeField]
        protected TMPro.TextMeshProUGUI text;

        protected bool on = true;

        public void OnPointerDown(PointerEventData eventData)
        {
            on = !on;
            text.text = on ? "NEON" : "NEOFF";
        }
    }
}