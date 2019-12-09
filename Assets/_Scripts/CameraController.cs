using UnityEngine;
using System.Collections;

namespace LATwo
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        protected Transform player;
        [SerializeField]
        protected float dmgShakeThreshold = 1f, baseShake = 0.8f, shakeTime = 0.4f;
        [SerializeField]
        protected AnimationCurve shakeStrength;

        protected Vector3 offset;

        private void OnEnable()
        {
            Message<PlayerDamaged>.Add(OnPlayerDamaged);
        }

        private void OnDisable()
        {
            Message<PlayerDamaged>.Remove(OnPlayerDamaged);
        }

        void OnPlayerDamaged(PlayerDamaged dmg)
        {
            if(dmg.newHealth > 0)
            {
                if(dmg.damage >= dmgShakeThreshold)
                {
                    StopAllCoroutines();
                    offset = Vector3.zero;
                    StartCoroutine(DoShake());
                }
            }
        }

        IEnumerator DoShake()
        {
            for (float t = 0; t < shakeTime; t += Time.deltaTime)
            {
                float valT = shakeStrength.Evaluate(t / shakeTime);
                offset = Vector3.Lerp(offset, Random.insideUnitCircle.normalized * baseShake * valT, valT);
                yield return null;
            }
        }

        void Update()
        {
            Vector3 pos = player.position + offset;
            pos.z = transform.position.z;
            transform.position = pos;
        }


    }
}