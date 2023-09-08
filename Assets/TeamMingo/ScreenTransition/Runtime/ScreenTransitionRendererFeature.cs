using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TeamMingo.ScreenTransition.Runtime
{
  public class ScreenTransitionRendererFeature : ScriptableRendererFeature
  {
      
      public enum BufferType
      {
          CameraColor
      }
      
    class RenderPass : ScriptableRenderPass
    {
        public FilterMode filterMode { get; set; }
        public Settings settings;

        RenderTargetIdentifier source;
        RenderTargetIdentifier destination;
        int temporaryRTId = Shader.PropertyToID("_TempRT");

        int sourceId;
        int destinationId;
        bool isSourceAndDestinationSameTarget;

        string m_ProfilerTag;

        public RenderPass(string tag)
        {
            m_ProfilerTag = tag;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            blitTargetDescriptor.depthBufferBits = 0;

            isSourceAndDestinationSameTarget = settings.sourceType == settings.destinationType &&
                (settings.sourceType == BufferType.CameraColor || settings.sourceTextureId == settings.destinationTextureId);

            var renderer = renderingData.cameraData.renderer;

            if (settings.sourceType == BufferType.CameraColor)
            {
                sourceId = -1;
                source = renderer.cameraColorTarget;
            }
            else
            {
                sourceId = Shader.PropertyToID(settings.sourceTextureId);
                cmd.GetTemporaryRT(sourceId, blitTargetDescriptor, filterMode);
                source = new RenderTargetIdentifier(sourceId);
            }

            if (isSourceAndDestinationSameTarget)
            {
                destinationId = temporaryRTId;
                cmd.GetTemporaryRT(destinationId, blitTargetDescriptor, filterMode);
                destination = new RenderTargetIdentifier(destinationId);
            }
            else if (settings.destinationType == BufferType.CameraColor)
            {
                destinationId = -1;
                destination = renderer.cameraColorTarget;
            }
            else
            {
                destinationId = Shader.PropertyToID(settings.destinationTextureId);
                cmd.GetTemporaryRT(destinationId, blitTargetDescriptor, filterMode);
                destination = new RenderTargetIdentifier(destinationId);
            }
        }

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

            // Can't read and write to same color target, create a temp render target to blit. 
            if (isSourceAndDestinationSameTarget)
            {
                Blit(cmd, source, destination, settings.blitMaterial, settings.blitMaterialPassIndex);
                Blit(cmd, destination, source);
            }
            else
            {
                Blit(cmd, source, destination, settings.blitMaterial, settings.blitMaterialPassIndex);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        /// <inheritdoc/>
        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (destinationId != -1)
                cmd.ReleaseTemporaryRT(destinationId);

            if (source == destination && sourceId != -1)
                cmd.ReleaseTemporaryRT(sourceId);
        }
    }

    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        public Material blitMaterial = null;
        public int blitMaterialPassIndex = -1;
        public BufferType sourceType = BufferType.CameraColor;
        public BufferType destinationType = BufferType.CameraColor;
        public string sourceTextureId = "_SourceTexture";
        public string destinationTextureId = "_DestinationTexture";
    }

    public Settings settings = new Settings();
    private RenderPass _blitPass;

    public override void Create()
    {
        _blitPass = new RenderPass(name);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.blitMaterial == null)
        {
            return;
        }

        _blitPass.renderPassEvent = settings.renderPassEvent;
        _blitPass.settings = settings;
        renderer.EnqueuePass(_blitPass);
    }
  }
}