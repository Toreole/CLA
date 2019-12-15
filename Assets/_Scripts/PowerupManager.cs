using UnityEngine;
using System.Collections;

namespace LATwo
{
    public class PowerupManager : Pool<PowerupBehaviour>
    {
        [SerializeField]
        protected Powerup[] powerups;
        [SerializeField, Range(0f, 1f)]
        protected float baseSpawnChance = 0.1f;
        [SerializeField]
        protected int enemyCountToAlwaysSpawn = 10;

        protected float spawnChance;

        protected override void OnEnable()
        {
            base.OnEnable();
            spawnChance = baseSpawnChance;
            Message<ReturnToPool<EnemyBehaviour>>.Add(SpawnPowerup);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Message<ReturnToPool<EnemyBehaviour>>.Remove(SpawnPowerup);
        }

        void SpawnPowerup(ReturnToPool<EnemyBehaviour> rEnemy)
        {
            //verify spawn
            if(Random.value < spawnChance)
            {
                //Get a random powerup
                var toSpawn = powerups[Random.Range(0, powerups.Length)];
                //initialize object
                var spawned = M_GetPoolObject();
                spawned.transform.position = rEnemy.transform.position;
                //set powerup (also enables the object)
                spawned.Powerup = toSpawn;
                spawnChance = baseSpawnChance;
            } 
            else
            {
                //increase the spawnChance. 
                spawnChance += ((1f - baseSpawnChance) / (float)enemyCountToAlwaysSpawn);
            }
        }
    }
}