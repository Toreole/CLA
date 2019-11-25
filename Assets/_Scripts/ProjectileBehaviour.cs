using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LATwo
{
    public class ProjectileBehaviour : MonoBehaviour
    {
        public Projectile Settings { get; set; }
        public GameObject HitEffect { get; set; }
        public Sprite Sprite { set => renderer.sprite = value; }
        
        [SerializeField]
        protected Rigidbody2D body;
        [SerializeField]
        protected new SpriteRenderer renderer;

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
            if (!Settings.homing)
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
            Message<ReturnToPool<ProjectileBehaviour>>.Raise(this); //TODO: register this in the pool
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            //TODO: set up layers so it ONLY collides with the player.
            //send a message (yikes)
            //do the hitEffect thingy.
            //disables this and notify the pool
            if(HitEffect)
            {
                //...
            }
            Message<ReturnToPool<ProjectileBehaviour>>.Raise(this);
        }
    }
}