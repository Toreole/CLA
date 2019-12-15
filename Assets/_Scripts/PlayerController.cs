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

        protected Vector2 mouseDir, movementInput;
        protected float lastShotTime, lastSecondaryShot;
        //secondary stuff is given by powerups.
        protected Projectile secondaryProjectile;
        protected int secondaryAmmo;
        
        //TODO!: think about power-ups
        
        protected void Start()
        {
            currentHealth = maxHealth;
        }

        private void OnEnable()
        {
            Message<PickupPowerup>.Add(ApplyPowerup);
        }

        private void OnDisable()
        {
            Message<PickupPowerup>.Remove(ApplyPowerup);
        }

        void ApplyPowerup(PickupPowerup powerup)
        {
            var p = powerup.value;
            switch(p.type)
            {
                case PowerupType.Health:
                    currentHealth = Mathf.Clamp(currentHealth + p.effectStrength, 0f, maxHealth);
                    Message<PlayerDamaged>.Raise(new PlayerDamaged() { newHealth = currentHealth, damage = -currentHealth });
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
            //FAILED THE GAME REEEEEEEEEEEEEEEEEEEEEEEEEEEEE
        }

        private void FixedUpdate()
        {
            //target movement for this frame.
            movementInput *= speed;
            body.velocity = movementInput;

            //update position at the end of this fixedupdate lol
            Position = body.position;
        }

        private void Update()
        {
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
    }

    public struct PlayerDamaged
    {
        public float newHealth;
        public float damage;
    }
}