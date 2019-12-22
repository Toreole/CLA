using UnityEngine;
using TMPro;

namespace LATwo
{
    public class ScoreMultiplierUI : MonoBehaviour
    {
        [SerializeField]
        protected Animator anim;
        [SerializeField]
        protected string trigger;
        [SerializeField]
        protected TextMeshProUGUI field;

        private void OnEnable()
        {
            Message<ScoreMultiplierChange>.Add(OnScoreMulChange);
        }

        private void OnDisable()
        {
            Message<ScoreMultiplierChange>.Remove(OnScoreMulChange);
        }

        void OnScoreMulChange(ScoreMultiplierChange change)
        {
            if (Mathf.Abs(change.delta) < 0.1f)
                return;
            anim.SetTrigger(trigger);
            field.text = change.multiplier.ToString("0.0") + "x";
        }
    }
}
