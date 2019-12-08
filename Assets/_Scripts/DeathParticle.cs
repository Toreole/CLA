using UnityEngine;
using System.Collections;

namespace LATwo
{
    public class DeathParticle : MonoBehaviour
    {
        [SerializeField]
        protected ParticleSystem system;

        public ParticleSystem.MainModule Main => system.main;

        void OnEnable()
        {
            StartCoroutine(DisableLater());
        }

        IEnumerator DisableLater()
        {
            yield return new WaitForSeconds(2.1f);
            Message<ReturnToPool<DeathParticle>>.Raise(this); //ok
        }
    }
}