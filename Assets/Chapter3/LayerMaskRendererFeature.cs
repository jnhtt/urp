using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Chapter3
{
    public class LayerMaskRendererFeature : ScriptableRendererFeature
    {
        private LayerMaskRenderPass currentPass;

        public override void Create()
        {
            if (currentPass == null)
            {
                currentPass = new LayerMaskRenderPass();
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            currentPass.SetRenderTarget(renderer.cameraColorTarget);
            renderer.EnqueuePass(currentPass);
        }
    }
}
