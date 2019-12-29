using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Chapter5
{
    public class MultiPass : ScriptableRenderPass
    {
        private const string Tag = "MultiPass";
        private RenderTargetIdentifier currentTarget;
        private List<ShaderTagId> shaderTagIdList = new List<ShaderTagId>();
        private FilteringSettings filteringSettings;

        public MultiPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            shaderTagIdList.Add(new ShaderTagId("Pass1"));
            shaderTagIdList.Add(new ShaderTagId("Pass2"));
            shaderTagIdList.Add(new ShaderTagId("Pass3"));
            filteringSettings = new FilteringSettings(RenderQueueRange.opaque, 1 << LayerMask.NameToLayer("Chapter5"));
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
                var cam = renderingData.cameraData.camera;
                var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, sortFlags);

                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
