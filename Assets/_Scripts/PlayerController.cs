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
            fire = "LeftClick";
        [SerializeField]
        protected float maxHealth = 100f, positiveSpeed = 5f, negativeSpeed = 3f;
        [SerializeField]
        protected Projectile projectile;
        [SerializeField]
        protected float attackRate = 0.4f;
        [SerializeField]
        protected Camera cam;

        protected Vector2 mouseDir, movementInput;
        protected float lastShotTime; 
        
        //TODO!: think about power-ups
        
        protected void Start()
        {
            currentHealth = maxHealth;
        }

        protected override void Die()
        {
            //FAILED THE GAME REEEEEEEEEEEEEEEEEEEEEEEEEEEEE
        }

        private void FixedUpdate()
        {
            //target movement for this frame.
            var dot = Vector2.Dot(mouseDir, movementInput);
            movementInput *= (dot >= 0 ? positiveSpeed : negativeSpeed);
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
        }
    }
}