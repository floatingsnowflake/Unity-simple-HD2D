//Stylized Water 2
//Staggart Creations (http://staggart.xyz)
//Copyright protected under Unity Asset Store EULA

//#define DEFAULT_GUI

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.AnimatedValues;
#if URP
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#endif

namespace StylizedWater2
{
    public class MaterialUI : ShaderGUI
    {
#if URP
        private const string BASE_SHADER_NAME = "Universal Render Pipeline/FX/Stylized Water 2";
        private const string TESSELLATION_SHADER_NAME = "Universal Render Pipeline/FX/Stylized Water 2 (Tessellation)";
        private MaterialEditor materialEditor;
        
        private MaterialProperty _ZWrite;
        private MaterialProperty _Cull;
        private MaterialProperty _ShadingMode;
        private MaterialProperty _Direction;
        private MaterialProperty _Speed;
        
        private MaterialProperty _SlopeStretching;
        private MaterialProperty _SlopeSpeed;
        private MaterialProperty _SlopeThreshold;

        private MaterialProperty _BaseColor;
        private MaterialProperty _ShallowColor;
        //private MaterialProperty _Smoothness;
        //private MaterialProperty _Metallic;
        
        private MaterialProperty _HorizonColor;
        private MaterialProperty _HorizonDistance;
        private MaterialProperty _DepthVertical;
        private MaterialProperty _DepthHorizontal;
        private MaterialProperty _DepthExp;
        private MaterialProperty _VertexColorDepth;
        
        private MaterialProperty _WaveTint;
        private MaterialProperty _WorldSpaceUV;
        private MaterialProperty _TranslucencyStrength;
        private MaterialProperty _TranslucencyExp;
        private MaterialProperty _TranslucencyCurvatureMask;
        private MaterialProperty _TranslucencyReflectionMask;
        private MaterialProperty _EdgeFade;
        private MaterialProperty _ShadowStrength;

        private MaterialProperty _CausticsTex;
        private MaterialProperty _CausticsBrightness;
        private MaterialProperty _CausticsTiling;
        private MaterialProperty _CausticsSpeed;
        private MaterialProperty _CausticsDistortion;
        private MaterialProperty _RefractionStrength;

        private MaterialProperty _UnderwaterSurfaceSmoothness;
        private MaterialProperty _UnderwaterRefractionOffset;

        private MaterialProperty _IntersectionSource;
        private MaterialProperty _IntersectionStyle;
        private MaterialProperty _IntersectionNoise;
        private MaterialProperty _IntersectionColor;
        private MaterialProperty _IntersectionLength;
        private MaterialProperty _IntersectionClipping;
        private MaterialProperty _IntersectionFalloff;
        private MaterialProperty _IntersectionTiling;
        private MaterialProperty _IntersectionRippleDist;
        private MaterialProperty _IntersectionRippleStrength;
        private MaterialProperty _IntersectionSpeed;

        private MaterialProperty _FoamTex;
        private MaterialProperty _FoamColor;
        private MaterialProperty _FoamSize;
        private MaterialProperty _FoamSpeed;
        private MaterialProperty _FoamTiling;
        private MaterialProperty _FoamWaveMask;
        private MaterialProperty _FoamWaveMaskExp;
        private MaterialProperty _VertexColorFoam;

        private MaterialProperty _BumpMap;
        private MaterialProperty _BumpMapSlope;
        private MaterialProperty _BumpMapLarge;
        private MaterialProperty _NormalTiling;
        private MaterialProperty _NormalStrength;
        private MaterialProperty _NormalSpeed;
        private MaterialProperty _DistanceNormalsFadeDist;
        private MaterialProperty _DistanceNormalsTiling;
        private MaterialProperty _SparkleIntensity;
        private MaterialProperty _SparkleSize;

        private MaterialProperty _SunReflectionSize;
        private MaterialProperty _SunReflectionStrength;
        private MaterialProperty _SunReflectionDistortion;
        private MaterialProperty _PointSpotLightReflectionStrength;
        private MaterialProperty _PointSpotLightReflectionSize;
        private MaterialProperty _PointSpotLightReflectionDistortion;
        
        private MaterialProperty _ReflectionStrength;
        private MaterialProperty _ReflectionDistortion;
        private MaterialProperty _ReflectionBlur;
        private MaterialProperty _ReflectionFresnel;
        private MaterialProperty _ReflectionLighting;

        private MaterialProperty _WaveSpeed;
        private MaterialProperty _WaveHeight;
        private MaterialProperty _VertexColorWaveFlattening;
        private MaterialProperty _WaveNormalStr;
        private MaterialProperty _WaveDistance;
        private MaterialProperty _WaveFadeDistance;
        private MaterialProperty _WaveSteepness;
        private MaterialProperty _WaveCount;
        private MaterialProperty _WaveDirection;

        private MaterialProperty _TessValue;
        private MaterialProperty _TessMin;
        private MaterialProperty _TessMax;

        private bool tesselationEnabled;

        private UI.Material.Section generalSection;
        private UI.Material.Section lightingSection;
        private UI.Material.Section colorSection;
        private UI.Material.Section underwaterSection;
        private UI.Material.Section normalsSection;
        private UI.Material.Section reflectionSection;
        private UI.Material.Section intersectionSection;
        private UI.Material.Section foamSection;
        private UI.Material.Section wavesSection;
        private UI.Material.Section advancedSection;

        //Keyword states
        private MaterialProperty _LightingOn;
        private MaterialProperty _ReceiveShadows;
        private MaterialProperty _FlatShadingOn;
        private MaterialProperty _TranslucencyOn;
        private MaterialProperty _RiverModeOn;
        private MaterialProperty _SpecularReflectionsOn;
        private MaterialProperty _EnvironmentReflectionsOn;
        private MaterialProperty _DisableDepthTexture;
        private MaterialProperty _CausticsOn;
        private MaterialProperty _NormalMapOn;
        private MaterialProperty _DistanceNormalsOn;
        private MaterialProperty _FoamOn;
        private MaterialProperty _RefractionOn;
        private MaterialProperty _WavesOn;

        private MaterialProperty _CurvedWorldBendSettings;
        
        private GUIContent simpleShadingContent;
        private GUIContent advancedShadingContent;

        private bool initialized;
        private bool transparentShadowsEnabled;
        private bool depthAfterTransparents = false;
        private bool underwaterRenderingInstalled;

        private void FindProperties(MaterialProperty[] props, Material material)
        {
            tesselationEnabled = material.HasProperty("_TessValue");

            _IntersectionSource = FindProperty("_IntersectionSource", props);
            _IntersectionStyle = FindProperty("_IntersectionStyle", props);
            
            if (tesselationEnabled)
            {
                _TessValue = FindProperty("_TessValue", props);
                _TessMin = FindProperty("_TessMin", props);
                _TessMax = FindProperty("_TessMax", props);
            }

            _Cull = FindProperty("_Cull", props);
            _ZWrite = FindProperty("_ZWrite", props);
            _ShadingMode = FindProperty("_ShadingMode", props);
            _ShadowStrength = FindProperty("_ShadowStrength", props);
            _Direction = FindProperty("_Direction", props);
            _Speed = FindProperty("_Speed", props);

            _SlopeStretching = FindProperty("_SlopeStretching", props);
            _SlopeSpeed = FindProperty("_SlopeSpeed", props);
            _SlopeThreshold = FindProperty("_SlopeThreshold", props);
            
            _DisableDepthTexture = FindProperty("_DisableDepthTexture", props);
            _RefractionOn = FindProperty("_RefractionOn", props);

            _BaseColor = FindProperty("_BaseColor", props);
            _ShallowColor = FindProperty("_ShallowColor", props);
            //_Smoothness = FindProperty("_Smoothness", props);
            //_Metallic = FindProperty("_Metallic", props);
            _HorizonColor = FindProperty("_HorizonColor", props);
            _HorizonDistance = FindProperty("_HorizonDistance", props);
            _DepthVertical = FindProperty("_DepthVertical", props);
            _DepthHorizontal = FindProperty("_DepthHorizontal", props);
            _DepthExp = FindProperty("_DepthExp", props);
            _VertexColorDepth = FindProperty("_VertexColorDepth", props);

            _WaveTint = FindProperty("_WaveTint", props);
            _WorldSpaceUV = FindProperty("_WorldSpaceUV", props);
            _TranslucencyStrength = FindProperty("_TranslucencyStrength", props);
            _TranslucencyExp = FindProperty("_TranslucencyExp", props);
            _TranslucencyCurvatureMask = FindProperty("_TranslucencyCurvatureMask", props);
            _TranslucencyReflectionMask = FindProperty("_TranslucencyReflectionMask", props);
            _EdgeFade = FindProperty("_EdgeFade", props);

            _CausticsOn = FindProperty("_CausticsOn", props);
            _CausticsTex = FindProperty("_CausticsTex", props);
            _CausticsBrightness = FindProperty("_CausticsBrightness", props);
            _CausticsTiling = FindProperty("_CausticsTiling", props);
            _CausticsSpeed = FindProperty("_CausticsSpeed", props);
            _CausticsDistortion = FindProperty("_CausticsDistortion", props);
            _RefractionStrength = FindProperty("_RefractionStrength", props);
            
            _UnderwaterSurfaceSmoothness = FindProperty("_UnderwaterSurfaceSmoothness", props);
            _UnderwaterRefractionOffset = FindProperty("_UnderwaterRefractionOffset", props);
            
            _IntersectionNoise = FindProperty("_IntersectionNoise", props);
            _IntersectionColor = FindProperty("_IntersectionColor", props);
            _IntersectionLength = FindProperty("_IntersectionLength", props);
            _IntersectionClipping = FindProperty("_IntersectionClipping", props);
            _IntersectionFalloff = FindProperty("_IntersectionFalloff", props);
            _IntersectionTiling = FindProperty("_IntersectionTiling", props);
            _IntersectionRippleDist = FindProperty("_IntersectionRippleDist", props);
            _IntersectionRippleStrength = FindProperty("_IntersectionRippleStrength", props);
            _IntersectionSpeed = FindProperty("_IntersectionSpeed", props);
            
            _FoamTex = FindProperty("_FoamTex", props);
            _FoamColor = FindProperty("_FoamColor", props);
            _FoamSize = FindProperty("_FoamSize", props);
            _FoamSpeed = FindProperty("_FoamSpeed", props);
            _FoamTiling = FindProperty("_FoamTiling", props);
            _FoamWaveMask = FindProperty("_FoamWaveMask", props);
            _FoamWaveMaskExp = FindProperty("_FoamWaveMaskExp", props);
            _VertexColorFoam = FindProperty("_VertexColorFoam", props);
            
            _BumpMap = FindProperty("_BumpMap", props);
            _BumpMapSlope = FindProperty("_BumpMapSlope", props);
            _NormalTiling = FindProperty("_NormalTiling", props);
            _NormalStrength = FindProperty("_NormalStrength", props);
            _NormalSpeed = FindProperty("_NormalSpeed", props);

            _BumpMapLarge = FindProperty("_BumpMapLarge", props);
            _DistanceNormalsFadeDist = FindProperty("_DistanceNormalsFadeDist", props);
            _DistanceNormalsTiling = FindProperty("_DistanceNormalsTiling", props);
            
            _SparkleIntensity = FindProperty("_SparkleIntensity", props);
            _SparkleSize = FindProperty("_SparkleSize", props);

            _SunReflectionSize = FindProperty("_SunReflectionSize", props);
            _SunReflectionStrength = FindProperty("_SunReflectionStrength", props);
            _SunReflectionDistortion = FindProperty("_SunReflectionDistortion", props);
            _PointSpotLightReflectionStrength = FindProperty("_PointSpotLightReflectionStrength", props);
            _PointSpotLightReflectionSize = FindProperty("_PointSpotLightReflectionSize", props);
            _PointSpotLightReflectionDistortion = FindProperty("_PointSpotLightReflectionDistortion", props);
            
            _ReflectionStrength = FindProperty("_ReflectionStrength", props);
            _ReflectionDistortion = FindProperty("_ReflectionDistortion", props);
            _ReflectionBlur = FindProperty("_ReflectionBlur", props);
            _ReflectionFresnel = FindProperty("_ReflectionFresnel", props);
            _ReflectionLighting = FindProperty("_ReflectionLighting", props);

            _WaveSpeed = FindProperty("_WaveSpeed", props);
            _WaveHeight = FindProperty("_WaveHeight", props);
            _VertexColorWaveFlattening = FindProperty("_VertexColorWaveFlattening", props);
            _WaveNormalStr = FindProperty("_WaveNormalStr", props);
            _WaveDistance = FindProperty("_WaveDistance", props);
            _WaveFadeDistance = FindProperty("_WaveFadeDistance", props);
            _WaveSteepness = FindProperty("_WaveSteepness", props);
            _WaveCount = FindProperty("_WaveCount", props);
            _WaveDirection = FindProperty("_WaveDirection", props);

            //Keyword states
            _LightingOn = FindProperty("_LightingOn", props);
            _ReceiveShadows = FindProperty("_ReceiveShadows", props);
            _FlatShadingOn = FindProperty("_FlatShadingOn", props);
            _TranslucencyOn = FindProperty("_TranslucencyOn", props);
            _RiverModeOn = FindProperty("_RiverModeOn", props);
            _FoamOn = FindProperty("_FoamOn", props);
            _SpecularReflectionsOn = FindProperty("_SpecularReflectionsOn", props);
            _EnvironmentReflectionsOn = FindProperty("_EnvironmentReflectionsOn", props);
            _NormalMapOn = FindProperty("_NormalMapOn", props);
            _DistanceNormalsOn = FindProperty("_DistanceNormalsOn", props);
            _WavesOn = FindProperty("_WavesOn", props);

            if(material.HasProperty("_CurvedWorldBendSettings")) _CurvedWorldBendSettings = FindProperty("_CurvedWorldBendSettings", props);
            
            simpleShadingContent = new GUIContent("Simple", 
             "Mobile friendly");

            advancedShadingContent = new GUIContent("Advanced",
                "Advanced mode does:\n\n" +
                "• Chromatic refraction\n" +
                "• Higher accuracy normal map blending\n" +
                "• Caustics & Translucency shading for point/spot lights\n" +
                "• Double sampling of depth, for accurate refraction\n" +
                "• Accurate blending of light color for translucency shading\n" +
                "• Additional texture sample for distance normals");
        }

        private void OnEnable(MaterialEditor materialEditorIn)
        {
            lightingSection = new UI.Material.Section(materialEditorIn,"LIGHTING", new GUIContent("Lighting/Shading"));
            generalSection = new UI.Material.Section(materialEditorIn,"GENERAL", new GUIContent("General"));
            colorSection = new UI.Material.Section(materialEditorIn,"COLOR", new GUIContent("Color", "Controls for the base color of the water and transparency"));
            underwaterSection = new UI.Material.Section(materialEditorIn,"UNDERWATER", new GUIContent("Underwater", "Pertains the appearance of anything seen under the water surface. Not related to any actual underwater rendering"));
            normalsSection = new UI.Material.Section(materialEditorIn,"NORMALS", new GUIContent("Normals", "Normal maps represent the small-scale curvature of the water surface. This is used for lighting and reflections"));
            reflectionSection = new UI.Material.Section(materialEditorIn,"REFLECTIONS", new GUIContent("Reflections", "Sun specular reflection, and environment reflections (reflection probes and planar reflections)"));
            foamSection = new UI.Material.Section(materialEditorIn,"FOAM", new GUIContent("Surface Foam"));
            intersectionSection = new UI.Material.Section(materialEditorIn,"INTERSECTION", new GUIContent("Intersection Foam", "Draws a foam effects on opaque objects that are touching the water"));
            wavesSection = new UI.Material.Section(materialEditorIn,"WAVES", new GUIContent("Waves", "Parametric gerstner waves, which modify the surface curvature and animate the mesh's vertices"));
            advancedSection = new UI.Material.Section(materialEditorIn,"ADVANCED", new GUIContent("Advanced"));
            
            underwaterRenderingInstalled = StylizedWaterEditor.UnderwaterRenderingInstalled();
            
            #if URP
            transparentShadowsEnabled = PipelineUtilities.TransparentShadowsEnabled();
            #if UNITY_2022_2_OR_NEWER
            depthAfterTransparents = PipelineUtilities.IsDepthAfterTransparents();
            #endif
            #endif

            foreach (UnityEngine.Object target in materialEditorIn.targets)
            {
                MaterialChanged((Material)target);
                
                //Any material not yet upgraded would have this property...
                if (GetLegacyVectorProperty((Material)target, "_VertexColorMask").x >= 0)
                {
                    UpgradeProperties((Material)target);
                }
            }
            
            initialized = true;
        }
        
        public override void OnClosed(Material material)
        {
            initialized = false;
        }

        //https://github.com/Unity-Technologies/Graphics/blob/648184ec8405115e2fcf4ad3023d8b16a191c4c7/com.unity.render-pipelines.universal/Editor/ShaderGUI/BaseShaderGUI.cs
        public override void OnGUI(MaterialEditor materialEditorIn, MaterialProperty[] props)
        {
            this.materialEditor = materialEditorIn;

            materialEditor.SetDefaultGUIWidths();
            materialEditor.UseDefaultMargins();
            EditorGUIUtility.labelWidth = 0f;

            Material material = materialEditor.target as Material;

            //Requires refetching for undo/redo to function
            FindProperties(props, material);

#if DEFAULT_GUI
            base.OnGUI(materialEditor, props);
            return;
#endif
            
            if (!initialized)
            {
                OnEnable(materialEditor);
            }
            
            ShaderPropertiesGUI(material);
            
            UI.DrawFooter();
        }

        public void ShaderPropertiesGUI(Material material)
        {
            EditorGUI.BeginChangeCheck();
            
            DrawHeader();
            
            EditorGUILayout.Space();
            
            DrawGeneral();
            DrawLighting();
            DrawColor();
            DrawNormals();
            DrawUnderwater();
            DrawFoam();
            DrawIntersection();
            DrawReflections();
            DrawWaves();
            DrawAdvanced(material);
            
            if (material.HasProperty("_CurvedWorldBendSettings"))
            {
                EditorGUILayout.LabelField("Curved World 2020", EditorStyles.boldLabel);
                materialEditor.ShaderProperty(_CurvedWorldBendSettings, _CurvedWorldBendSettings.displayName);
                EditorGUILayout.Space();
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in  materialEditor.targets)
                    MaterialChanged((Material)obj);
            }
        }

        public override void OnMaterialPreviewGUI(MaterialEditor materialEditor, Rect rect, GUIStyle background)
        {
            UI.Material.DrawMaterialHeader(materialEditor, rect, background);
            
            UI.DrawNotification(!UniversalRenderPipeline.asset, "Universal Render Pipeline is currently not active!", "Show me", StylizedWaterEditor.OpenGraphicsSettings, MessageType.Error);

            if (UniversalRenderPipeline.asset && initialized)
            {
                UI.DrawNotification(
                    UniversalRenderPipeline.asset.supportsCameraDepthTexture == false &&
                    _DisableDepthTexture.floatValue == 0f,
                    "Depth texture is disabled, which is required for the material's current configuration",
                    "Enable",
                    StylizedWaterEditor.EnableDepthTexture,
                    MessageType.Error);
                
                UI.DrawNotification(
                    UniversalRenderPipeline.asset.supportsCameraOpaqueTexture == false && _RefractionOn.floatValue == 1f,
                    "Opaque texture is disabled, which is required for the material's current configuration",
                    "Enable",
                    StylizedWaterEditor.EnableOpaqueTexture,
                    MessageType.Error);
            }
            
            UI.DrawNotification(depthAfterTransparents && _ZWrite.floatValue > 0, "\nZWrite option (Advanced tab) is enabled & Depth Texture Mode is set to \'After Transparents\" on the default renderer\n\nWater can not render properly with this combination\n", MessageType.Error);
        }

        private void MaterialChanged(Material material)
        {
            if (material == null) throw new ArgumentNullException("material");

            SetMaterialKeywords(material);
            
            material.SetTexture("_CausticsTex", _CausticsTex.textureValue);
            material.SetTexture("_BumpMap", _BumpMap.textureValue);
            material.SetTexture("_BumpMapSlope", _BumpMapSlope.textureValue);
            material.SetTexture("_BumpMapLarge", _BumpMapLarge.textureValue);
            material.SetTexture("_FoamTex", _FoamTex.textureValue);
            material.SetTexture("_IntersectionNoise", _IntersectionNoise.textureValue);
        }

        private void SetMaterialKeywords(Material material)
        {
#if URP
            //Keywords;
            CoreUtils.SetKeyword(material, "_ADVANCED_SHADING", material.GetFloat("_ShadingMode") == 1f);
            CoreUtils.SetKeyword(material, "_SHARP_INERSECTION", material.GetFloat("_IntersectionStyle") == 1);
            CoreUtils.SetKeyword(material, "_SMOOTH_INTERSECTION", material.GetFloat("_IntersectionStyle") == 2);
#endif
        }
        
        private void DrawHeader()
        {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android || EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                if (PlayerSettings.GetUseDefaultGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget) == false &&
                    PlayerSettings.GetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget)[0] == GraphicsDeviceType.OpenGLES2)
                {
                    UI.DrawNotification("You are targeting the OpenGLES 2.0 graphics API, which is not supported. Shader will not compile on the device", MessageType.Error);
                }
            }
            
            Rect rect = EditorGUILayout.GetControlRect();
            
            GUIContent c = new GUIContent("Version " + AssetInfo.INSTALLED_VERSION);
            rect.width = EditorStyles.miniLabel.CalcSize(c).x + 8f;
            //rect.x += (rect.width * 2f);
            rect.y -= 3f;
            GUI.Label(rect, c, EditorStyles.label);

            rect.x += rect.width + 3f;
            rect.y += 2f;
            rect.width = 16f;
            rect.height = 16f;
            
            GUI.DrawTexture(rect, EditorGUIUtility.IconContent("preAudioLoopOff").image);
            if (Event.current.type == EventType.MouseDown)
            {
                if (rect.Contains(Event.current.mousePosition) && Event.current.button == 0)
                {
                    AssetInfo.VersionChecking.GetLatestVersionPopup();
                    Event.current.Use();
                }
            }

            if (rect.Contains(Event.current.mousePosition))
            {
                Rect tooltipRect = rect;
                tooltipRect.y -= 20f;
                tooltipRect.width = 120f;
                GUI.Label(tooltipRect, "Check for update", GUI.skin.button);
            }

            c = new GUIContent("Open asset window", EditorGUIUtility.IconContent("_Help").image, "Show help and third-party integrations");
            rect.width = (EditorStyles.miniLabel.CalcSize(c).x + 32f);
            rect.x = EditorGUIUtility.currentViewWidth - rect.width - 17f;
            rect.height = 17f;

            if (GUI.Button(rect, c))
            {
                HelpWindow.ShowWindow();
            }

            GUILayout.Space(3f);
        }
        
        #region Sections
        private void DrawGeneral()
        {
            generalSection.DrawHeader(() => SwitchSection(generalSection));
            
            if (EditorGUILayout.BeginFadeGroup(generalSection.anim.faded))
            {
                EditorGUILayout.Space();
                
                materialEditor.ShaderProperty(_Cull, new GUIContent(_Cull.displayName, "Controls which sides of the water mesh surface is visible"));

                using (new EditorGUI.DisabledGroupScope(_RiverModeOn.floatValue > 0))
                {
                    materialEditor.ShaderProperty(_WorldSpaceUV, new GUIContent(_WorldSpaceUV.displayName, "Use either the mesh's UV or world-space units as a base for texture tiling"));
                }
                if(_RiverModeOn.floatValue > 0) EditorGUILayout.HelpBox("Shader will use always Mesh UV coordinates when River Mode is enabled.", MessageType.None);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);

                UI.Material.DrawVector2(_Direction, "Direction");
                UI.Material.DrawFloatField(_Speed, label:"Speed");
                
                EditorGUILayout.Space();

                materialEditor.ShaderProperty(_RiverModeOn, new GUIContent("River Mode",
                        "When enabled, all animations flow in the vertical UV direction and stretch on slopes, creating faster flowing water." +
                        " \n\nSurface foam also draws on slopes"));

                if (_RiverModeOn.floatValue > 0 || _RiverModeOn.hasMixedValue)
                {
                    materialEditor.ShaderProperty(_SlopeStretching, new GUIContent("Slope stretching", null, "On slopes, stretches the UV's by this much. Creates the illusion of faster flowing water"), 1);
                    materialEditor.ShaderProperty(_SlopeSpeed, new GUIContent("Slope speed", null, "On slopes, animation speed is multiplied by this value"), 1);
                    materialEditor.ShaderProperty(_SlopeThreshold, new GUIContent(_SlopeThreshold.displayName, "A higher value results in foam also drawing on surfaces that are relatively flat"), 1);
                }


                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void DrawLighting()
        {
            lightingSection.DrawHeader(() => SwitchSection(lightingSection));

            if (EditorGUILayout.BeginFadeGroup(lightingSection.anim.faded))
            {
                EditorGUILayout.Space();

                using (new EditorGUILayout.HorizontalScope())
                {
                    #if UNITY_2022_1_OR_NEWER
                    MaterialEditor.BeginProperty(_ShadingMode);
                    #endif
                    
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = _ShadingMode.hasMixedValue;

                    if (_ShadingMode.hasMixedValue)
                    {
                        materialEditor.ShaderProperty(_ShadingMode, advancedShadingContent);
                    }
                    else
                    {
                        EditorGUILayout.LabelField(_ShadingMode.displayName, GUILayout.Width(EditorGUIUtility.labelWidth));

                        float shadingMode = GUILayout.Toolbar((int)_ShadingMode.floatValue, new GUIContent[] { simpleShadingContent, advancedShadingContent, }, GUILayout.MaxWidth((250f)));
                        
                        if (EditorGUI.EndChangeCheck())
                            _ShadingMode.floatValue = shadingMode;
                    }

                    EditorGUI.showMixedValue = false;
                    
                    #if UNITY_2022_1_OR_NEWER
                    MaterialEditor.EndProperty();
                    #endif
                }
                
                EditorGUILayout.Space();

                if ((EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
                     EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS) && _ShadingMode.floatValue == 1f)
                {
                    UI.DrawNotification("The current shading mode is not intended to be used on mobile hardware", MessageType.Warning);
                }

                materialEditor.ShaderProperty(_LightingOn, new GUIContent("Enable lighting", "Color from lights and Ambient light will affect the material. Can be disabled if using Unlit shaders, or fixed lighting"));

                materialEditor.ShaderProperty(_FlatShadingOn, new GUIContent("Flat/low-poly shading", "When enabled, normals are calculated per mesh face, resulting in a faceted appearance (low poly look)"));
                UI.DrawNotification(_FlatShadingOn.floatValue > 0 && tesselationEnabled, "Tessellation is enabled, it should not be used to achieve the desired effect", MessageType.Warning);

                UI.DrawNotification(_FlatShadingOn.floatValue > 0f && _WavesOn.floatValue == 0f, "Flat shading has little effect if waves are disabled", MessageType.Warning);

                materialEditor.ShaderProperty(_ReceiveShadows, new GUIContent("Receive shadows"));
                if ((_ReceiveShadows.floatValue > 0 || _ReceiveShadows.hasMixedValue) && !transparentShadowsEnabled && _ShadingMode.floatValue != 0)
                {
                    #if URP
                    transparentShadowsEnabled = PipelineUtilities.TransparentShadowsEnabled();
                    #endif
                }
                UI.DrawNotification((_ReceiveShadows.floatValue > 0 || _ReceiveShadows.hasMixedValue) && !transparentShadowsEnabled,
                    "Transparent shadows are disabled in the default Forward renderer", "Show me",
                    StylizedWaterEditor.SelectForwardRenderer, MessageType.Warning);
                
                using (new EditorGUI.DisabledScope(_LightingOn.floatValue < 1f || _LightingOn.hasMixedValue))
                {
                    if ((_ReceiveShadows.floatValue > 0 || _ReceiveShadows.hasMixedValue))
                    {
                        materialEditor.ShaderProperty(_ShadowStrength, "Strength", 1);
                    }
                }

                EditorGUILayout.Space();

                using (new EditorGUI.DisabledScope(_NormalMapOn.floatValue == 0f))
                {
                    EditorGUILayout.LabelField("Sparkles", EditorStyles.boldLabel);
                    materialEditor.ShaderProperty(_SparkleIntensity, "Intensity");
                    materialEditor.ShaderProperty(_SparkleSize, "Size");
                }
                UI.DrawNotification(_NormalMapOn.floatValue == 0f, "Sparkles require the normal map feature to be enabled", MessageType.None);
                
                EditorGUILayout.Space();

                materialEditor.ShaderProperty(_TranslucencyOn, new GUIContent("Translucency", "Creates the appearance of sun light shining through the waves.\n\nNote that is only visible at grazing light angle"));

                if (_TranslucencyOn.floatValue > 0 || _TranslucencyOn.hasMixedValue)
                {
                    materialEditor.ShaderProperty(_TranslucencyStrength, new GUIContent("Strength"), 1);
                    materialEditor.ShaderProperty(_TranslucencyExp, new GUIContent("Exponent", "Essentially controls the width/scale of the effect"), 1);
                    materialEditor.ShaderProperty(_TranslucencyCurvatureMask, new GUIContent("Curvature mask", "Masks the effect by the orientation of the surface. Surfaces facing away from the sun will receive less of an effect"), 1);
                    materialEditor.ShaderProperty(_TranslucencyReflectionMask, new GUIContent("Reflection Mask", "Controls how strongly reflections are laid over the effect. A value of 1 is physically accurate"), 1);
                }
                
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void DrawColor()
        {
            colorSection.DrawHeader(() => SwitchSection(colorSection));

            if (EditorGUILayout.BeginFadeGroup(colorSection.anim.faded))
            {
                EditorGUILayout.Space();

                UI.Material.DrawColorField(_BaseColor, true, _BaseColor.displayName, "Base water color, alpha channel controls transparency");
                UI.Material.DrawColorField(_ShallowColor, true, _ShallowColor.displayName, "Water color in shallow areas, alpha channel controls transparency. Note that the caustics effect is visible here, setting the alpha to 100% hides caustics");
                
                //materialEditor.ShaderProperty(_Smoothness);
                //materialEditor.ShaderProperty(_Metallic);

                using (new EditorGUI.DisabledGroupScope(_DisableDepthTexture.floatValue == 1f && !_DisableDepthTexture.hasMixedValue))
                {
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Fog/Density", EditorStyles.boldLabel);
                    materialEditor.ShaderProperty(_DepthVertical, new GUIContent("Distance Depth", "Distance measured from the camera to the surface behind the water. Water turns denser the more the camera looks along it"));
                    materialEditor.ShaderProperty(_DepthHorizontal, label:"Vertical Depth");
                    
                    materialEditor.ShaderProperty(_DepthExp, new GUIContent("Exponential", tooltip:"Exponential depth works best for shallow water and relatively flat shores"), 1);
                }
                
                EditorGUILayout.Space();

                materialEditor.ShaderProperty(_VertexColorDepth, new GUIContent("Vertex color depth (G)", "The Green vertex color channel also adds opacity"));
                using (new EditorGUI.DisabledGroupScope(_DisableDepthTexture.floatValue == 1f && !_DisableDepthTexture.hasMixedValue))
                {
                    UI.Material.DrawFloatField(_EdgeFade, "Edge fading", "Fades out the water where it intersects with opaque objects.\n\nRequires the depth texture option to be enabled");
                    _EdgeFade.floatValue = Mathf.Max(0f, _EdgeFade.floatValue);
                }
                EditorGUILayout.Space();

                UI.Material.DrawColorField(_HorizonColor, true, _HorizonColor.displayName, "Color as perceived on the horizon, where looking across the water");
                materialEditor.ShaderProperty(_HorizonDistance, _HorizonDistance.displayName);

                materialEditor.ShaderProperty(_WaveTint, new GUIContent(_WaveTint.displayName, "Adds a bright/dark tint based on wave height\n\nWaves feature must be enabled"));

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void DrawNormals()
        {
            normalsSection.DrawHeader(() => SwitchSection(normalsSection));

            if (EditorGUILayout.BeginFadeGroup(normalsSection.anim.faded))
            {
                EditorGUILayout.Space();

                materialEditor.ShaderProperty(_NormalMapOn,  new GUIContent("Enable", "Normals add small-scale detail to the water surface, which in turn is used in various lighting techniques"));
                
                EditorGUILayout.Space();

                if (_NormalMapOn.floatValue > 0f || _NormalMapOn.hasMixedValue)
                {
                    materialEditor.TextureProperty(_BumpMap, "Normal map");

                    if (_RiverModeOn.floatValue > 0f || _RiverModeOn.hasMixedValue)
                    {
                        materialEditor.TextureProperty(_BumpMapSlope, "River slopes");
                    }
                    UI.Material.DrawFloatTicker(_NormalTiling, "Tiling");
                    UI.Material.DrawFloatTicker(_NormalSpeed, "Speed multiplier");
                    materialEditor.ShaderProperty(_NormalStrength, "Strength (lighting)");
                    if (_LightingOn.floatValue < 1f || _LightingOn.hasMixedValue)
                    {
                        UI.DrawNotification("Lighting is disabled, normal strength has no effect", MessageType.Info);
                    }
                    
                    EditorGUILayout.Space();

                    materialEditor.ShaderProperty(_DistanceNormalsOn, new GUIContent("Distance normals", "Resamples normals in the distance, at a larger scale. At the cost of additional processing, tiling artifacts can be greatly reduced"));

                    if (_DistanceNormalsOn.floatValue > 0 || _DistanceNormalsOn.hasMixedValue)
                    {
                        materialEditor.TextureProperty(_BumpMapLarge, "Normal map");
                        UI.Material.DrawFloatTicker(_DistanceNormalsTiling, "Tiling multiplier");

                        UI.Material.DrawMinMaxSlider(_DistanceNormalsFadeDist, 0f, 500, "Blend distance range", tooltip:"Min/max distance the effect should start to blend in");
                    }
                }

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void DrawUnderwater()
        {
            underwaterSection.DrawHeader(() => SwitchSection(underwaterSection));

            if (EditorGUILayout.BeginFadeGroup(underwaterSection.anim.faded))
            {
                EditorGUILayout.Space();

                materialEditor.ShaderProperty(_CausticsOn, "Caustics");
                
                if (_CausticsOn.floatValue == 1 || _CausticsOn.hasMixedValue)
                {
                    materialEditor.TextureProperty(_CausticsTex, "Texture (Additively blended)");
                    UI.Material.DrawFloatField(_CausticsBrightness);
                    materialEditor.ShaderProperty(_CausticsDistortion, new GUIContent(_CausticsDistortion.displayName, "Distorted the caustics based on the normal map"));
                    
                    EditorGUILayout.Space();

                    UI.Material.DrawFloatTicker(_CausticsTiling);
                    UI.Material.DrawFloatTicker(_CausticsSpeed);
                }
                if (_DisableDepthTexture.floatValue == 1f && _CausticsOn.floatValue == 1f)
                {
                    UI.DrawNotification("Caustics are disabled because the \"Disable depth texture\" option is", MessageType.Error);
                }

                EditorGUILayout.Space();

                materialEditor.ShaderProperty(_RefractionOn, "Refraction");

                if (_RefractionOn.floatValue == 1f || _RefractionOn.hasMixedValue)
                {
                    if (_NormalMapOn.floatValue == 0f && _WavesOn.floatValue == 0f)
                    {
                        UI.DrawNotification("Refraction will have no effect if normals and waves are disabled", MessageType.Warning);
                    }
                    
                    materialEditor.ShaderProperty(_RefractionStrength, new GUIContent("Strength", "Note: Distortion strength is influenced by the strength of the normal map texture"), 1);
                }
                else
                {
                    if (underwaterRenderingInstalled)
                    {
                        UI.DrawNotification("[Underwater Rendering] It's recommended to keep refraction enabled.\n\n It is performed anyway for the underwater surface", MessageType.Warning);
                    }
                }
                
                if (underwaterRenderingInstalled)
                {
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Underwater Surface Rendering", EditorStyles.boldLabel);
                    materialEditor.ShaderProperty(_UnderwaterSurfaceSmoothness, new GUIContent("Surface Smoothness", "Controls how distorted everything above the water appears from below"));
                    materialEditor.ShaderProperty(_UnderwaterRefractionOffset, new GUIContent("Refraction offset", "Creates a wide \"circle\" of visible air above the camera. Pushes it further away from the camera"));
                }

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void DrawFoam()
        {
            foamSection.DrawHeader(() => SwitchSection(foamSection));

            if (EditorGUILayout.BeginFadeGroup(foamSection.anim.faded))
            {
                EditorGUILayout.Space();
                
                materialEditor.ShaderProperty(_FoamOn, "Enable");
                if (_FoamOn.floatValue > 0 || _FoamOn.hasMixedValue)
                {
                    materialEditor.TextureProperty(_FoamTex, "Texture (R=Mask)");
                    UI.Material.DrawColorField(_FoamColor, true, "Color", "Color of the foam, the alpha channel controls opacity");
                    materialEditor.ShaderProperty(_VertexColorFoam, new GUIContent("Vertex color painting (A)",
                        "Enable the usage of the vertex color Alpha channel to add foam"));

                    materialEditor.ShaderProperty(_FoamSize, new GUIContent(_FoamSize.displayName, "Clips the texture based on its grayscale values. This means if the foam texture is a hard black/white texture, it has no effect"));
                    using (new EditorGUI.DisabledGroupScope(_RiverModeOn.floatValue > 0 && !_RiverModeOn.hasMixedValue))
                    {
                        materialEditor.ShaderProperty(_FoamWaveMask, new GUIContent(_FoamWaveMask.displayName, "Opt to only show the foam on the highest points of waves"));
                        materialEditor.ShaderProperty(_FoamWaveMaskExp, new GUIContent("Exponent", "Pushes the mask more towards the top of the waves"), 1);
                    }
                    
                    UI.Material.DrawFloatTicker(_FoamTiling);
                    UI.Material.DrawFloatTicker(_FoamSpeed);
                }

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void DrawIntersection()
        {
            intersectionSection.DrawHeader(() => SwitchSection(intersectionSection));

            if (EditorGUILayout.BeginFadeGroup(intersectionSection.anim.faded))
            {
                EditorGUILayout.Space();
                
                using (new EditorGUILayout.HorizontalScope())
                {
                    #if UNITY_2022_1_OR_NEWER
                    MaterialEditor.BeginProperty(_IntersectionStyle);
                    #endif
                    
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = _IntersectionStyle.hasMixedValue;

                    if (_IntersectionStyle.hasMixedValue)
                    {
                        materialEditor.ShaderProperty(_IntersectionStyle, "Style");
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Style", GUILayout.Width(EditorGUIUtility.labelWidth));

                        float intersectionStyle = GUILayout.Toolbar((int)_IntersectionStyle.floatValue,
                            new GUIContent[]
                            {
                                new GUIContent("None"), new GUIContent("Sharp"), new GUIContent("Smooth"),
                            }, GUILayout.MaxWidth((250f))
                            );
                        
                        if (EditorGUI.EndChangeCheck())
                            _IntersectionStyle.floatValue = intersectionStyle;
                    }

                    EditorGUI.showMixedValue = false;
                    
                    #if UNITY_2022_1_OR_NEWER
                    MaterialEditor.EndProperty();
                    #endif
                }

                if (_IntersectionStyle.floatValue > 0 || _IntersectionStyle.hasMixedValue)
                {
                    materialEditor.ShaderProperty(_IntersectionSource, new GUIContent("Gradient source", null, "The effect requires a grayscale gradient to work with, this sets what information should be used for this"));
                    if (_IntersectionSource.floatValue == 0 && _DisableDepthTexture.floatValue == 1f)
                    {
                        UI.DrawNotification("The depth texture option is disabled in the Advanced tab",
                            MessageType.Error);
                    }

                    materialEditor.TextureProperty(_IntersectionNoise, "Texture (R=Mask)");
                    UI.Material.DrawColorField(_IntersectionColor, true);
                    
                    materialEditor.ShaderProperty(_IntersectionLength, new GUIContent(_IntersectionLength.displayName, "Distance from objects/shore"));
                    materialEditor.ShaderProperty(_IntersectionFalloff, new GUIContent(_IntersectionFalloff.displayName, "The falloff represents a gradient"));
                    UI.Material.DrawFloatTicker(_IntersectionTiling);
                    UI.Material.DrawFloatTicker(_IntersectionSpeed, tooltip:"This value is multiplied by the Animation Speed value in the General tab");

                    if (_IntersectionStyle.floatValue == 1f || _IntersectionStyle.hasMixedValue)
                    {
                        materialEditor.ShaderProperty(_IntersectionClipping, new GUIContent(_IntersectionClipping.displayName, "Clips the effect based on its underlying grayscale values."));
                        UI.Material.DrawFloatTicker(_IntersectionRippleDist, _IntersectionRippleDist.displayName, "Distance between each ripples over the total intersection length");
                        materialEditor.ShaderProperty(_IntersectionRippleStrength, new GUIContent(_IntersectionRippleStrength.displayName, "Sets how much the ripples should be blended in with the effect"));
                    }
                }

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void DrawReflections()
        {
            reflectionSection.DrawHeader(() => SwitchSection(reflectionSection));

            if (EditorGUILayout.BeginFadeGroup(reflectionSection.anim.faded))
            {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Light reflections", EditorStyles.boldLabel);
                materialEditor.ShaderProperty(_SpecularReflectionsOn, "Enable");

                EditorGUI.indentLevel++;
                if (_SpecularReflectionsOn.floatValue > 0f || _SpecularReflectionsOn.hasMixedValue)
                {
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.LabelField("Directional Light", EditorStyles.boldLabel);

                    materialEditor.ShaderProperty(_SunReflectionStrength, new GUIContent("Strength", "This value is multiplied over the sun light's intensity"));
                    materialEditor.ShaderProperty(_SunReflectionSize, "Size");
                    materialEditor.ShaderProperty(_SunReflectionDistortion, new GUIContent("Distortion", "Note: Distortion is largely influenced by the strength of the normal map texture and wave curvature"));

                    if (_LightingOn.floatValue > 0f || _LightingOn.hasMixedValue)
                    {
                        EditorGUILayout.Space();
                        
                        EditorGUILayout.LabelField("Point/Spot lights", EditorStyles.boldLabel);
                        
                        materialEditor.ShaderProperty(_PointSpotLightReflectionStrength, new GUIContent("Strength", "This value is multiplied over the light's intensity"));
                        materialEditor.ShaderProperty(_PointSpotLightReflectionSize, new GUIContent("Size", "Specular reflection size for point/spot lights"));
                        materialEditor.ShaderProperty(_PointSpotLightReflectionDistortion, new GUIContent("Distortion", "Distortion is largely influenced by the strength of the normal map texture and wave curvature"));
                    }
                }
                EditorGUI.indentLevel--;

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Skybox/Reflection probes", EditorStyles.boldLabel);

                materialEditor.ShaderProperty(_EnvironmentReflectionsOn, new GUIContent("Enable", "Enabled reflections from the skybox and reflection probes (Note that URP does not support probe blending yet)"));
                #if UNITY_2022_1_OR_NEWER
                var customReflection = RenderSettings.customReflectionTexture;
                #else
                var customReflection = RenderSettings.customReflection;
                #endif
                if (_EnvironmentReflectionsOn.floatValue > 0 && RenderSettings.defaultReflectionMode == DefaultReflectionMode.Custom && !customReflection)
                {
                    UI.DrawNotification("Lighting settings: Environment reflections source is set to \"Custom\" without a cubemap assigned. No reflections will be visible", MessageType.Warning);
                }
                
                EditorGUILayout.Space();

                if (_EnvironmentReflectionsOn.floatValue > 0 || _EnvironmentReflectionsOn.hasMixedValue)
                {
                    materialEditor.ShaderProperty(_ReflectionStrength, _ReflectionStrength.displayName);
                    if (_LightingOn.floatValue > 0f || _LightingOn.hasMixedValue)
                    {
                        materialEditor.ShaderProperty(_ReflectionLighting, new GUIContent(_ReflectionLighting.displayName, "Technically, lighting shouldn't be applied to the reflected image. If reflections aren't updated in realtime, but lighting is, this is still beneficial.\n\nThis controls how much lighting affects the reflection"));
                    }
                    
                    #if !UNITY_2021_2_OR_NEWER
                    if (SceneView.lastActiveSceneView && SceneView.lastActiveSceneView.orthographic)
                    {
                        UI.DrawNotification("Reflection probes do not work with orthographic cameras until Unity 2021.2.0 (URP 12.0.0)", MessageType.Warning);
                    }
                    #endif
                    
                    EditorGUILayout.Space();
                    
                    materialEditor.ShaderProperty(_ReflectionFresnel, new GUIContent(_ReflectionFresnel.displayName, "Masks the reflection by the viewing angle in relationship to the surface (including wave curvature), which is more true to nature (known as fresnel)"));
                    materialEditor.ShaderProperty(_ReflectionDistortion, new GUIContent(_ReflectionDistortion.displayName, "Distorts the reflection by the wave normals and normal map"));
                    materialEditor.ShaderProperty(_ReflectionBlur, new GUIContent(_ReflectionBlur.displayName, "Blurs the reflection probe, this can be used for a more general reflection of colors"));
                }

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void DrawWaves()
        {
            wavesSection.DrawHeader(() => SwitchSection(wavesSection));
            
            if (EditorGUILayout.BeginFadeGroup(wavesSection.anim.faded))
            {
                EditorGUILayout.Space();
                
                UI.DrawNotification(_RiverModeOn.floatValue > 0 || _RiverModeOn.hasMixedValue, "Waves are automatically disabled when River Mode is enabled", MessageType.Info);

                using (new EditorGUI.DisabledGroupScope(_RiverModeOn.floatValue > 0 && !_RiverModeOn.hasMixedValue))
                {
                    materialEditor.ShaderProperty(_WavesOn, "Enable");
                    
                    EditorGUILayout.Space();
                    
                    if ((_WavesOn.floatValue == 1 || _WavesOn.hasMixedValue) && _RiverModeOn.floatValue < 1)
                    {
                        UI.Material.DrawFloatTicker(_WaveSpeed, label: "Speed multiplier");
                        materialEditor.ShaderProperty(_VertexColorWaveFlattening, new GUIContent("Vertex color flattening (B)",
                            "The Blue vertex color channel flattens waves\n\nNote: this does NOT affect buoyancy calculations!"));
                        
                        materialEditor.ShaderProperty(_WaveHeight, new GUIContent(_WaveHeight.displayName, "Waves will always push the water up from its base height, meaning waves never have a negative height"));
                       
                        UI.Material.DrawIntSlider(_WaveCount,
                            tooltip:
                            "Repeats the wave calculation X number of times, but with smaller waves each time");
     
                        Vector4 waveDir = _WaveDirection.vectorValue;
                        
                        #if UNITY_2022_1_OR_NEWER
                        MaterialEditor.BeginProperty(_WaveDirection);
                        #endif
                        
                        EditorGUI.BeginChangeCheck();
                        EditorGUI.showMixedValue = _WaveDirection.hasMixedValue;
                        Vector2 waveDir1;
                        Vector2 waveDir2;
                        waveDir1.x = waveDir.x;
                        waveDir1.y = waveDir.y;
                        waveDir2.x = waveDir.z;
                        waveDir2.y = waveDir.w;

                        EditorGUILayout.LabelField("Direction");
                        EditorGUI.indentLevel++;
                        waveDir1 = EditorGUILayout.Vector2Field("Sub layer 1 (Z)", waveDir1);
                        waveDir2 = EditorGUILayout.Vector2Field("Sub layer 2 (X)", waveDir2);
                        EditorGUI.indentLevel--;

                        waveDir = new Vector4(waveDir1.x, waveDir1.y, waveDir2.x, waveDir2.y);

                        if (EditorGUI.EndChangeCheck())
                        {
                            _WaveDirection.vectorValue = waveDir;
                        }
                        EditorGUI.showMixedValue = false;
                        
                        #if UNITY_2022_1_OR_NEWER
                        MaterialEditor.EndProperty();
                        #endif
                        
                        materialEditor.ShaderProperty(_WaveDistance, new GUIContent(_WaveDistance.displayName, "Distance between waves"));
                        materialEditor.ShaderProperty(_WaveSteepness, new GUIContent(_WaveSteepness.displayName, "Sharpness, depending on other settings here, a too high value will causes vertices to overlap. This also creates horizontal movement"));
                        materialEditor.ShaderProperty(_WaveNormalStr, new GUIContent(_WaveNormalStr.displayName, "Normals affect how curved the surface is perceived for direct and ambient light. Without this, the water will appear flat"));
                        
                        UI.Material.DrawMinMaxSlider(_WaveFadeDistance, 0f, 500f, "Fade Distance", "Fades out the waves between the start- and end distance. This can avoid tiling artifacts in the distance");
                    }
                }

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void DrawAdvanced(Material material)
        {
            advancedSection.DrawHeader(() => SwitchSection(advancedSection));

            if (EditorGUILayout.BeginFadeGroup(advancedSection.anim.faded))
            {
                EditorGUILayout.Space();

                materialEditor.ShaderProperty(_ZWrite, new GUIContent("Depth writing (ZWrite)", "Enable to have the water perform transparency sorting on itself. Advisable with high waves.\n\nIf this is disabled, other transparent materials will either render behind or in front of the water, depending on their render queue/priority set in their materials"));

                materialEditor.ShaderProperty(_DisableDepthTexture, new GUIContent("Disable depth texture", "Depth texture is used to measure the distance between the water surface and objects underneath it.\n\n" +
                                                                                                            "This is used for the color gradient and intersection effects"));
                EditorGUILayout.Space();
                
                EditorGUI.BeginChangeCheck();
                tesselationEnabled = EditorGUILayout.Toggle(
                    new GUIContent("Tessellation", "Dynamically subdivides the triangles to create denser topology near the camera." +
                                                                    "\n\nThis allows for more detailed wave animations." +
                                                                    "\n\nOnly supported on GPUs with Shader Model 4.6+. Should it fail, it will revert to the non-tessellated shader"), 
                                                                    tesselationEnabled);
                
                if (EditorGUI.EndChangeCheck())
                {
                    if(tesselationEnabled) AssignNewShaderToMaterial(material, material.shader, Shader.Find(TESSELLATION_SHADER_NAME));
                    else AssignNewShaderToMaterial(material, material.shader, Shader.Find(BASE_SHADER_NAME));
                }
                
                if (tesselationEnabled && _TessValue != null)
                {
                    UI.DrawNotification(_FlatShadingOn.floatValue > 0 || _FlatShadingOn.hasMixedValue, "Flat shading is enabled, tessellation should not be used to achieve the desired effect", MessageType.Warning);
                    
                    EditorGUI.indentLevel++;

                    materialEditor.ShaderProperty(_TessValue, _TessValue.displayName);
                    #if UNITY_PS4 || UNITY_XBOXONE || UNITY_GAMECORE
                    //AMD recommended performance optimization
                    EditorGUILayout.HelpBox("Value is internally limited to 15 for the current target platform (AMD-specific optimization)", MessageType.None);
                    #endif
                    UI.Material.DrawFloatField(_TessMin);
                    _TessMin.floatValue = Mathf.Clamp(_TessMin.floatValue, 0f, _TessMax.floatValue - 0.01f);
                    UI.Material.DrawFloatField(_TessMax);
                    EditorGUI.indentLevel--;
                    
                    UI.DrawNotification(material.enableInstancing, "Tessellation does not work correctly when GPU instancing is enabled", MessageType.Warning);
                }

                EditorGUILayout.Space();

                materialEditor.EnableInstancingField();
                materialEditor.RenderQueueField();

                if (material.renderQueue <= 2450 || material.renderQueue >= 3500)
                {
                    UI.DrawNotification("Material must be on the Transparent render queue (~3000). Otherwise incurs rendering artefacts", MessageType.Error);
                }
                //materialEditor.DoubleSidedGIField();

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void SwitchSection(UI.Material.Section s)
        {
            generalSection.Expanded = (s == generalSection) && !generalSection.Expanded;
            lightingSection.Expanded = (s == lightingSection) && !lightingSection.Expanded;
            colorSection.Expanded = (s == colorSection) && !colorSection.Expanded;
            underwaterSection.Expanded = (s == underwaterSection) && !underwaterSection.Expanded;
            normalsSection.Expanded = (s == normalsSection) && !normalsSection.Expanded;
            reflectionSection.Expanded = (s == reflectionSection) && !reflectionSection.Expanded;
            intersectionSection.Expanded = (s == intersectionSection) && !intersectionSection.Expanded;
            foamSection.Expanded = (s == foamSection) && !foamSection.Expanded;
            wavesSection.Expanded = (s == wavesSection) && !wavesSection.Expanded;
            advancedSection.Expanded = (s == advancedSection) && !advancedSection.Expanded;

            /*
            generalSection.Expanded = true;
            lightingSection.Expanded = true;
            colorSection.Expanded = true;
            underwaterSection.Expanded = true;
            normalsSection.Expanded = true;
            reflectionSection.Expanded = true;
            intersectionSection.Expanded = true;
            foamSection.Expanded = true;
            wavesSection.Expanded = true;
            advancedSection.Expanded = true;
            */
        }
        #endregion
        
        #region Upgrading
        private void UpgradeProperties(Material material)
        {
            //Upgrade to v1.3.0+
            if (EditorUtility.DisplayDialog(AssetInfo.ASSET_NAME, $"The selected water material needs to be updated to the format of v{AssetInfo.INSTALLED_VERSION}." +
                                                                  "\n\nAfter this automatic process, the material UI will support multi-selection and the Material Variants feature introduced in Unity 2022.1", "OK"))
                
            {
                //Ensure keyword states are synced with their float property counterpart
                if (material.IsKeywordEnabled("_UNLIT") && _LightingOn.floatValue > 0) _LightingOn.floatValue = 0f;
                if (!material.IsKeywordEnabled("_UNLIT") && _LightingOn.floatValue < 1f) _LightingOn.floatValue = 1;
                
                if (material.IsKeywordEnabled("_RECEIVE_SHADOWS_OFF") && _ReceiveShadows.floatValue > 0) _ReceiveShadows.floatValue = 0f;
                if (!material.IsKeywordEnabled("_RECEIVE_SHADOWS_OFF") && _ReceiveShadows.floatValue < 1f) _ReceiveShadows.floatValue = 1;

                if (material.IsKeywordEnabled("_FLAT_SHADING") && _FlatShadingOn.floatValue < 1f) _FlatShadingOn.floatValue = 1f;
                if (!material.IsKeywordEnabled("_FLAT_SHADING") && _FlatShadingOn.floatValue > 0) _FlatShadingOn.floatValue = 0;
                
                if (material.IsKeywordEnabled("_RIVER") && _RiverModeOn.floatValue < 1f) _RiverModeOn.floatValue = 1f;
                if (!material.IsKeywordEnabled("_RIVER") && _RiverModeOn.floatValue > 0) _RiverModeOn.floatValue = 0;

                if (material.IsKeywordEnabled("_FOAM") && _FoamOn.floatValue < 1f) _FoamOn.floatValue = 1f;
                if (!material.IsKeywordEnabled("_FOAM") && _FoamOn.floatValue > 0) _FoamOn.floatValue = 0;

                if (material.IsKeywordEnabled("_DISTANCE_NORMALS") && _DistanceNormalsOn.floatValue < 1f) _DistanceNormalsOn.floatValue = 1f;
                if (!material.IsKeywordEnabled("_DISTANCE_NORMALS") && _DistanceNormalsOn.floatValue > 0) _DistanceNormalsOn.floatValue = 0;

                if (material.IsKeywordEnabled("_TRANSLUCENCY") && _TranslucencyOn.floatValue < 1f) _TranslucencyOn.floatValue = 1f;
                if (!material.IsKeywordEnabled("_TRANSLUCENCY") && _TranslucencyOn.floatValue > 0) _TranslucencyOn.floatValue = 0;
                
                if (material.IsKeywordEnabled("_SPECULARHIGHLIGHTS_OFF") && _SpecularReflectionsOn.floatValue > 0) _SpecularReflectionsOn.floatValue = 0;
                if (!material.IsKeywordEnabled("_SPECULARHIGHLIGHTS_OFF") && _SpecularReflectionsOn.floatValue < 1f) _SpecularReflectionsOn.floatValue = 1;
                
                if (material.IsKeywordEnabled("_ENVIRONMENTREFLECTIONS_OFF") && _EnvironmentReflectionsOn.floatValue > 0) _EnvironmentReflectionsOn.floatValue = 0;
                if (!material.IsKeywordEnabled("_ENVIRONMENTREFLECTIONS_OFF") && _EnvironmentReflectionsOn.floatValue < 1f) _EnvironmentReflectionsOn.floatValue = 1;

                //Translucency settings
                {
                    Vector4 _TranslucencyParams = GetLegacyVectorProperty(material, "_TranslucencyParams");

                    _TranslucencyStrength.floatValue = _TranslucencyParams.x;
                    _TranslucencyExp.floatValue = _TranslucencyParams.y;
                    _TranslucencyCurvatureMask.floatValue = _TranslucencyParams.z;

                    DeleteProperty(material, "_TranslucencyParams");

                }
                
                //Animation direction/speed
                {
                    Vector4 _AnimationParams = GetLegacyVectorProperty(material, "_AnimationParams");

                    _Direction.vectorValue = new Vector4(_AnimationParams.x, _AnimationParams.y, 0f, 0f);
                    _Speed.floatValue = _AnimationParams.z;

                    DeleteProperty(material, "_AnimationParams");
                }
                
                //River slope stretching/speed
                {
                    Vector4 _SlopeParams = GetLegacyVectorProperty(material, "_SlopeParams");

                    _SlopeStretching.floatValue = _SlopeParams.x;
                    _SlopeSpeed.floatValue = _SlopeParams.y;

                    DeleteProperty(material, "_SlopeParams");
                }
                
                //Vertex color options
                {
                    Vector4 _VertexColorMask = GetLegacyVectorProperty(material, "_VertexColorMask");

                    _VertexColorDepth.floatValue = _VertexColorMask.y;
                    _VertexColorWaveFlattening.floatValue = _VertexColorMask.z;
                    _VertexColorFoam.floatValue = _VertexColorMask.w;

                    DeleteProperty(material, "_VertexColorMask");
                }
                
                //Distance normals
                {
                    Vector4 _DistanceNormalParams = GetLegacyVectorProperty(material, "_DistanceNormalParams");

                    _DistanceNormalsFadeDist.vectorValue = new Vector2(_DistanceNormalParams.x, _DistanceNormalParams.y);

                    DeleteProperty(material, "_DistanceNormalParams");
                }

                Debug.LogFormat("Auto upgraded material <b>\"{0}\"</b> properties to v{1}", material.name, AssetInfo.INSTALLED_VERSION);
                
                EditorUtility.SetDirty(material);
                AssetDatabase.SaveAssets();
            }
            
        }

        private Vector4 GetLegacyVectorProperty(Material mat, string name)
        {
            SerializedObject materialObj = new SerializedObject(mat);
            
            //Note: Vectors are actually stored as colors
            SerializedProperty vectorProperties = materialObj.FindProperty("m_SavedProperties.m_Colors");

            Vector4 vectorProp = Vector4.one * Mathf.NegativeInfinity;
            
            if (vectorProperties != null && vectorProperties.isArray) 
            {
                for (int j = vectorProperties.arraySize-1; j >= 0; j--) 
                {
                    string propName = vectorProperties.GetArrayElementAtIndex(j).displayName;

                    if (propName == name)
                    {
                        SerializedProperty val = vectorProperties.GetArrayElementAtIndex(j).FindPropertyRelative("second");
                        //Debug.Log("Found " + propName + " " + val.colorValue);

                        Color col = val.colorValue;
                        
                        return new Vector4(col.r, col.g, col.b, col.a);
                    }
                }
            }

            return vectorProp;
        }
        
        private void DeleteProperty(Material mat, string name)
        {
            SerializedObject materialObj = new SerializedObject(mat);
            
            SerializedProperty vectorProperties = materialObj.FindProperty("m_SavedProperties.m_Colors");
            
            if (vectorProperties != null && vectorProperties.isArray) 
            {
                for (int j = vectorProperties.arraySize-1; j >= 0; j--) 
                {
                    string propName = vectorProperties.GetArrayElementAtIndex(j).displayName;

                    if (propName == name) 
                    {
                        vectorProperties.DeleteArrayElementAtIndex(j);
                        materialObj.ApplyModifiedProperties();
                        
                        EditorUtility.SetDirty(mat);
                        
                        //Debug.Log("Deleted obsolete material property: " + name);
                    }
                }
            }
        }
        #endregion
#else
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            UI.DrawNotification("The Universal Render Pipeline package v" + AssetInfo.MIN_URP_VERSION + " or newer is not installed", MessageType.Error);
        }
#endif
    }
}