// * * * * * * * * * * * * * * * * * * * * * *
//  Description:
//
// * * * * * * * * * * * * * * * * * * * * * *

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace RealtimeCSG
{
    public static class MaterialExtensions
    {
        private static readonly int baseMap = Shader.PropertyToID("_BaseMap");
        private static readonly int baseColorMap = Shader.PropertyToID("_BaseColorMap");

        public static string MainTexKeyword(this Material material)
        {
            string pipeline = RenderPipelineHelper.GetCurrentPipeline(out Shader _);

            switch(pipeline)
            {
                case RenderPipelineHelper.URP_TAG:
                {
                    if(material.HasTexture(baseMap))
                    {
                        return "_BaseMap";
                    }

                    break;
                }
                case RenderPipelineHelper.HDRP_TAG:
                {
                    if(material.HasTexture(baseColorMap))
                    {
                        return "_BaseColorMap";
                    }

                    break;
                }
            }

            return "_MainTex";
        }

        public static void SetTransparent(this Material material)
        {
          /*material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

            material.SetOverrideTag("RenderType", "Transparent");
            material.SetOverrideTag("RenderQueue", "Transparent");

            material.SetFloat("_SrcBlend", 1);
            material.SetFloat("_SrcBlendAlpha", 1);
            material.SetFloat("_Surface", 1);*/
        }

        /*public static Material CreateInstance(this Material material)
        {
            EditorMaterialUtility.SetShaderNonModifiableDefaults();
        }*/
    }
}
