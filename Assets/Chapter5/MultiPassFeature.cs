using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Chapter5
{
    public class MultiPassFeature : ScriptableRendererFeature
    {
        private MultiPass currentPass;

        public override void Create()
        {
            if (currentPass == null)
            {
                currentPass = new MultiPass();
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            currentPass.SetRenderTarget(renderer.cameraColorTarget);
            renderer.EnqueuePass(currentPass);
        }
    }
}
