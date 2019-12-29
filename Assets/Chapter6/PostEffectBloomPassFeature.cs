using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Chapter6
{
    public class PostEffectBloomPassFeature : ScriptableRendererFeature
    {
        [Serializable]
        public class FeatureSettings
        {
            public float blurX = 1f;
            public float blurY = 1f;
        }


        [SerializeField]
        private FeatureSettings settings;

        private PostEffectBloomPass currentPass;

        public override void Create()
        {
            if (currentPass == null)
            {
                currentPass = new PostEffectBloomPass();
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            currentPass.SetRenderTarget(renderer.cameraColorTarget);
            currentPass.SetSettings(settings);
            renderer.EnqueuePass(currentPass);
        }
    }
}
