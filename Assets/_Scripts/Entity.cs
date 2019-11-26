﻿using UnityEngine;

namespace LATwo
{
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField]
        protected Rigidbody2D body;
        [SerializeField]
        protected new SpriteRenderer renderer;

        public Sprite Sprite { set { renderer.sprite = value; } }

        protected float currentHealth;

        public void Damage(float amount)
        {
            currentHealth -= amount;
            if(currentHealth <= 0)
            {
                Die();
            }
        }

        protected abstract void Die();
    }
}
