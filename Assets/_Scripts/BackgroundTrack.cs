using UnityEngine;
using System.Collections;

namespace LATwo
{
    public class BackgroundTrack : MonoBehaviour
    {
        [SerializeField]
        protected AudioClip track;

        // Use this for initialization
        void Start()
        {
            GameManager.TransitionBGM(track);
        }
    }
}