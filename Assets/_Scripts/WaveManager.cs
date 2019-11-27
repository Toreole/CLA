using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LATwo
{
    public class WaveManager : MonoBehaviour
    {
        public Wave[] waves;

        protected int totalEnemies;

        private void Start()
        {
            foreach (var x in waves)
                totalEnemies += x.amount;
        }

        //this is what spawns the waves and detects whether its been completed yet.
        //waves spawn after time.
        //all waves done => next level
        [System.Serializable]
        public class Wave
        {
            //not sure about this one chief.
            public Enemy enemyType;
            public int amount;
            public float time;
        }
    }
}