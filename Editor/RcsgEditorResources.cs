// * * * * * * * * * * * * * * * * * * * * * *
//  Description:
//
// * * * * * * * * * * * * * * * * * * * * * *

using JetBrains.Annotations;
using UnityEngine;

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

        public Material defaultWallMaterial;
        public Material defaultFloorMaterial;
        public Material defaultWindowMaterial;
        public Material defaultMetalMaterial;

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

        public Shader backgroundShader;
        public Shader coloredPolygonShader;
        public Shader customDotShader;
        public Shader customNoDepthSurfaceShader;
        public Shader customSurfaceShader;
        public Shader customThickWireShader;
        public Shader customWireShader;
        public Shader customWireDottedShader;
        public Shader genericLineShader;
        public Shader gridShader;
        public Shader noZTestGenericLineShader;
        public Shader orthoGridShader;
        public Shader specialSurfaceShader;
        public Shader transparentSpecialSurfaceShader;
        public Shader zTestGenericLineShader;

        private static RcsgEditorResources m_Instance;
        private static Material m_MissingErrorMaterial;

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

        private static string CurrentRenderPipeline
        {
            get => RenderPipelineHelper.GetCurrentPipeline(out Shader _);
        }

        public static Material DefaultOpaqueMaterial
        {
            get
            {
                return CurrentRenderPipeline switch
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
                return CurrentRenderPipeline switch
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
                return CurrentRenderPipeline switch
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
                return CurrentRenderPipeline switch
                {
                    RenderPipelineHelper.URP_TAG => "_DetailAlbedoMap"
                  , _                            => "_DetailMap"
                };
            }
        }

        public static Material MissingErrorMaterial
        {
            get
            {
                if(m_MissingErrorMaterial == null)
                {
                    m_MissingErrorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
                }

                return m_MissingErrorMaterial;
            }
        }
    }
}
