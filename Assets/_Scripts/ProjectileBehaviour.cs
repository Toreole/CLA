using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LATwo
{
    public class ProjectileBehaviour : MonoBehaviour
    {
        public Projectile Settings
        {
            get { return settings; }
            set {
                settings = value;
                renderer.sprite = settings.sprite;
                renderer.color = settings.tint;
            }
        }
        public Vector2 Position { set => transform.position = value; }
        protected Projectile settings;

        [SerializeField]
        protected Rigidbody2D body;
        [SerializeField]
        protected new SpriteRenderer renderer;

        public void Init(Projectile settings, GameObject context)
        {
            gameObject.layer = context.layer;
            transform.up = context.transform.up;
            this.Settings = settings;
            transform.position = context.transform.position;
            gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            if (Settings == null)
            {
                gameObject.SetActive(false);
                return;
            }
            body.velocity = Settings.speed * transform.up;
            StartCoroutine(EndLifeTime());
        }

        private void FixedUpdate()
        {
            if (!Settings.homing) //! homing projectiles only used by enemies, not by the player.
                return;
            //homing missiles should change trajectory around z axis (rotation);
            float angle = Vector2.SignedAngle(transform.up, (PlayerController.Position - body.position).normalized);
            //the angle betwee the movement and shit.
            body.rotation += (angle > 0? 1 : -1) * Mathf.Clamp(Settings.turnSpeed * Time.deltaTime, 0f, Mathf.Abs(angle));
            print(angle);
            body.velocity = Settings.speed * transform.up; //update velocity
        }

        IEnumerator EndLifeTime()
        {
            yield return new WaitForSeconds(Settings.lifeTime);
            if (Settings.hitEffect)
                Instantiate(Settings.hitEffect, transform.position, Quaternion.identity);
            print("die");
            Message<ReturnToPool<ProjectileBehaviour>>.Raise(this); 
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var entity = collision.gameObject.GetComponent<Entity>();
            if (entity)
                entity.Damage(settings.damage);
            if(Settings.hitEffect)
                Instantiate(Settings.hitEffect, transform.position, Quaternion.identity);
            Message<ReturnToPool<ProjectileBehaviour>>.Raise(this);
        }
    }
}