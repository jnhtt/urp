using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Chapter6
{
    public class BloomMarkPassFeature : ScriptableRendererFeature
    {
        public const int BloomMarkRenderingMaskLayer = 21;
        private BloomMarkPass currentPass;

        public override void Create()
        {
            if (currentPass == null)
            {
                currentPass = new BloomMarkPass();
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(currentPass);
        }
    }
}
