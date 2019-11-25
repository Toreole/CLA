using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LATwo
{
    public class ProjectileBehaviour : MonoBehaviour
    {
        public Projectile Settings { get; set; }
        public GameObject HitEffect { get; set; }
        
        [SerializeField]
        protected Rigidbody2D body;
#if UNITY_EDITOR
        [SerializeField]
        Projectile fallbackSettings;
#endif
        private void OnEnable()
        {
#if UNITY_EDITOR
            Settings = (Settings)? Settings: fallbackSettings;
#endif
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
            float angle = Vector2.Angle(transform.up, PlayerController.Position - body.position);
            //the angle betwee the movement and shit.
            body.rotation += Mathf.Clamp((angle > 0? 1 : -1) * Settings.turnSpeed * Time.deltaTime, -angle, angle);
            print(angle);
            body.velocity = Settings.speed * transform.up; //update velocity
        }

        IEnumerator EndLifeTime()
        {
            yield return new WaitForSeconds(Settings.lifeTime);
            Message<ProjectileBehaviour>.Raise(this); //TODO: register this in the pool
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
            Message<ProjectileBehaviour>.Raise(this);
        }
    }
}