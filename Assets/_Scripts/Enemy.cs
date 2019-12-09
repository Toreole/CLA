using UnityEngine;

namespace LATwo
{
    /// <summary>
    /// SO that defines any given enemy
    /// </summary>
    [CreateAssetMenu(menuName = "Game_SO/EnemySettings")]
    public class Enemy : ScriptableObject
    {
        public Sprite sprite;
        public Color tint;
        public int pointValue;
        public float speed;
        public float health;
        public float attackRate;
        public float attackRange;
        public MovementType movePattern;
        public float preferredStrafeDistance;
        public float strafeTolerance = 0.5f;
        public float strafeSpeed;
        public Projectile projectile; 
    }

    public enum MovementType
    {
        FollowPlayer, Strafe, Stationary
    }
}