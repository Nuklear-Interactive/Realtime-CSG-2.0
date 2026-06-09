// * * * * * * * * * * * * * * * * * * * * * *
//  Description:
//
// * * * * * * * * * * * * * * * * * * * * * *

using UnityEngine;
using UnityEngine.Rendering;

namespace RealtimeCSG
{
    public class RenderPipelineHelper
    {
        public const string URP_TAG = "URP";
        public const string HDRP_TAG = "HDRP";
        public const string BUILTIN_TAG = "Builtin";

        public static string GetCurrentPipeline(out Shader defaultShader)
        {
            RenderPipelineAsset pipelineAsset = GraphicsSettings.currentRenderPipeline;

            // pipeline asset isnt null, so we assume it is a render pipeline
            if(pipelineAsset != null)
            {
                defaultShader = pipelineAsset.defaultShader;

                if(defaultShader.name.StartsWith("Universal Render Pipeline"))
                {
                    return URP_TAG;
                }

                if(defaultShader.name.StartsWith("HDRP"))
                {
                    return HDRP_TAG;
                }

                // we hand off detection to someone else, as a custom render pipeline will require manual work anyway.
                return defaultShader.name;
            }

            // pipeline asset is null, so we'll return the default standard shader.
            defaultShader = Shader.Find("Standard");

            return BUILTIN_TAG;
        }
    }
}
