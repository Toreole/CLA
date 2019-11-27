using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LATwo
{
    public class WaveManager : MonoBehaviour
    {
        public Wave[] waves;

        protected int totalEnemies;

        private IEnumerator Start()
        {
            foreach (var x in waves)
                totalEnemies += x.amount;

            foreach(Wave wave in waves)
            {
                for(int i = 0; i < wave.amount; i++)
                {
                    var enemy = EnemyPool.GetPoolObject();
                    enemy.Settings = wave.enemyType;
                    Vector2 direction = Random.insideUnitCircle.normalized;
                    Vector2 offset = Mathf.Lerp(wave.minDistance, wave.maxDistance, Random.value) * direction;
                    enemy.transform.position = PlayerController.Position + offset;
                    enemy.gameObject.SetActive(true);
                }
                yield return new WaitForSeconds(wave.time);
            }
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
            public float minDistance, maxDistance;
        }
    }
}