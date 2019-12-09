using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LATwo
{
    public class WaveManager : MonoBehaviour
    {
        public Wave[] waves;

        [SerializeField]
        protected LayerMask wallLayer;

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
                    var pos = GetRandomPosition(wave);
                    while (!PositionIsValid(pos))
                        pos = GetRandomPosition(wave);
                    enemy.transform.position = GetRandomPosition(wave);
                    enemy.gameObject.SetActive(true);

                }
                yield return new WaitForSeconds(wave.time);
            }
        }

        Vector2 GetRandomPosition(Wave wave)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            Vector2 offset = Mathf.Lerp(wave.minDistance, wave.maxDistance, Random.value) * direction;
            return direction * offset;
        }
        bool PositionIsValid(Vector2 pos)
        {
            return !Physics2D.OverlapPoint(pos, wallLayer);
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