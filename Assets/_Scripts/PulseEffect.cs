using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LATwo
{
    public class PulseEffect : MonoBehaviour
    {
        [SerializeField]
        protected Material mat;
        [Header("On Stage Clear")]
        [SerializeField, Range(0.0f, .49f)]
        protected float edgeOffset;
        [SerializeField, Range(0.0f, 1.5f)]
        protected float maxTimeOffset;

        protected Pulse[] pulses = new Pulse[5];

        private void OnEnable()
        {
            Message<StageCleared>.Add(OnStageClear);
        }

        private void OnDisable()
        {
            Message<StageCleared>.Remove(OnStageClear);
        }

        public void OnStageClear(StageCleared st)
        {
            //Generate random pulses.
            for (int i = 0; i < 5; i++)
                pulses[i] = Pulse.GetRandom(edgeOffset, maxTimeOffset);
            mat.SetFloatArray("_Impacts", pulses.GetTimes());
            mat.SetVectorArray("_ImpactPoints", new Vector4[] {
                pulses[0].screenPoint,
                pulses[1].screenPoint,
                pulses[2].screenPoint,
                pulses[3].screenPoint,
                pulses[4].screenPoint
            });
        }

        private void Start()
        {
            //initialize the arrays with something far in the negative time, so they dont happen once the camera is enabled.
            mat.SetFloatArray("_Impacts", new float[] { -100f, -100f, -100f, -100f, -100f }); 
            mat.SetVectorArray("_ImpactPoints", new Vector4[] { Vector4.one, Vector4.one, Vector4.one, Vector4.one, Vector4.one });
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, mat);
        }
    }

    public struct Pulse
    {
        public float time;
        public Vector4 screenPoint;

        public static Pulse GetRandom(float offset, float timeDeviation)
        {
            Pulse p = new Pulse();
            p.time = Time.timeSinceLevelLoad + Random.value * timeDeviation;
            p.screenPoint.x = Mathf.Lerp(offset, 1f - offset, Random.value);
            p.screenPoint.y = Mathf.Lerp(offset, 1f - offset, Random.value);
            return p;
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(PulseEffect))]
    public class PulseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //default editor
            base.OnInspectorGUI();
            //test button.
            if (GUILayout.Button("Test Waves"))
            {
                (target as PulseEffect).OnStageClear(default);
            }
        }
    }
#endif
}