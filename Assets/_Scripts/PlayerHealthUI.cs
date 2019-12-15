using UnityEngine;
using System.Collections;

namespace LATwo
{
    public class PlayerHealthUI : MonoBehaviour
    {
        [SerializeField]
        protected UnityEngine.UI.Slider healthSlider;

        private void OnEnable()
        {
            Message<PlayerDamaged>.Add(UpdateSlider);
        }

        private void OnDisable()
        {
            Message<PlayerDamaged>.Remove(UpdateSlider);
        }

        void UpdateSlider(PlayerDamaged dmg)
        {
            healthSlider.value = dmg.newHealth;
            //print("update slider:" + dmg.newHealth);
        }
    }
}