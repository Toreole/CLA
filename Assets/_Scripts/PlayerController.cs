using UnityEngine;
using System.Collections;

namespace LATwo
{
    public class PlayerController : Entity
    {
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
        protected float actualScore = 0;
        protected float scoreMultiplier = 1.0f;
        protected int enemiesKilled;
        protected bool gotDamage = false;

        protected bool gamePaused = false;

        [Header("Score")]
        [SerializeField]
        protected float scoreMulGain = 0.1f;
        [SerializeField]
        protected int enemiesForMulGain = 5;

        //current active player, should never be null realistically.
        public static PlayerController current;

        protected void StartGame(GameStarted start)
        {
            canMove = true;
            currentHealth = maxHealth;
        }

        private void OnEnable()
        {
            current = this;
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
            if(!gotDamage)
            {
                scoreMultiplier += 1;
                Message<ScoreMultiplierChanged>.Raise(new ScoreMultiplierChanged(scoreMultiplier, 1));
            }
            gotDamage = false;
            canMove = false;
            vulnerable = false;
            body.velocity = Vector2.zero;
            //body.isKinematic = true;
        }

        void ApplyPowerup(PickupPowerup powerup)
        {
            var p = powerup.value;
            PromptText prompt;
            switch(p.type)
            {
                case PowerupType.Health:
                    //print("heal:" + p.effectStrength.ToString("00.00")); 
                    //print(currentHealth);
                    currentHealth = Mathf.Clamp(currentHealth + p.effectStrength, 0f, maxHealth);
                    //print(currentHealth);
                    Message<PlayerDamaged>.Raise(new PlayerDamaged() { newHealth = currentHealth, damage = 0f });
                    prompt = $"+ Health";
                    break;
                case PowerupType.Projectile:
                    secondaryProjectile = p.projectile;
                    secondaryAmmo = (int)p.effectStrength;
                    prompt = $"+ Special Ammo";
                    break;
                case PowerupType.Speed:
                    speed += p.effectStrength;
                    StartCoroutine(RemoveSpeed(p.effectStrength, p.effectLength, p.tint));
                    prompt = $"+ Speed";
                    break;
                case PowerupType.Firerate:
                    attackRate /= p.effectStrength;
                    StartCoroutine(RemoveFireBoost(p.effectStrength, p.effectLength, p.tint));
                    prompt = $"+ Firerate";
                    break;
                case PowerupType.Invincibility:
                    StartCoroutine(DoInvincible(p.effectLength, p.tint));
                    prompt = $"+ {p.effectLength.ToString("0.0")}s Invincibility";
                    break;
                default:
                    prompt = "";
                    break;
            }
            prompt.color = powerup.value.tint;
            Message<PromptText>.Raise(prompt);
        }

        IEnumerator RemoveSpeed(float strength, float length, Color tint)
        {
            yield return new WaitForSeconds(length);
            speed -= strength;
            PromptText prompt = "- Speed";
            prompt.color = tint;
            Message<PromptText>.Raise(prompt);
        }
        IEnumerator RemoveFireBoost(float boost, float time, Color tint)
        {
            yield return new WaitForSeconds(time);
            attackRate *= boost;
            PromptText prompt = "- Firerate";
            prompt.color = tint;
            Message<PromptText>.Raise(prompt);
        }
        IEnumerator DoInvincible(float time, Color tint)
        {
            StopCoroutine("DoInvulnerab");
            vulnerable = false;
            yield return new WaitForSeconds(time);
            vulnerable = true;
            PromptText prompt = "- Invincibility";
            prompt.color = tint;
            Message<PromptText>.Raise(prompt);
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
        }

        private void Update()
        {
            Shader.SetGlobalVector("_PlayerPos", transform.position);

            if (currentHealth <= 0 || !canMove)
                return;
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                gamePaused ^= true; //hah, inverted!
                if (gamePaused)
                    Message<PauseGame>.Raise(new PauseGame());
                else
                    Message<ContinueGame>.Raise(new ContinueGame());
            }
            if (gamePaused)
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
            //score decrease
            actualScore = Mathf.Clamp(actualScore - 5, 0, 999999);
            CurrentScore = Mathf.RoundToInt(actualScore);
            //yeet 
            Message<ScoreMultiplierChanged>.Raise(new ScoreMultiplierChanged(1.0f, 1f - scoreMultiplier));
            scoreMultiplier = 1.0f;
            gotDamage = true;

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
            actualScore += enemy.value.PointValue * scoreMultiplier;
            CurrentScore = Mathf.RoundToInt(actualScore);

            enemiesKilled++;
            if(enemiesKilled >= enemiesForMulGain)
            {
                enemiesKilled = 0;
                scoreMultiplier += scoreMulGain;
                Message<ScoreMultiplierChanged>.Raise(new ScoreMultiplierChanged(scoreMultiplier, scoreMulGain));
            }

            //update score, then update UI
            scoreDisplay.text = CurrentScore.ToScoreString(scoreLength);
        }
        
        internal static void ResetScore()
            => CurrentScore = 0;
    }
}