using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace LATwo
{
    public class CRT : MonoBehaviour
    {
        [SerializeField]
        protected Material material;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, material);
        }
    }
}