using UnityEngine;
using System.Collections;

namespace LATwo
{
    public class PowerupBehaviour : MonoBehaviour
    {
        [SerializeField]
        protected string playerTag = "Player";
        [SerializeField]
        protected new SpriteRenderer renderer;

        protected Powerup powerup;
        public Powerup Powerup
        {
            get => powerup;
            set
            {
                transform.localScale = Vector3.one;
                powerup = value;
                gameObject.SetActive(true);
                renderer.sprite = powerup.sprite;
                renderer.color = powerup.tint;
                StartCoroutine(DeactivateLater());
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //print("ree");
            if (collision.CompareTag(playerTag))
            {
                StopAllCoroutines();
                Message<PickupPowerup>.Raise(powerup);
                Message<ReturnToPool<PowerupBehaviour>>.Raise(this);
            }
        }

        IEnumerator DeactivateLater()
        {
            yield return new WaitForSeconds(powerup.lifeTime);
            for(float t = 0; t < 0.75f; t += Time.deltaTime)
            {
                transform.localScale = Vector3.one * (1f - t / 0.75f);
                yield return null;
            }
            Message<ReturnToPool<PowerupBehaviour>>.Raise(this);
        }
    }
}