using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ShmupKit.PostProcessing
{
    
    [Serializable, VolumeComponentMenuForRenderPipeline("ShmupKit/PostProcessing/Pixelate", typeof(UniversalRenderPipeline))]
    public class Pixelate : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter pixelScale = new ClampedFloatParameter(4.0f, 1.0f, 100f);
        
        public bool IsActive()
        {
            return true;
        }

        public bool IsTileCompatible()
        {
            return true;
        }
    }
}