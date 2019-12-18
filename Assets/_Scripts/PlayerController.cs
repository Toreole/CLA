﻿using UnityEngine;
using System.Collections;

namespace LATwo
{
    public class PlayerController : Entity
    {
        public static Vector2 Position { get; private set; } //hacky but necessary

        [SerializeField] //primary input
        protected string horizontalInput = "Horizontal",
            verticalInput = "Vertical",
            fire = "LeftClick",
            secondaryFire = "RightClick";
        [SerializeField]
        protected float maxHealth = 100f, speed = 5f;
        [SerializeField]
        protected Projectile projectile;
        [SerializeField]
        protected float attackRate = 0.4f, invulTime = 0.2f;
        [SerializeField]
        protected Camera cam;
        [SerializeField]
        protected AudioClip hitSound;
        //SCORE
        [SerializeField]
        protected TMPro.TextMeshProUGUI scoreDisplay;
        [SerializeField]
        protected int scoreLength = 6;
        
        protected Vector2 mouseDir, movementInput;
        protected float lastShotTime, lastSecondaryShot;
        //secondary stuff is given by powerups.
        protected Projectile secondaryProjectile;
        protected int secondaryAmmo;

        //yes i need this
        protected bool canMove = false;
        protected bool vulnerable = true;

        public static int CurrentScore { get; private set; }
        
        protected void StartGame(GameStarted start)
        {
            canMove = true;
            currentHealth = maxHealth;
        }

        private void OnEnable()
        {
            scoreDisplay.text = CurrentScore.ToScoreString(scoreLength);
            Message<ReturnToPool<EnemyBehaviour>>.Add(UpdateScore);
            Message<PickupPowerup>.Add(ApplyPowerup);
            Message<GameStarted>.Add(StartGame);
            Message<StageCleared>.Add(OnClearStage);
            Message<StageStarted>.Add(OnStageStart);
        }

        private void OnDisable()
        {
            Message<ReturnToPool<EnemyBehaviour>>.Remove(UpdateScore);
            Message<PickupPowerup>.Remove(ApplyPowerup);
            Message<GameStarted>.Remove(StartGame);
            Message<StageCleared>.Remove(OnClearStage);
            Message<StageStarted>.Remove(OnStageStart);
        }

        void OnStageStart(StageStarted st)
        {
            canMove = true;
            vulnerable = true;
            //body.isKinematic = false;
        }

        void OnClearStage(StageCleared st)
        {
            canMove = false;
            vulnerable = false;
            body.velocity = Vector2.zero;
            //body.isKinematic = true;
        }

        void ApplyPowerup(PickupPowerup powerup)
        {
            var p = powerup.value;
            switch(p.type)
            {
                case PowerupType.Health:
                    //print("heal:" + p.effectStrength.ToString("00.00")); 
                    //print(currentHealth);
                    currentHealth = Mathf.Clamp(currentHealth + p.effectStrength, 0f, maxHealth);
                    //print(currentHealth);
                    Message<PlayerDamaged>.Raise(new PlayerDamaged() { newHealth = currentHealth, damage = 0f });
                    break;
                case PowerupType.Projectile:
                    secondaryProjectile = p.projectile;
                    secondaryAmmo = (int)p.effectStrength;
                    break;
                case PowerupType.Speed:
                    speed += p.effectStrength;
                    StartCoroutine(RemoveSpeed(p.effectStrength, p.effectLength));
                    break;
                case PowerupType.Firerate:
                    attackRate /= p.effectStrength;
                    StartCoroutine(RemoveFireBoost(p.effectStrength, p.effectLength));
                    break; 

            }
        }

        IEnumerator RemoveSpeed(float strength, float length)
        {
            yield return new WaitForSeconds(length);
            speed -= strength;
        }
        IEnumerator RemoveFireBoost(float boost, float time)
        {
            yield return new WaitForSeconds(time);
            attackRate *= boost;
        }

        protected override void Die()
        {
            //TODO: THIS.
            body.velocity = Vector2.zero;
            Message<GameOver>.Raise(new GameOver() { finalScore = CurrentScore, playerDied = true } );
            vulnerable = false;
        }

        private void FixedUpdate()
        {
            if (currentHealth <= 0 || !canMove)
                return;
            //target movement for this frame.
            movementInput *= speed;
            body.velocity = movementInput;

            //update position at the end of this fixedupdate lol
            Position = body.position;
        }

        private void Update()
        {
            if (currentHealth <= 0 || !canMove)
                return;
            //movement input
            movementInput.x = Input.GetAxisRaw(horizontalInput);
            movementInput.y = Input.GetAxisRaw(verticalInput);
            movementInput.Normalize();

            //mouse position -> direction
            Vector2 mp = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseDir = (mp - body.position).normalized;
            transform.up = mouseDir;

            //firing bullets.
            if(Input.GetButton(fire) && Time.time - lastShotTime >= attackRate)
            {
                ProjectilePool.GetPoolObject().Init(projectile, gameObject);
                lastShotTime = Time.time;
            }
            if (secondaryAmmo > 0)
            {
                if (Input.GetButton(secondaryFire) && Time.time - lastSecondaryShot >= attackRate)
                {
                    ProjectilePool.GetPoolObject().Init(secondaryProjectile, gameObject);
                    lastSecondaryShot = Time.time;
                    secondaryAmmo--;
                }
            }
        }

        public override void Damage(float amount)
        {
            if (!vulnerable)
                return;
            base.Damage(amount);
            var dmg = new PlayerDamaged
            {
                newHealth = currentHealth,
                damage = amount
            };
            Message<PlayerDamaged>.Raise(dmg);
            GameManager.PlaySFX(hitSound);
            StartCoroutine(DoInvulnerab());
            IEnumerator DoInvulnerab()
            {
                vulnerable = false;
                yield return new WaitForSeconds(invulTime);
                vulnerable = true;
            }
        }

        //EDIT: Removed ScoreManager and added score to the player.
        //since the return to pool message is called on enemies when they die, this works
        void UpdateScore(ReturnToPool<EnemyBehaviour> enemy)
        {
            CurrentScore += enemy.value.Settings.pointValue;
            //update score, then update UI
            scoreDisplay.text = CurrentScore.ToScoreString(scoreLength);
        }
        
        internal static void ResetScore()
            => CurrentScore = 0;
    }
}