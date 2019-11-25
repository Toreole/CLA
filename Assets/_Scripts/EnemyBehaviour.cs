using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LATwo
{
    public class EnemyBehaviour : MonoBehaviour
    {
        [SerializeField]
        protected Enemy settings;
        
        //the target basically
        public Transform Player { get; set; }

        void Die()
        {
            Message<EnemyBehaviour>.Raise(this);
        }

    }
}