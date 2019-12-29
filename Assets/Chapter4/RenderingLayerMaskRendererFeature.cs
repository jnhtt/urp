using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Chapter4
{
    public class RenderingLayerMaskRendererFeature : ScriptableRendererFeature
    {
        private RenderingLayerMaskRenderPass currentPass;

        public override void Create()
        {
            if (currentPass == null)
            {
                currentPass = new RenderingLayerMaskRenderPass();
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            currentPass.SetRenderTarget(renderer.cameraColorTarget);
            renderer.EnqueuePass(currentPass);
        }
    }
}
