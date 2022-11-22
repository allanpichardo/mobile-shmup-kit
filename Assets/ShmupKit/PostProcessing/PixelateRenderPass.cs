using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ShmupKit.PostProcessing
{
    [Serializable]
    public class PixelateRenderPass : ScriptableRenderPass
    {
        // Used to render from camera to post processings
	    // back and forth, until we render the final image to
	    // the camera
        RenderTargetIdentifier source;
        RenderTargetIdentifier destinationA;
        RenderTargetIdentifier destinationB;
        RenderTargetIdentifier latestDest;

        readonly int temporaryRTIdA = Shader.PropertyToID("_TempRT");
        readonly int temporaryRTIdB = Shader.PropertyToID("_TempRTB");
        private static readonly int ScreenWidth = Shader.PropertyToID("_ScreenWidth");
        private static readonly int ScreenHeight = Shader.PropertyToID("_ScreenHeight");
        private static readonly int CellSizeX = Shader.PropertyToID("_CellSizeX");
        private static readonly int CellSizeY = Shader.PropertyToID("_CellSizeY");

        public PixelateRenderPass()
        {
            // Set the render pass event
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // Grab the camera target descriptor. We will use this when creating a temporary render texture.
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;

            var renderer = renderingData.cameraData.renderer;
            source = renderer.cameraColorTarget;

            // Create a temporary render texture using the descriptor from above.
            cmd.GetTemporaryRT(temporaryRTIdA , descriptor, FilterMode.Bilinear);
            destinationA = new RenderTargetIdentifier(temporaryRTIdA);
            cmd.GetTemporaryRT(temporaryRTIdB , descriptor, FilterMode.Bilinear);
            destinationB = new RenderTargetIdentifier(temporaryRTIdB);
        }
        
        // The actual execution of the pass. This is where custom rendering occurs.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
    	    // Skipping post processing rendering inside the scene View
            if(renderingData.cameraData.isSceneViewCamera)
                return;
            
            // Here you get your materials from your custom class
            // (It's up to you! But here is how I did it)
            var materials = PixelateEffectMaterials.Instance;
            if (materials == null)
            {
                Debug.LogError("Pixelate Effect Post Processing Materials instance is null");
                return;
            }
            
            CommandBuffer cmd = CommandBufferPool.Get("Custom Post Processing");
            cmd.Clear();

		    // This holds all the current Volumes information
		    // which we will need later
            var stack = VolumeManager.instance.stack;

            #region Local Methods

		    // Swaps render destinations back and forth, so that
		    // we can have multiple passes and similar with only a few textures
            void BlitTo(Material mat, int pass = 0)
            {
                var first = latestDest;
                var last = first == destinationA ? destinationB : destinationA;
                Blit(cmd, first, last, mat, pass);

                latestDest = last;
            }

            #endregion

		    // Starts with the camera source
            latestDest = source;

            //---Custom effect here---
            var customEffect = stack.GetComponent<Pixelate>();
            // Only process if the effect is active
            if (customEffect.IsActive())
            {
                var material = materials.PixelateMaterial;
                material.SetFloat(ScreenWidth, renderingData.cameraData.camera.pixelWidth);
                material.SetFloat(ScreenHeight, renderingData.cameraData.camera.pixelHeight);
                
                float ratio = (float)renderingData.cameraData.camera.pixelWidth / (float)renderingData.cameraData.camera.pixelHeight;
                material.SetFloat(CellSizeY, customEffect.pixelScale.value * ratio);
                material.SetFloat(CellSizeX, customEffect.pixelScale.value);
                
                BlitTo(material);
            }
            
		    // DONE! Now that we have processed all our custom effects, applies the final result to camera
            Blit(cmd, latestDest, source);
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

	    //Cleans the temporary RTs when we don't need them anymore
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(temporaryRTIdA);
            cmd.ReleaseTemporaryRT(temporaryRTIdB);
        }
    }
}