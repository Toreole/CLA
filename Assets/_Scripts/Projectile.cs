using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LATwo
{
    /// <summary>
    /// SO that defines Projectiles
    /// </summary>
    [CreateAssetMenu(menuName = "Game_SO/ProjectileSettings")]
    public class Projectile : ScriptableObject
    {
        public Sprite sprite;
        public Color tint;
        public float damage;
        public float speed;
        public bool homing;
        public float turnSpeed;
        public float lifeTime;
        public GameObject hitEffect;
        public AudioClip shootSound;
    }
}
