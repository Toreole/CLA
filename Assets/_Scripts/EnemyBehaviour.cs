using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LATwo
{
    public class EnemyBehaviour : Entity
    {
        //i like properties
        public Enemy Settings {
            private get { return settings; }
            set {
                settings = value;
                currentHealth = settings.health;
                this.Sprite = settings.sprite;
            }
        }
        protected Enemy settings;

        protected float lastShotTime;

        void Die()
        {
            Message<ReturnToPool<EnemyBehaviour>>.Raise(this);
        }

        private void Update()
        {
            //Movement and shooting.
        }

    }
}