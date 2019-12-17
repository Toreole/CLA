using UnityEngine;
using System.Collections;

namespace LATwo
{
    public class PulseEffect : MonoBehaviour
    {
        [SerializeField]
        protected Material mat;

        private void Start()
        {
            //this is temporary for testing.
            mat.SetFloatArray("_Impacts", new float[] { 1f, 1.5f, 2f, 2.5f, 3f }); //testing
            mat.SetVectorArray("_ImpactPoints", new Vector4[] { Vector4.one, Vector4.zero, new Vector4(0.5f, 0.5f), new Vector4(0, 0.3f), new Vector4(0.3f, 0.6f) });
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, mat);
        }

    }
}