using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using InternalRealtimeCSG;
using RealtimeCSG.Foundation;
using UnityEngine.Rendering;

namespace RealtimeCSG
{
    public static class MaterialUtility
    {
        internal const string SHADER_NAME_ROOT = "Hidden/CSG/Internal/";
        internal const string SPECIAL_SURFACE_SHADER_NAME = "SpecialSurface";
        internal const string SPECIAL_SURFACE_SHADER_ID = SHADER_NAME_ROOT + SPECIAL_SURFACE_SHADER_NAME;
        internal const string TRANSPARENT_SPECIAL_SURFACE_SHADER_NAME = "TransparentSpecialSurface";

        internal const string TRANSPARENT_SPECIAL_SURFACE_SHADER_ID = SHADER_NAME_ROOT + TRANSPARENT_SPECIAL_SURFACE_SHADER_NAME;

        internal const string HIDDEN_NAME = "hidden";
        internal const string CULLED_NAME = "culled";
        internal const string COLLIDER_NAME = "collider";
        internal const string TRIGGER_NAME = "trigger";
        internal const string SHADOW_ONLY_NAME = "shadowOnly";
        internal const string CAST_SHADOWS_NAME = "castShadows";
        internal const string RECEIVE_SHADOWS_NAME = "receiveShadows";

        internal const string HIDDEN_MATERIAL_NAME = TRANSPARENT_SPECIAL_SURFACE_SHADER_NAME + "_" + HIDDEN_NAME;
        internal const string CULLED_MATERIAL_NAME = TRANSPARENT_SPECIAL_SURFACE_SHADER_NAME + "_" + CULLED_NAME;
        internal const string COLLIDER_MATERIAL_NAME = SPECIAL_SURFACE_SHADER_NAME + "_" + COLLIDER_NAME;
        internal const string TRIGGER_MATERIAL_NAME = TRANSPARENT_SPECIAL_SURFACE_SHADER_NAME + "_" + TRIGGER_NAME;
        internal const string SHADOW_ONLY_MATERIAL_NAME = SPECIAL_SURFACE_SHADER_NAME + "_" + SHADOW_ONLY_NAME;
        internal const string CAST_SHADOWS_MATERIAL_NAME = SPECIAL_SURFACE_SHADER_NAME + "_" + CAST_SHADOWS_NAME;
        internal const string RECEIVE_SHADOWS_MATERIAL_NAME = SPECIAL_SURFACE_SHADER_NAME + "_" + RECEIVE_SHADOWS_NAME;

        internal static Shader SpecialSurfaceShader;
        internal static Shader TransparentSpecialSurfaceShader;

        private static readonly Dictionary<string, Material> editorMaterials = new Dictionary<string, Material>();

        private static bool m_ShadersInitialized;
        private static int m_PixelsPerPointId = -1;
        private static int m_LineThicknessMultiplierId = -1;
        private static int m_LineDashMultiplierId = -1;
        private static int m_LineAlphaMultiplierId = -1;

        private static readonly int editorColorMaterialColorProp = Shader.PropertyToID("_Color");

        private static void ShaderInit()
        {
            m_ShadersInitialized = true;

            m_PixelsPerPointId = Shader.PropertyToID("_pixelsPerPoint");
            m_LineThicknessMultiplierId = Shader.PropertyToID("_thicknessMultiplier");
            m_LineDashMultiplierId = Shader.PropertyToID("_dashMultiplier");
            m_LineAlphaMultiplierId = Shader.PropertyToID("_alphaMultiplier");

            SpecialSurfaceShader = Shader.Find(SPECIAL_SURFACE_SHADER_ID);
            TransparentSpecialSurfaceShader = Shader.Find(TRANSPARENT_SPECIAL_SURFACE_SHADER_ID);
        }

        internal static Material GenerateEditorMaterial(string shaderName, string textureName = null, string materialName = null)
        {
            string name = $"{shaderName}:{textureName}";

            if(editorMaterials.TryGetValue(name, out Material material))
            {
                // just in case one of many unity bugs destroyed the material
                if(!material)
                {
                    editorMaterials.Remove(name);
                }
                else
                {
                    return material;
                }
            }

            if(materialName == null)
            {
                materialName = name.Replace(':', '_');
            }

            Shader shader = Shader.Find(SHADER_NAME_ROOT + shaderName);

            if(!shader)
            {
                Debug.LogWarning("Could not find internal shader: " + SHADER_NAME_ROOT + shaderName);

                return null;
            }

            material = new Material(shader)
            {
                name = materialName
              , hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset
            };

            if(textureName != null)
            {
                material.mainTexture = Resources.Load<Texture2D>($"RealtimeCSG/Textures/{textureName}");

                if(!material.mainTexture)
                {
                    Debug.LogWarning("Could not find internal texture: " + textureName);
                }
            }

            editorMaterials.Add(name, material);

            return material;
        }

        internal static Material GenerateEditorColorMaterial(Color color)
        {
            string name = $"Color:{color}";

            if(editorMaterials.TryGetValue(name, out Material material))
            {
                // just in case one of many unity bugs destroyed the material
                if(!material)
                {
                    editorMaterials.Remove(name);
                }
                else
                {
                    return material;
                }
            }

            Shader shader = Shader.Find("Unlit/Color");

            if(!shader)
            {
                return null;
            }

            material = new Material(shader)
            {
                name = name.Replace(':', '_')
              , hideFlags = HideFlags.None | HideFlags.DontUnloadUnusedAsset
            };

            material.SetColor(editorColorMaterialColorProp, color);

            editorMaterials.Add(name, material);

            return material;
        }

        internal static PhysicsMaterial GetRuntimePhysicMaterial(string materialName)
        {
            return Resources.Load<PhysicsMaterial>($"RealtimeCSG/Materials/{materialName}");
        }

        private static PhysicsMaterial m_DefaultPhysicsMaterial;

        public static PhysicsMaterial DefaultPhysicsMaterial
        {
            get
            {
                if(!m_DefaultPhysicsMaterial)
                {
                    m_DefaultPhysicsMaterial = GetRuntimePhysicMaterial("Default");

                    if(!m_DefaultPhysicsMaterial)
                    {
                        Debug.LogError("Default physics material is missing");
                    }
                }

                return m_DefaultPhysicsMaterial;
            }
        }

        private static readonly Dictionary<Color, Material> colorMaterials = new Dictionary<Color, Material>();

        internal static Material GetColorMaterial(Color color)
        {
            if(colorMaterials.TryGetValue(color, out Material material))
            {
                // just in case one of many unity bugs destroyed the material
                if(!material)
                {
                    colorMaterials.Remove(color);
                }
                else
                {
                    return material;
                }
            }

            material = GenerateEditorColorMaterial(color);

            if(!material)
            {
                return AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
            }

            colorMaterials.Add(color, material);

            return material;
        }

        private static float m_LineThicknessMultiplier = 1.0f;

        internal static float LineThicknessMultiplier
        {
            get => m_LineThicknessMultiplier;
            set
            {
                if(Math.Abs(m_LineThicknessMultiplier - value) < MathConstants.EQUALITY_EPSILON)
                {
                    return;
                }

                m_LineThicknessMultiplier = value;
            }
        }

        private static float m_LineDashMultiplier = 1.0f;

        internal static float LineDashMultiplier
        {
            get => m_LineDashMultiplier;
            set
            {
                if(Math.Abs(m_LineDashMultiplier - value) < MathConstants.EQUALITY_EPSILON)
                {
                    return;
                }

                m_LineDashMultiplier = value;
            }
        }

        private static float m_LineAlphaMultiplier = 1.0f;

        internal static float LineAlphaMultiplier
        {
            get => m_LineAlphaMultiplier;
            set
            {
                if(Math.Abs(m_LineAlphaMultiplier - value) < MathConstants.EQUALITY_EPSILON)
                {
                    return;
                }

                m_LineAlphaMultiplier = value;
            }
        }

        private static Material m_ZTestGenericLine;

        internal static Material ZTestGenericLine
        {
            get
            {
                if(!m_ZTestGenericLine)
                {
                    m_ZTestGenericLine = GenerateEditorMaterial("ZTestGenericLine");
                }

                return m_ZTestGenericLine;
            }
        }

        internal static void InitGenericLineMaterial(Material genericLineMaterial)
        {
            if(!genericLineMaterial)
            {
                return;
            }

            if(!m_ShadersInitialized)
            {
                ShaderInit();
            }

            if(m_PixelsPerPointId != -1)
            {
                genericLineMaterial.SetFloat(m_PixelsPerPointId, EditorGUIUtility.pixelsPerPoint);
            }

            if(m_LineThicknessMultiplierId != -1)
            {
                genericLineMaterial.SetFloat(m_LineThicknessMultiplierId, m_LineThicknessMultiplier * EditorGUIUtility.pixelsPerPoint);
            }

            if(m_LineDashMultiplierId != -1)
            {
                genericLineMaterial.SetFloat(m_LineDashMultiplierId, m_LineDashMultiplier);
            }

            if(m_LineAlphaMultiplierId != -1)
            {
                genericLineMaterial.SetFloat(m_LineAlphaMultiplierId, m_LineAlphaMultiplier);
            }
        }

        private static Material m_NoZTestGenericLine;

        internal static Material NoZTestGenericLine
        {
            get
            {
                if(!m_NoZTestGenericLine)
                {
                    m_NoZTestGenericLine = GenerateEditorMaterial("NoZTestGenericLine");
                }

                return m_NoZTestGenericLine;
            }
        }

        private static Material m_ColoredPolygonMaterial;

        internal static Material ColoredPolygonMaterial
        {
            get
            {
                if(!m_ColoredPolygonMaterial)
                {
                    m_ColoredPolygonMaterial = GenerateEditorMaterial("Surface");
                }

                return m_ColoredPolygonMaterial;
            }
        }

        private static Material m_HiddenMaterial;

        internal static Material HiddenMaterial
        {
            get
            {
                if(!m_HiddenMaterial)
                {
                    m_HiddenMaterial = GenerateEditorMaterial(TRANSPARENT_SPECIAL_SURFACE_SHADER_NAME, HIDDEN_NAME, HIDDEN_MATERIAL_NAME);
                }

                return m_HiddenMaterial;
            }
        }

        private static Material m_CulledMaterial;

        internal static Material CulledMaterial
        {
            get
            {
                if(!m_CulledMaterial)
                {
                    m_CulledMaterial = GenerateEditorMaterial(TRANSPARENT_SPECIAL_SURFACE_SHADER_NAME, CULLED_NAME, CULLED_MATERIAL_NAME);
                }

                return m_CulledMaterial;
            }
        }

        private static Material m_ColliderMaterial;

        internal static Material ColliderMaterial
        {
            get
            {
                if(!m_ColliderMaterial)
                {
                    m_ColliderMaterial = GenerateEditorMaterial(SPECIAL_SURFACE_SHADER_NAME, COLLIDER_NAME, COLLIDER_MATERIAL_NAME);
                }

                return m_ColliderMaterial;
            }
        }

        private static Material m_TriggerMaterial;

        internal static Material TriggerMaterial
        {
            get
            {
                if(!m_TriggerMaterial)
                {
                    m_TriggerMaterial = GenerateEditorMaterial(TRANSPARENT_SPECIAL_SURFACE_SHADER_NAME, TRIGGER_NAME, TRIGGER_MATERIAL_NAME);
                }

                return m_TriggerMaterial;
            }
        }

        private static Material m_ShadowOnlyMaterial;

        internal static Material ShadowOnlyMaterial
        {
            get
            {
                if(!m_ShadowOnlyMaterial)
                {
                    m_ShadowOnlyMaterial = GenerateEditorMaterial(SPECIAL_SURFACE_SHADER_NAME, SHADOW_ONLY_NAME, SHADOW_ONLY_MATERIAL_NAME);
                }

                return m_ShadowOnlyMaterial;
            }
        }

        private static Material m_CastShadowsMaterial;

        internal static Material CastShadowsMaterial
        {
            get
            {
                if(!m_CastShadowsMaterial)
                {
                    m_CastShadowsMaterial = GenerateEditorMaterial(SPECIAL_SURFACE_SHADER_NAME, CAST_SHADOWS_NAME, CAST_SHADOWS_MATERIAL_NAME);
                }

                return m_CastShadowsMaterial;
            }
        }

        private static Material m_ReceiveShadowsMaterial;

        internal static Material ReceiveShadowsMaterial
        {
            get
            {
                if(!m_ReceiveShadowsMaterial)
                {
                    m_ReceiveShadowsMaterial = GenerateEditorMaterial(SPECIAL_SURFACE_SHADER_NAME, RECEIVE_SHADOWS_NAME, RECEIVE_SHADOWS_MATERIAL_NAME);
                }

                return m_ReceiveShadowsMaterial;
            }
        }

        internal static Texture2D CreateSolidColorTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];

            for(int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            Texture2D newTexture = new Texture2D(width, height);
            newTexture.hideFlags = HideFlags.DontUnloadUnusedAsset;
            newTexture.SetPixels(pixels);
            newTexture.Apply();

            return newTexture;
        }

        private static readonly Dictionary<Material, RenderSurfaceType> materialTypeLookup = new Dictionary<Material, RenderSurfaceType>();

        internal static void ResetMaterialTypeLookup()
        {
            materialTypeLookup.Clear();
        }

        internal static RenderSurfaceType GetMaterialSurfaceType(Material material)
        {
            if(ReferenceEquals(material, null))
            {
                return RenderSurfaceType.Normal;
            }

            if(materialTypeLookup.TryGetValue(material, out RenderSurfaceType surfaceType))
            {
                return surfaceType;
            }

            if(!material)
            {
                return RenderSurfaceType.Normal;
            }

            if(!m_ShadersInitialized)
            {
                ShaderInit();
            }

            Shader shader = material.shader;

            if(shader != SpecialSurfaceShader &&
               shader != TransparentSpecialSurfaceShader)
            {
                materialTypeLookup[material] = RenderSurfaceType.Normal;

                return RenderSurfaceType.Normal;
            }

            string shaderName = shader.name;

            if(shaderName != SPECIAL_SURFACE_SHADER_ID &&
               shaderName != TRANSPARENT_SPECIAL_SURFACE_SHADER_ID)
            {
                materialTypeLookup[material] = RenderSurfaceType.Normal;

                return RenderSurfaceType.Normal;
            }

            switch(material.name)
            {
                case HIDDEN_MATERIAL_NAME:
                {
                    surfaceType = RenderSurfaceType.Hidden;
                    materialTypeLookup[material] = surfaceType;

                    return surfaceType;
                }
                case CULLED_MATERIAL_NAME:
                {
                    surfaceType = RenderSurfaceType.Culled;
                    materialTypeLookup[material] = surfaceType;

                    return surfaceType;
                }
                case COLLIDER_MATERIAL_NAME:
                {
                    surfaceType = RenderSurfaceType.Collider;
                    materialTypeLookup[material] = surfaceType;

                    return surfaceType;
                }
                case TRIGGER_MATERIAL_NAME:
                {
                    surfaceType = RenderSurfaceType.Trigger;
                    materialTypeLookup[material] = surfaceType;

                    return surfaceType;
                }
                case SHADOW_ONLY_MATERIAL_NAME:
                {
                    surfaceType = RenderSurfaceType.ShadowOnly;
                    materialTypeLookup[material] = surfaceType;

                    return surfaceType;
                }
                case CAST_SHADOWS_MATERIAL_NAME:
                {
                    surfaceType = RenderSurfaceType.CastShadows;
                    materialTypeLookup[material] = surfaceType;

                    return surfaceType;
                }
                case RECEIVE_SHADOWS_MATERIAL_NAME:
                {
                    surfaceType = RenderSurfaceType.ReceiveShadows;
                    materialTypeLookup[material] = surfaceType;

                    return surfaceType;
                }
            }

            materialTypeLookup[material] = RenderSurfaceType.Normal;

            return RenderSurfaceType.Normal;
        }

        internal static Material GetSurfaceMaterial(RenderSurfaceType renderSurfaceType)
        {
            switch(renderSurfaceType)
            {
                case RenderSurfaceType.Hidden:
                    return HiddenMaterial;
                case RenderSurfaceType.Culled:
                    return CulledMaterial;
                case RenderSurfaceType.Collider:
                    return ColliderMaterial;
                case RenderSurfaceType.Trigger:
                    return TriggerMaterial;
                case RenderSurfaceType.ShadowOnly:
                    return ShadowOnlyMaterial;
                case RenderSurfaceType.CastShadows:
                    return CastShadowsMaterial;
                case RenderSurfaceType.ReceiveShadows:
                    return ReceiveShadowsMaterial;
                case RenderSurfaceType.Normal:
                default:
                    return null;
            }
        }
    }
}
