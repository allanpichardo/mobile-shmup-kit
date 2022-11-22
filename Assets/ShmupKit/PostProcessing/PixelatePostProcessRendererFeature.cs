using System;
using UnityEngine.Rendering.Universal;

namespace ShmupKit.PostProcessing
{
    [Serializable]
    public class PixelatePostProcessRendererFeature : ScriptableRendererFeature
    {
        private PixelateRenderPass pass;
        
        public override void Create()
        {
            pass = new PixelateRenderPass();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(pass);
        }
    }
}