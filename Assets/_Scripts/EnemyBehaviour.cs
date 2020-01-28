﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LATwo
{
    public class EnemyBehaviour : Entity
    {
        protected Enemy settings;

        protected float lastShotTime;
        protected float strafeAngle; //Strafe speed = ° / s  -- this is the angle to Vector2.right (1,0)
        protected bool inStrafeDistance = false;
        protected float localStrafeDistance;
        protected int strafeDirection = 1;

        Vector2 direction;
        float dist;

        //TODO: THIS: property set??
        public Transform follow;
        public Transform target;

        private bool isFrozen = false;
        public event System.Action OnEnemyDied;

        public int PointValue => settings.pointValue;

        protected override void Die()
        {
            Message<ReturnToPool<EnemyBehaviour>>.Raise(this);

            //invoke local event and clear the delegate.
            OnEnemyDied?.Invoke();
            OnEnemyDied = null;

            //this used to be ondisable, but Die works just as fine
            inStrafeDistance = false;
            Destroy(GetComponent<PolygonCollider2D>()); //weird fix
            //yeet
            var system = DeathParticlePool.GetPoolObject();
            system.transform.position = transform.position + new Vector3(0, 0, -1.5f); //position needs to be a little offset for it to render correctly.
            var main = system.Main;
            main.startColor = settings.tint;
            system.gameObject.SetActive(true);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
                Die();
        }
#endif
        private void FixedUpdate()
        {
            if (isFrozen)
                return;
            //Movement and shooting.
            direction = ((Vector2)target.position - body.position).normalized;

            if (settings.movePattern == MovementType.Strafe)
                Strafe();
            else if (settings.movePattern == MovementType.FollowPlayer)
                FollowPlayer();
            else if (settings.movePattern == MovementType.Stationary)
            {
                transform.up = direction;
                body.velocity = Vector2.zero;
            }
            else //movePattern == MovementType.Worm
            {
                WormAlong();
            }

            Shoot();
        }

        void Shoot()
        {
            if (settings.movePattern == MovementType.Strafe && !inStrafeDistance) //strafe should only shoot while in their distance.
                return;
            if (dist > settings.attackRange) //out of range
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

        void WormAlong()
        {

        }

        void Strafe()
        {
            dist = Vector2.Distance(follow.position, body.position);
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
                    strafeAngle = Vector2.SignedAngle(Vector2.right, -direction);
                    localStrafeDistance = dist;
                }
            }
            //TODO: do strafing
            //1. change the strafe angle
            strafeAngle += settings.strafeSpeed * Time.deltaTime * strafeDirection;
            //2 move to the new position.
            Vector2 offset = Quaternion.AngleAxis(strafeAngle, Vector3.forward) * Vector2.right;
            offset *= localStrafeDistance;
            body.MovePosition((Vector2)follow.position + offset);
            transform.up = direction;
        }

        private void OnEnable()
        {
            lastShotTime = Time.time + Random.Range(-1f, 1f); //randomize the time a bit so they dont all sync
            gameObject.AddComponent<PolygonCollider2D>();
            Message<GameOver>.Add(OnGameOver);
        }

        private void OnDisable()
        {
            Message<GameOver>.Remove(OnGameOver);   
        }

        private void OnGameOver(GameOver go)
        {
            isFrozen = true;
        }

        //TODO: think about this
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.CompareTag("Wall"))
            {
                //wall.
                strafeDirection *= -1;
            }
        }

        public void FullInit(Enemy eType, Transform target)
        {
            Init(eType, target);
            //Do things based on the settings.
            if(eType.movePattern == MovementType.Worm)
            {
                //instantiate other worm parts behind this one right away.
                EnemyBehaviour previousSegment = this;
                for(int i = 0; i < settings.chainLength; i++)
                {
                    var segment = EnemyPool.GetPoolObject();
                    segment.follow = previousSegment.transform;

                    //next segment should listen for when the segment in front of it died.
                    previousSegment.OnEnemyDied += segment.SetHead;
                    previousSegment = segment;
                    //init the segment
                    segment.Init(eType, target);
                }
            }
        }

        //not having a follow target as a worm makes you act as head of the subworm.
        void SetHead()
        {
            follow = null;
        }

        public void Init(Enemy eType, Transform target)
        {
            //initialize all the enemy stuff like sprites n shit
            settings = eType;
            currentHealth = eType.health;
            this.Sprite = eType.sprite;
            renderer.color = settings.tint;
            this.target = target;
            gameObject.SetActive(true);
        }
    }
}