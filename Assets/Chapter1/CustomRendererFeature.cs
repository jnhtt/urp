using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Chapter1
{
    public class CustomRendererFeature : ScriptableRendererFeature
    {
        private CustomRenderPass currentPass;

        public override void Create()
        {
            if (currentPass == null)
            {
                currentPass = new CustomRenderPass();
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            Debug.Log("AddRenderPasses");
            currentPass.SetRenderTarget(renderer.cameraColorTarget);
            renderer.EnqueuePass(currentPass);
        }
    }
}
