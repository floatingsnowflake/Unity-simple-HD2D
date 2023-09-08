//Stylized Water 2
//Staggart Creations (http://staggart.xyz)
//Copyright protected under Unity Asset Store EULA

Shader "Universal Render Pipeline/FX/Stylized Water 2"
{
	Properties
	{
		//[Header(Rendering)]
		[Toggle] _ZWrite("Depth writing", Float) = 0
		[MaterialEnum(Doublesided, 0, Frontfaces, 1, Backfaces, 2)] _Cull("Culling", Float) = 2
		[MaterialEnum(Simple, 0,Advanced, 1)] _ShadingMode("Shading mode", Float) = 1

		//[Header(Feature switches)]
		_Direction("Animation direction", Vector) = (1,1,0,0)
		_Speed("Animation Speed", Float) = 1
		
		_SlopeStretching("Slope UV stretch", Float) = 0.5
		_SlopeSpeed("Slope speed multiplier", Float) = 2
		_SlopeThreshold("Slope threshold", Range(0 , 1)) = 0.25
		[MaterialEnum(Mesh UV,0,World XZ projected ,1)]_WorldSpaceUV("UV Coordinates", Float) = 1

		//[Header(Color)]
		[HDR]_BaseColor("Deep", Color) = (0, 0.44, 0.62, 1)
		[HDR]_ShallowColor("Shallow", Color) = (0.1, 0.9, 0.89, 0.02)
		_WaveTint("Wave tint", Range( -0.1 , 0.1)) = 0
		[HDR]_HorizonColor("Horizon", Color) = (0.84, 1, 1, 0.15)
		_HorizonDistance("Horizon Distance", Range(0.01 , 32)) = 8
		[Toggle] _VertexColorDepth("Vertex color (G) depth", Float) = 0
		_DepthVertical("View Depth", Range(0.01 , 16)) = 4
		_DepthHorizontal("Vertical Height Depth", Range(0.01 , 8)) = 1
		[Toggle] _DepthExp("Exponential Blend", Float) = 1
		_EdgeFade("Edge Fade", Float) = 0.1
		_ShadowStrength("Shadow Strength", Range(0 , 1)) = 1

		//_Smoothness("Smoothness", Range(0.0, 1.0)) = 0.9
		//_Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		
		_TranslucencyStrength("Translucency Strength", Range(0 , 3)) = 1
		_TranslucencyExp("Translucency Exponent", Range(1 , 32)) = 8
		_TranslucencyCurvatureMask("Translucency Curvature mask", Range(0, 1)) = 0
		_TranslucencyReflectionMask("Translucency Reflection mask", Range(0, 1)) = 0

		//[Header(Underwater)]
		_CausticsBrightness("Brightness", Float) = 2
		_CausticsTiling("Tiling", Float) = 0.5
		_CausticsSpeed("Speed multiplier", Float) = 0.1
		_CausticsDistortion("Distortion", Range(0, 1)) = 0.15
		[NoScaleOffset][SingleLineTexture]_CausticsTex("Caustics RGB", 2D) = "black" {}
		
		_UnderwaterSurfaceSmoothness("Underwater Surface Smoothness", Range(0, 1)) = 0.8
		_UnderwaterRefractionOffset("Underwater Refraction Offset", Range(0, 1)) = 0.2
		
		_RefractionStrength("_RefractionStrength", Range(0 , 3)) = 0.1

		//[Header(Intersection)]
		[MaterialEnum(Depth Texture,0,Vertex Color (R),1,Depth Texture and Vertex Color,2)] _IntersectionSource("Intersection source", Float) = 0
		[MaterialEnum(None,0,Sharp,1,Smooth,2)] _IntersectionStyle("Intersection style", Float) = 1

		[NoScaleOffset][SingleLineTexture]_IntersectionNoise("Intersection noise", 2D) = "white" {}
		_IntersectionColor("Color", Color) = (1,1,1,1)
		_IntersectionLength("Distance", Range(0.01 , 5)) = 2
		_IntersectionClipping("Cutoff", Range(0.01, 1)) = 0.5
		_IntersectionFalloff("Falloff", Range(0.01 , 1)) = 0.5
		_IntersectionTiling("Noise Tiling", float) = 0.2
		_IntersectionSpeed("Speed multiplier", float) = 0.1
		_IntersectionRippleDist("Ripple distance", float) = 32
		_IntersectionRippleStrength("Ripple Strength", Range(0 , 1)) = 0.5

		//[Header(Foam)]
		[NoScaleOffset][SingleLineTexture]_FoamTex("Foam Mask", 2D) = "black" {}
		_FoamColor("Color", Color) = (1,1,1,1)
		_FoamSize("Cutoff", Range(0.01 , 0.999)) = 0.01
		_FoamSpeed("Speed multiplier", float) = 0.1
		_FoamWaveMask("Wave mask", Range(0 , 1)) = 0
		_FoamWaveMaskExp("Wave mask exponent", Range(1 , 8)) = 1
		_FoamTiling("Tiling", float) = 0.1
		[Toggle] _VertexColorFoam("Vertex color (A) foam", Float) = 0

		//[Header(Normals)]
		[NoScaleOffset][Normal][SingleLineTexture]_BumpMap("Normals", 2D) = "bump" {}
		[NoScaleOffset][Normal][SingleLineTexture]_BumpMapSlope("Normals (River slopes)", 2D) = "bump" {}
		_NormalTiling("Tiling", Float) = 1
		_NormalStrength("Strength", Range(0 , 1)) = 0.5
		_NormalSpeed("Speed multiplier", Float) = 0.2
		
		[NoScaleOffset][Normal][SingleLineTexture]_BumpMapLarge("Normals (Distance)", 2D) = "bump" {}
		_DistanceNormalsFadeDist("Distance normals blend (Start/End)", Vector) = (100, 300, 0, 0)
		_DistanceNormalsTiling("Distance normals: Tiling multiplier", Float) = 0.25

		_SparkleIntensity("Sparkle Intensity", Range(0 , 10)) = 00
		_SparkleSize("Sparkle Size", Range( 0 , 1)) = 0.280

		//[Header(Sun Reflection)]
		[PowerSlider(0.1)] _SunReflectionSize("Sun Size", Range(0 , 1)) = 0.5
		_SunReflectionStrength("Sun Strength", Float) = 10
		_SunReflectionDistortion("Sun Distortion", Range(0 ,2)) = 0.49
		_PointSpotLightReflectionStrength("Point/spot light strength", Float) = 10
		[PowerSlider(0.1)] _PointSpotLightReflectionSize("Point/spot light size", Range(0 , 1)) = 0
		_PointSpotLightReflectionDistortion("Point/spot light distortion", Range(0, 1)) = 0.5

		//[Header(World Reflection)]
		_ReflectionStrength("Strength", Range( 0 , 1)) = 0
		_ReflectionDistortion("Distortion", Range( 0 , 2)) = 0.05
		_ReflectionBlur("Blur", Range( 0 , 1)) = 0	
		_ReflectionFresnel("Curvature mask", Range( 0.01 , 20)) = 5	
		_ReflectionLighting("Lighting influence", Range( 0 , 1)) = 0	
		_PlanarReflection("Planar Reflections", 2D) = "" {} //Instanced
		_PlanarReflectionsEnabled("Planar Enabled", float) = 0 //Instanced
		
		//[Header(Waves)]
		_WaveSpeed("Speed", Float) = 2
		_WaveHeight("Height", Range(0 , 10)) = 0.25
		[Toggle] _VertexColorWaveFlattening("Vertex color (B) wave flattening", Float) = 0

		_WaveNormalStr("Normal Strength", Range(0 , 6)) = 0.5
		_WaveDistance("Distance", Range(0 , 1)) = 0.8
		_WaveFadeDistance("Wave fade distance (Start/End)", Vector) = (150, 300, 0, 0)

		_WaveSteepness("Steepness", Range(0 , 5)) = 0.1
		_WaveCount("Count", Range(1 , 5)) = 1
		_WaveDirection("Direction", vector) = (1,1,1,1)
		
		//Keyword states
		[ToggleOff(_UNLIT)] _LightingOn("Enable lighting", Float) = 1
		[ToggleOff(_RECEIVE_SHADOWS_OFF)] _ReceiveShadows("Recieve Shadows", Float) = 1

		[Toggle(_FLAT_SHADING)] _FlatShadingOn("Flat shading", Float) = 0
		[Toggle(_TRANSLUCENCY)] _TranslucencyOn("Enable translucency shading", Float) = 1
		[Toggle(_REFRACTION)] _RefractionOn("_REFRACTION", Float) = 1
		[Toggle(_RIVER)] _RiverModeOn("River Mode", Float) = 0
		[Toggle(_CAUSTICS)] _CausticsOn("Caustics ON", Float) = 1
		[ToggleOff(_SPECULARHIGHLIGHTS_OFF)] _SpecularReflectionsOn("Specular Reflections", Float) = 1
		[ToggleOff(_ENVIRONMENTREFLECTIONS_OFF)] _EnvironmentReflectionsOn("Environment Reflections", Float) = 1
		[Toggle(_NORMALMAP)] _NormalMapOn("Normal maps", Float) = 1
		[Toggle(_DISTANCE_NORMALS)] _DistanceNormalsOn("Distance normal map", Float) = 1
		[Toggle(_FOAM)] _FoamOn("Foam", Float) = 1
		[Toggle(_DISABLE_DEPTH_TEX)] _DisableDepthTexture("Disable depth texture", Float) = 0
		[Toggle(_WAVES)] _WavesOn("_WAVES", Float) = 0

		/* start Tessellation */
		//_TessValue("Max subdivisions", Range(1, 32)) = 16
		//_TessMin("Start Distance", Float) = 0
		//_TessMax("End Distance", Float) = 15
 		/* end Tessellation */
		
		//[CurvedWorldBendSettings] _CurvedWorldBendSettings("0|1|1", Vector) = (0, 0, 0, 0)
	}

	SubShader
	{		
		Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" "Queue" = "Transparent" }
				
		Pass
		{	
			Name "ForwardLit"
			Tags { "LightMode"="UniversalForward" }
			
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite [_ZWrite]
			Cull [_Cull]
			/* start COZY */
//			Stencil { Ref 221 Comp Always Pass Replace }
			/* end COZY */
			ZTest LEqual
			
			HLSLPROGRAM

			#pragma multi_compile_instancing
			/* start UnityFog */
			#pragma multi_compile_fog
			/* end UnityFog */

			#pragma target 3.0

			#define _SURFACE_TYPE_TRANSPARENT 1
			
			// Material Keywords
			#pragma shader_feature_local _NORMALMAP
			#pragma shader_feature_local _WAVES
			#pragma shader_feature_local _FLAT_SHADING
			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma shader_feature_local _RIVER
			#pragma shader_feature_local_fragment _DISABLE_DEPTH_TEX
			#pragma shader_feature_local_fragment _REFRACTION
			#pragma shader_feature_local_fragment _ADVANCED_SHADING
			#pragma shader_feature_local_fragment _UNLIT
			#pragma shader_feature_local_fragment _CAUSTICS
			#pragma shader_feature_local_fragment _DISTANCE_NORMALS
			#pragma shader_feature_local_fragment _FOAM
			#pragma shader_feature_local_fragment _TRANSLUCENCY
			#pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
			#pragma shader_feature_local_fragment _ _SHARP_INERSECTION _SMOOTH_INTERSECTION

			//Will be stripped, if extensions aren't installed
			#pragma multi_compile_fragment _ UNDERWATER_ENABLED
			//#pragma multi_compile _ MODIFIERS_ENABLED
			//#pragma multi_compile _ WAVE_SIMULATION

			#if !_ADVANCED_SHADING
			#define _SIMPLE_SHADING
			#endif

			#if _RIVER
			#undef _WAVES
			#undef UNDERWATER_ENABLED
			#endif

			//Required to differentiate between skybox and scene geometry
			#if UNDERWATER_ENABLED
			#undef _DISABLE_DEPTH_TEX 
			#endif
			
			//Caustics require depth texture
			#if _DISABLE_DEPTH_TEX
			#undef _CAUSTICS
			#endif
			
			//Requires some form of per-pixel offset
			#if !_NORMALMAP && !_WAVES
			#undef _REFRACTION
			#endif
			
			//Unity global keywords
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
			//Uncomment to support a single shadow cascade in Unity 2020.3
			//#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

			//Half-fix for shadow cascades breaking in 2020.3, due to keywords following a set up needed to support newer versions
			#if UNITY_VERSION < 202110 && _MAIN_LIGHT_SHADOWS
			#define _MAIN_LIGHT_SHADOWS_CASCADE 0
			#endif
			
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT
			
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS //URP 11+
			//Tiny use-case, disabled to reduce variants (each adds about 200-500)
			//#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING			

			//Stripped during building on older versions
			//URP 12+ only (2021.2+)
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fragment _ DEBUG_DISPLAY
			//#pragma multi_compile_fragment _ _LIGHT_LAYERS

			//URP 14+ (2022.2+)
			#pragma multi_compile _ _FORWARD_PLUS
			//#pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS

			#include "Libraries/URP.hlsl"

			/* start AtmosphericHeightFog */
//			#pragma multi_compile AHF_NOISEMODE_OFF AHF_NOISEMODE_PROCEDURAL3D
			/* end AtmosphericHeightFog */

			//Defines
			#define SHADERPASS_FORWARD
			
			/* start Tessellation */
//			#define TESSELLATION_ON
//			#pragma require tessellation tessHW
//			#pragma hull Hull
//			#pragma domain Domain
			/* end Tessellation */
			
			#pragma vertex Vertex
			#pragma fragment ForwardPassFragment

			#include "Libraries/Input.hlsl"

			//Uncommenting and rewriting is handled by the Curved World 2020 asset
			//#define CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_X_POSITIVE
			//#define CURVEDWORLD_BEND_ID_1
			//#pragma shader_feature_local CURVEDWORLD_DISABLED_ON
			//#pragma shader_feature_local CURVEDWORLD_NORMAL_TRANSFORMATION_ON
			//#include "Assets/Amazing Assets/Curved World/Shaders/Core/CurvedWorldTransform.cginc"
			
			#include "Libraries/Common.hlsl"
			#include "Libraries/Fog.hlsl"
			#include "Libraries/Waves.hlsl"
			#include "Libraries/Lighting.hlsl"

			#ifdef UNDERWATER_ENABLED
			#include "Underwater/UnderwaterFog.hlsl"
			#include "Underwater/UnderwaterShading.hlsl"
			#endif

			#ifdef WAVE_SIMULATION
			#include "Libraries/Simulation/Simulation.hlsl"
			#endif

			#include "Libraries/Features.hlsl"
			#include "Libraries/Caustics.hlsl"

			#if MODIFIERS_ENABLED
			#include "SurfaceModifiers/SurfaceModifiers.hlsl"
			#endif
			
			#include "Libraries/Vertex.hlsl"

			/* start Tessellation */
//			#include "Libraries/Tesselation.hlsl"
			/* end Tessellation */

			Varyings Vertex(Attributes v)
			{
				return LitPassVertex(v);
			}

			#include "Libraries/ForwardPass.hlsl"
			
			ENDHLSL
		}
		
		//Currently unused, except for prototypes (such as depth texture injection)
		Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }
            
            ZWrite On
			//ColorMask RG
            Cull Off

            HLSLPROGRAM
            #pragma target 3.0
            #pragma multi_compile_instancing

            #pragma shader_feature_local _WAVES

            /* start Tessellation */
//			#define TESSELLATION_ON
//			#pragma require tessellation tessHW
//			#pragma hull Hull
//			#pragma domain Domain
			/* end Tessellation */
            
            #pragma vertex Vertex
            #pragma fragment DepthOnlyFragment

            #define SHADERPASS_DEPTHONLY

            #include "Libraries/URP.hlsl"
            #include "Libraries/Input.hlsl"

			//#define CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_X_POSITIVE
			//#define CURVEDWORLD_BEND_ID_1
			//#pragma shader_feature_local CURVEDWORLD_DISABLED_ON
			//#pragma shader_feature_local CURVEDWORLD_NORMAL_TRANSFORMATION_ON
			//#include "Assets/Amazing Assets/Curved World/Shaders/Core/CurvedWorldTransform.cginc"

            #include "Libraries/Common.hlsl"
            #include "Libraries/Fog.hlsl"
            #include "Libraries/Waves.hlsl"

            #include "Libraries/Vertex.hlsl"

            /* start Tessellation */
//          #include "Libraries/Tesselation.hlsl"
            /* end Tessellation */

            Varyings Vertex(Attributes v)
            {
                return LitPassVertex(v);
            }

            half4 DepthOnlyFragment(Varyings input, FRONT_FACE_TYPE facing : FRONT_FACE_SEMANTIC) : SV_TARGET
            {
				UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

            	float depth = input.positionCS.z;

                return float4(depth, facing, 0, 0);
            }
            ENDHLSL

        }
	}

	CustomEditor "StylizedWater2.MaterialUI"
	Fallback "Hidden/InternalErrorShader"	
}
