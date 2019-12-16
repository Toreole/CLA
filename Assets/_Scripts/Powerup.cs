using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LATwo
{
    [CreateAssetMenu(menuName = "Game_SO/PowerupSettings")]
    public class Powerup : ScriptableObject
    {
        public float lifeTime = 5f;

        //strength: (int) projectile => ammo.
        //(float) health => health restore.
        //(float) speed => bonus speed
        public float effectStrength;

        //length: projectile : ignore
        //health: ignore
        //speed: (l<0 => permanent?), time in seconds
        public float effectLength;

        public Projectile projectile;

        public PowerupType type;

        public Sprite sprite;
        public Color tint;
    }

    public enum PowerupType
    {
        Projectile, Health, Speed
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Powerup))]
    public class PowerupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Powerup pu = target as Powerup;
            EditorUtility.SetDirty(pu); //im actually fucking stupid for forgetting this
            //without the object being setDirty, it wont actually save the data.
            //basic shit first.
            pu.lifeTime = EditorGUILayout.FloatField("Life Time", pu.lifeTime);
            pu.sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", pu.sprite, typeof(Sprite), false);
            pu.tint = EditorGUILayout.ColorField("Tint", pu.tint);

            pu.type = (PowerupType) EditorGUILayout.EnumPopup("Type", pu.type);

            //I dont like the look of switch() but it works.
            switch(pu.type)
            {
                case PowerupType.Health:
                    pu.effectStrength = EditorGUILayout.FloatField("Health Restored", pu.effectStrength);
                    //pu.effectLength = EditorGUILayout.FloatField("Time To Recover", pu.effectLength);
                    break;
                case PowerupType.Projectile:
                    pu.effectStrength = EditorGUILayout.IntField("Ammo", (int)pu.effectStrength);
                    pu.projectile = (Projectile)EditorGUILayout.ObjectField("Projectile", pu.projectile, typeof(Projectile), false);
                    break;
                case PowerupType.Speed:
                    pu.effectLength = EditorGUILayout.FloatField("Speed Time", pu.effectLength);
                    pu.effectStrength = EditorGUILayout.FloatField("Bonus Speed", pu.effectStrength);
                    break;
            }
        }
    }
#endif
}
