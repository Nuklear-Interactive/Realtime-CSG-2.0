// * * * * * * * * * * * * * * * * * * * * * *
//  Description:
//
// * * * * * * * * * * * * * * * * * * * * * *

using JetBrains.Annotations;
using UnityEditor.Embree;
using UnityEngine;
using UnityEngine.Serialization;

namespace RealtimeCSG.Foundation
{
    [CreateAssetMenu(menuName = "Realtime-CSG/EditorResources", fileName = "RCSG-Editor-Resources", order = 0)]
    public class RcsgEditorResources : ScriptableObject
    {
        [Header("Runtime Resources")]
        [UsedImplicitly] public Texture2D defaultWallTexture;

        [UsedImplicitly] public Texture2D defaultFloorTexture;
        [UsedImplicitly] public Texture2D defaultWindowTexture;
        [UsedImplicitly] public Texture2D defaultCheckerTexture;
        [UsedImplicitly] public Texture2D defaultDetailCheckerTexture;
        [UsedImplicitly] public Texture2D defaultMetalnessTexture;
        [UsedImplicitly] public Texture2D defaultRoughnessTexture;

        [UsedImplicitly] public Material templateUrpOpaqueMaterial;
        [UsedImplicitly] public Material templateUrpTransparentMaterial;
        [UsedImplicitly] public Material templateBirpOpaqueMaterial;
        [UsedImplicitly] public Material templateBirpTransparentMaterial;
        [UsedImplicitly] public Material templateHdrpOpaqueMaterial;
        [UsedImplicitly] public Material templateHdrpTransparentMaterial;

        [Header("Editor Resources")]
        [UsedImplicitly] public Texture2D castShadowsTexture;

        [UsedImplicitly] public Texture2D colliderTexture;
        [UsedImplicitly] public Texture2D culledTexture;
        [UsedImplicitly] public Texture2D discardedTexture;
        [UsedImplicitly] public Texture2D hiddenTexture;
        [UsedImplicitly] public Texture2D invisibleTexture;
        [UsedImplicitly] public Texture2D receiveShadowsTexture;
        [UsedImplicitly] public Texture2D shadowOnlyTexture;
        [UsedImplicitly] public Texture2D triggerTexture;

        private static RcsgEditorResources m_Instance;

        public static RcsgEditorResources Instance
        {
            get
            {
                if(m_Instance == null)
                {
                    //Debug.Log("Realtime-CSG Extensions editor resources aren't loaded, attempting to load them.");
                    m_Instance = Resources.Load<RcsgEditorResources>("RealtimeCSG/Editor/RCSG-Editor-Resources");
                }

                return m_Instance;
            }
        }

        public static Material DefaultOpaqueMaterial
        {
            get
            {
                string pipeline = RenderPipelineHelper.GetCurrentPipeline(out Shader _);

                return pipeline switch
                {
                    RenderPipelineHelper.URP_TAG  => Instance.templateUrpOpaqueMaterial
                  , RenderPipelineHelper.HDRP_TAG => Instance.templateHdrpOpaqueMaterial
                  , _                             => Instance.templateBirpOpaqueMaterial
                };
            }
        }

        public static Material DefaultTransparentMaterial
        {
            get
            {
                string pipeline = RenderPipelineHelper.GetCurrentPipeline(out Shader _);

                return pipeline switch
                {
                    RenderPipelineHelper.URP_TAG  => Instance.templateUrpTransparentMaterial
                  , RenderPipelineHelper.HDRP_TAG => Instance.templateHdrpTransparentMaterial
                  , _                             => Instance.templateBirpTransparentMaterial
                };
            }
        }

        public static string MetallicMapName
        {
            get
            {
                string pipeline = RenderPipelineHelper.GetCurrentPipeline(out Shader _);

                return pipeline switch
                {
                    RenderPipelineHelper.HDRP_TAG => "_MaskMap"
                  , _                             => "_MetallicGlossMap"
                };
            }
        }

        public static string DetailAlbedoMapName
        {
            get
            {
                string pipeline = RenderPipelineHelper.GetCurrentPipeline(out Shader _);

                return pipeline switch
                {
                    RenderPipelineHelper.URP_TAG => "_DetailAlbedoMap"
                  , _                            => "_DetailMap"
                };
            }
        }
    }
}
