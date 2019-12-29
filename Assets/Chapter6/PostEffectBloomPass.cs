using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Chapter6
{
    public class PostEffectBloomPass : ScriptableRenderPass
    {
        private const string BlurShaderName = "Hidden/SeparableBlur";
        private const string BloomBlitShaderName = "Hidden/BloomBlit";
        private const string AdditiveBlitShaderName = "Hidden/AdditiveBlit";
        private const string Tag = "PostEffectBloom";
        private RenderTargetIdentifier currentTarget;
        private Material bloomBlitMaterial;
        private Material additiveBlitMaterial;
        private int blurTempRT1;
        private int blurTempRT2;
        private Material blurMaterial;
        private PostEffectBloomPassFeature.FeatureSettings settings;

        public PostEffectBloomPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            bloomBlitMaterial = CoreUtils.CreateEngineMaterial(BloomBlitShaderName);
            additiveBlitMaterial = CoreUtils.CreateEngineMaterial(AdditiveBlitShaderName);

            blurTempRT1 = Shader.PropertyToID("blurTempRT1");
            blurTempRT2 = Shader.PropertyToID("blurTempRT2");

            blurMaterial = CoreUtils.CreateEngineMaterial(BlurShaderName);
            blurMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        public void SetSettings(PostEffectBloomPassFeature.FeatureSettings settings)
        {
            this.settings = settings;
        }

        public void SetRenderTarget(RenderTargetIdentifier target)
        {
            currentTarget = target;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(Tag);
            using (new ProfilingSample(cmd, Tag))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var cameraData = renderingData.cameraData;
                var w = cameraData.camera.scaledPixelWidth / 2;
                var h = cameraData.camera.scaledPixelHeight / 2;
                cmd.GetTemporaryRT(blurTempRT1, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
                cmd.GetTemporaryRT(blurTempRT2, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);

                bloomBlitMaterial.SetFloat("_BloomThreshold", RenderingDataManager.Instance.BloomThreshold);
                cmd.Blit(RenderingDataManager.Instance.BloomMarkTextureId, blurTempRT1, bloomBlitMaterial);

                SetupCommandBufferBlur(cmd, w, h);

                additiveBlitMaterial.SetFloat("_Amount", RenderingDataManager.Instance.BloomAmount);
                cmd.Blit(blurTempRT1, currentTarget, additiveBlitMaterial);

                cmd.ReleaseTemporaryRT(blurTempRT1);
                cmd.ReleaseTemporaryRT(blurTempRT2);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        private void SetupCommandBufferBlur(CommandBuffer cmd, int w, int h)
        {
            cmd.SetGlobalVector("offsets", new Vector4(settings.blurX * 2.0f / w, 0, 0, 0));
            cmd.Blit(blurTempRT1, blurTempRT2, blurMaterial);
            cmd.SetGlobalVector("offsets", new Vector4(0, settings.blurY * 2.0f / h, 0, 0));
            cmd.Blit(blurTempRT2, blurTempRT1, blurMaterial);
            cmd.SetGlobalVector("offsets", new Vector4(settings.blurX * 4.0f / w, 0, 0, 0));
            cmd.Blit(blurTempRT1, blurTempRT2, blurMaterial);
            cmd.SetGlobalVector("offsets", new Vector4(0, settings.blurY * 4.0f / h, 0, 0));
            cmd.Blit(blurTempRT2, blurTempRT1, blurMaterial);
        }
    }
}
