using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Chapter6
{
    public class BloomMarkPass : ScriptableRenderPass
    {
        public static readonly Color CLEAR = new Color(0, 0, 0, 0);
        private const string Tag = "BloomMark";
        private static string ZWriteShaderName = "Hidden/InternalZWriteOnly";
        private static ShaderTagId ObstacleShaderTagId = new ShaderTagId("UniversalForward");
        private static ShaderTagId BloomMarkShaderTagId = new ShaderTagId("BloomMark");
        private FilteringSettings obstacleFilteringSettings;
        private FilteringSettings bloomFilteringSettings;
        private RenderTargetHandle bloomMarkRenderTextureHandle;
        private Material zwriteMaterial;

        public BloomMarkPass()
        {
            uint bloomLayerMask = 1 << BloomMarkPassFeature.BloomMarkRenderingMaskLayer;
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            bloomFilteringSettings = new FilteringSettings(RenderQueueRange.opaque, -1, bloomLayerMask);
            obstacleFilteringSettings = new FilteringSettings(RenderQueueRange.opaque, -1, uint.MaxValue ^ bloomLayerMask);

            zwriteMaterial = CoreUtils.CreateEngineMaterial(ZWriteShaderName);
            bloomMarkRenderTextureHandle.Init(Tag);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(bloomMarkRenderTextureHandle.id, cameraTextureDescriptor);
            RenderingDataManager.Instance.SetBloomMarkTextureId(bloomMarkRenderTextureHandle.id);

            ConfigureTarget(bloomMarkRenderTextureHandle.Identifier());
            ConfigureClear(ClearFlag.All, CLEAR);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(Tag);
            using (new ProfilingSample(cmd, Tag))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var cam = renderingData.cameraData.camera;
                var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(ObstacleShaderTagId, ref renderingData, sortFlags);
                drawSettings.overrideMaterial = zwriteMaterial;
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref obstacleFilteringSettings);

                drawSettings = CreateDrawingSettings(BloomMarkShaderTagId, ref renderingData, sortFlags);
                drawSettings.overrideMaterial = null;
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref bloomFilteringSettings);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            base.FrameCleanup(cmd);
            cmd.ReleaseTemporaryRT(bloomMarkRenderTextureHandle.id);
        }
    }
}
