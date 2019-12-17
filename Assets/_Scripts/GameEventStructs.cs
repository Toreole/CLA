using System;
using System.Collections.Generic;
using UnityEngine;

namespace LATwo
{
    public struct VerifyScore
    {
        public int score;
        public string name;
    }

    public struct PlayerDamaged
    {
        public float newHealth;
        public float damage;
    }

    // - EnemyBehaviour
    // - DeathParticle
    // - PowerupBehaviour
    // - ProjectileBehaviour
    public struct ReturnToPool<T> where T : Component
    {
        public T value;

        public Transform transform => value.transform;

        public static implicit operator ReturnToPool<T>(T obj) { return new ReturnToPool<T>() { value = obj }; }
        public static implicit operator T(ReturnToPool<T> obj) { return obj.value; }
    }

    public struct PickupPowerup
    {
        public Powerup value;
        public static implicit operator Powerup(PickupPowerup obj) { return obj.value; }
        public static implicit operator PickupPowerup(Powerup obj) { return new PickupPowerup { value = obj }; }
    }

    public struct GameOver
    {
        public bool playerDied;
        public int finalScore;
    }

    public struct GameStarted
    {

    }

    public struct LevelChanged
    {

    }

    public struct StageCleared
    {

    }
}
