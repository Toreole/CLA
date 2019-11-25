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
        public float pointValue;
        public float speed;
        public float health;
        public float attackRate;
        public MovementType movePattern;
        public float preferredStrafeDistance;
        public Projectile projectile;
    }

    public enum MovementType
    {
        FollowPlayer, Strafe, Stationary
    }
}