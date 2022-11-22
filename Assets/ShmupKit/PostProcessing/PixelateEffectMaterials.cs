using UnityEngine;

namespace ShmupKit.PostProcessing
{
    [CreateAssetMenu(fileName = "PixelateEffectMaterials", menuName = "ShmupKit/PostProcessing/PixelateEffectMaterials", order = 0)]
    public class PixelateEffectMaterials : ScriptableObject
    {
        public Material PixelateMaterial;
        
        private static PixelateEffectMaterials _instance;
        
        public static PixelateEffectMaterials Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<PixelateEffectMaterials>("PixelateEffectMaterials");
                }
                return _instance;
            }
        }
    }
}