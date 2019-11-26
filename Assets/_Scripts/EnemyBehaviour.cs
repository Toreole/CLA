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
                renderer.color = settings.tint;
            }
        }
        protected Enemy settings;

        protected float lastShotTime;
        protected float strafeAngle; //Strafe speed = ° / s  -- this is the angle to Vector2.right (1,0)
        protected bool inStrafeDistance = false;
        protected float localStrafeDistance;
        protected int strafeDirection = 1;

        Vector2 direction;
        float dist;

        protected override void Die()
        {
            Message<ReturnToPool<EnemyBehaviour>>.Raise(this);
        }

        private void FixedUpdate()
        {
            //Movement and shooting.
            direction = (PlayerController.Position - body.position).normalized;

            if (Settings.movePattern == MovementType.Strafe)
                Strafe();
            else if (Settings.movePattern == MovementType.FollowPlayer)
                FollowPlayer();

            Shoot();
        }

        void Shoot()
        {
            if (settings.movePattern == MovementType.Strafe && !inStrafeDistance) //strafe should only shoot while in their distance.
                return;
            else if (dist > settings.attackRange) //out of range
                return;
            if(Time.time - lastShotTime >= settings.attackRate)
            {
                ProjectilePool.GetPoolObject().Init(settings.projectile, gameObject);
                lastShotTime = Time.time;
            }
        }

        void FollowPlayer()
        {
            transform.up = direction;
            body.velocity = direction * settings.speed;
        }

        void Strafe()
        {
            dist = Vector2.Distance(PlayerController.Position, body.position);
            if (!inStrafeDistance)
            {
                if (dist > settings.preferredStrafeDistance + settings.strafeTolerance)
                {
                    FollowPlayer();
                    return;
                }
                else if (dist < settings.preferredStrafeDistance - settings.strafeTolerance)
                {
                    //run away from player (backwards)
                    transform.up = direction;
                    body.velocity = -direction * settings.speed;
                    return;
                }
                else
                {
                    inStrafeDistance = true;
                    strafeAngle = Vector2.SignedAngle(Vector2.right, direction);
                    localStrafeDistance = dist;
                }
            }
            //TODO: do strafing
            //1. change the strafe angle
            strafeAngle += settings.strafeSpeed * Time.deltaTime * strafeDirection;
            //2 move to the new position.
            Vector2 offset = Quaternion.AngleAxis(strafeAngle, Vector3.forward) * Vector2.right;
            offset *= localStrafeDistance;
            body.MovePosition(PlayerController.Position + offset);
        }

        private void OnEnable()
        {
            lastShotTime = Time.time;
        }

        private void OnDisable()
        {
            inStrafeDistance = false;
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.CompareTag("Wall"))
            {
                //wall.
                strafeDirection *= -1;
            }
        }
    }
}