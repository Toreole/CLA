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
        [SerializeField]
        protected string nextLevel;
        [SerializeField]
        protected bool waitOnGameStartCountdown = false;
        [SerializeField]
        protected float startOffset = 1f;

        protected int totalEnemies;
        protected int deadEnemies = 0;

        private IEnumerator SpawnWaves()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            Message<StageStarted>.Raise(default);
            yield return new WaitForSeconds(startOffset);
            foreach (var x in waves)
                totalEnemies += x.amount;

            foreach(Wave wave in waves)
            {
                for(int i = 0; i < wave.amount; i++)
                {
                    var enemy = EnemyPool.GetPoolObject();
                    enemy.FullInit(wave.enemyType, PlayerController.current.transform);
                    var pos = GetRandomPosition(wave);
                    while (!PositionIsValid(pos))
                        pos = GetRandomPosition(wave);
                    enemy.transform.position = GetRandomPosition(wave);

                }
                yield return new WaitForSeconds(wave.time);
            }
        }

        private void OnEnable()
        {
            Message<ReturnToPool<EnemyBehaviour>>.Add(EnemyDied);
            if(waitOnGameStartCountdown)
                Message<GameStarted>.Add(StartGame);
            else
                StartCoroutine(SpawnWaves());
            Message<GameOver>.Add(EndGame);
        }

        private void OnDisable()
        {
            Message<ReturnToPool<EnemyBehaviour>>.Remove(EnemyDied);
            if (waitOnGameStartCountdown)
                Message<GameStarted>.Remove(StartGame);
            Message<GameOver>.Remove(EndGame);
        }

        private void StartGame(GameStarted gs)
        {
            StartCoroutine(SpawnWaves());
        }

        void EndGame(GameOver go)
        {
            //stop the spawning.
            StopAllCoroutines();
        }

        void EnemyDied(ReturnToPool<EnemyBehaviour> enemy)
        {
            deadEnemies++;
            if(deadEnemies >= totalEnemies)
            {
                //This should go to the next level, OR show the winning end-screen.
                if (string.IsNullOrEmpty(nextLevel))
                {
                    Message<GameOver>.Raise(new GameOver() { finalScore = PlayerController.CurrentScore, playerDied = false });
                }
                else
                {
                    Message<StageCleared>.Raise(new StageCleared() { bufferTime = 3f }); //TODO: i guess
                    StartCoroutine(GoToNextLevel());
                }
            }
        }

        IEnumerator GoToNextLevel()
        {
            yield return new WaitForSeconds(3f);
            GameManager.GoToLevel(nextLevel);
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