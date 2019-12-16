using UnityEngine;
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
        protected float attackRate = 0.4f;
        [SerializeField]
        protected Camera cam;
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
        
        public static int CurrentScore { get; private set; }
        
        protected void StartGame(GameStarted start)
        {
            currentHealth = maxHealth;
        }

        private void OnEnable()
        {
            Message<ReturnToPool<EnemyBehaviour>>.Add(UpdateScore);
            scoreDisplay.text = GetScoreText();
            Message<PickupPowerup>.Add(ApplyPowerup);
            Message<GameStarted>.Add(StartGame);
        }

        private void OnDisable()
        {
            Message<ReturnToPool<EnemyBehaviour>>.Remove(UpdateScore);
            Message<PickupPowerup>.Remove(ApplyPowerup);
            Message<GameStarted>.Remove(StartGame);
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
            }
        }

        IEnumerator RemoveSpeed(float strength, float length)
        {
            yield return new WaitForSeconds(length);
            speed -= strength;
        }

        protected override void Die()
        {
            //TODO: THIS.
            body.velocity = Vector2.zero;
            Message<GameOver>.Raise(new GameOver() { finalScore = CurrentScore, playerDied = true } );
        }

        private void FixedUpdate()
        {
            if (currentHealth <= 0)
                return;
            //target movement for this frame.
            movementInput *= speed;
            body.velocity = movementInput;

            //update position at the end of this fixedupdate lol
            Position = body.position;
        }

        private void Update()
        {
            if (currentHealth <= 0)
                return;
            //movement input
            movementInput.x = Input.GetAxis(horizontalInput);
            movementInput.y = Input.GetAxis(verticalInput);
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
            base.Damage(amount);
            var dmg = new PlayerDamaged
            {
                newHealth = currentHealth,
                damage = amount
            };
            Message<PlayerDamaged>.Raise(dmg);
        }

        //EDIT: Removed ScoreManager and added score to the player.
        //since the return to pool message is called on enemies when they die, this works
        void UpdateScore(ReturnToPool<EnemyBehaviour> enemy)
        {
            CurrentScore += enemy.value.Settings.pointValue;
            //update score, then update UI
            scoreDisplay.text = GetScoreText();
        }

        string GetScoreText()
        {
            string score = CurrentScore.ToString();
            for (int l = score.Length; l < scoreLength; l++)
                score = "0" + score;
            return score;
        }

        internal static void ResetScore()
            => CurrentScore = 0;
    }
}