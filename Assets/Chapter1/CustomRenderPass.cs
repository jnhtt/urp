using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Chapter1
{
    public class CustomRenderPass : ScriptableRenderPass
    {
        private const string Tag = "CustomRenderPass";
        private RenderTargetIdentifier currentTarget;
        private List<ShaderTagId> shaderTagIdList = new List<ShaderTagId>();
        private FilteringSettings filteringSettings;

        public CustomRenderPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            shaderTagIdList.Add(new ShaderTagId("CustomChapter1"));
            filteringSettings = new FilteringSettings(new RenderQueueRange(RenderQueueRange.minimumBound, RenderQueueRange.maximumBound), -1);
        }

        public void SetRenderTarget(RenderTargetIdentifier target)
        {
            currentTarget = target;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            ScriptableCullingParameters cullingParameters = new ScriptableCullingParameters();
            renderingData.cameraData.camera.TryGetCullingParameters(out cullingParameters);
            var cullResults = context.Cull(ref cullingParameters);

            CommandBuffer cmd = CommandBufferPool.Get(Tag);
            using (new ProfilingSample(cmd, Tag))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                var cam = renderingData.cameraData.camera;
                var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, sortFlags);

                context.DrawRenderers(cullResults, ref drawSettings, ref filteringSettings);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
