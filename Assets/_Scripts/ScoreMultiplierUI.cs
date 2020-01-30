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
            Message<ScoreMultiplierChanged>.Add(OnScoreMulChange);
        }

        private void OnDisable()
        {
            Message<ScoreMultiplierChanged>.Remove(OnScoreMulChange);
        }

        void OnScoreMulChange(ScoreMultiplierChanged change)
        {
            if (Mathf.Abs(change.delta) < 0.1f)
                return;
            anim.SetTrigger(trigger);
            field.text = change.multiplier.ToString("0.0") + "x";
        }
    }
}
