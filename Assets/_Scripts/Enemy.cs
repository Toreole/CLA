using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

        //Move patterns depend on the SO.
        public MovementType movePattern;
        public float preferredStrafeDistance;
        public float strafeTolerance = 0.5f;
        public float strafeSpeed;
        public Projectile projectile;
        public int chainLength;
    }

    public enum MovementType
    {
        FollowPlayer, Strafe, Stationary, Worm
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Enemy))]
    public class EnemyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Enemy e = target as Enemy;
            EditorUtility.SetDirty(e);

            //cluster of all common enemy fields.
            e.sprite = (Sprite) EditorGUILayout.ObjectField("Sprite", e.sprite, typeof(Sprite), false);
            e.tint = EditorGUILayout.ColorField("Tint", e.tint);
            e.pointValue = EditorGUILayout.IntField("Score Value", e.pointValue);
            e.projectile = (Projectile)EditorGUILayout.ObjectField("Projectile", e.projectile, typeof(Projectile), false);
            e.health = EditorGUILayout.FloatField("health", e.health);
            e.attackRate = EditorGUILayout.FloatField("Attack Rate", e.attackRate);
            e.attackRange = EditorGUILayout.FloatField("Attack Range", e.attackRange);

            e.movePattern = (MovementType) EditorGUILayout.EnumPopup("Movement Pattern", e.movePattern);

            switch(e.movePattern)
            {
                case MovementType.FollowPlayer:
                    e.speed = EditorGUILayout.FloatField("Move Speed", e.speed);
                    break;
                case MovementType.Stationary:
                    break;
                case MovementType.Strafe:
                    e.speed = EditorGUILayout.FloatField("Non-Strafe Speed", e.speed);
                    e.preferredStrafeDistance = EditorGUILayout.FloatField("Strafe Distance", e.preferredStrafeDistance);
                    e.strafeTolerance = EditorGUILayout.FloatField("Strafe Tolerance", e.strafeTolerance);
                    e.strafeSpeed = EditorGUILayout.FloatField("Angular SPeed", e.strafeSpeed);
                    break;
                case MovementType.Worm:
                    e.chainLength = EditorGUILayout.IntField("Worm Length", e.chainLength);
                    break;
            }
        }
    }
#endif
}