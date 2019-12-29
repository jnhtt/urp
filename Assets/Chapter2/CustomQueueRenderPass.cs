using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Chapter2
{
    public class CustomQueueRenderPass : ScriptableRenderPass
    {
        private const string Tag = "CustomQueueRenderPass";
        private RenderTargetIdentifier currentTarget;
        private List<ShaderTagId> shaderTagIdList = new List<ShaderTagId>();
        private FilteringSettings filteringSettings;

        public CustomQueueRenderPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            shaderTagIdList.Add(new ShaderTagId("UniveralForward"));
            filteringSettings = new FilteringSettings(new RenderQueueRange(2200, 2201), 1 << LayerMask.NameToLayer("Chapter2"));
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
