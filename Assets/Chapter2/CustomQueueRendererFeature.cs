using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Chapter2
{
    public class CustomQueueRendererFeature : ScriptableRendererFeature
    {
        private CustomQueueRenderPass currentPass;

        public override void Create()
        {
            if (currentPass == null)
            {
                currentPass = new CustomQueueRenderPass();
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            currentPass.SetRenderTarget(renderer.cameraColorTarget);
            renderer.EnqueuePass(currentPass);
        }
    }
}
