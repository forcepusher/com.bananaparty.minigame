Shader "Custom/StandardWithOverrides" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo", 2D) = "white" {}

        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Glossiness ("Smoothness", Range(0.0, 1.0)) = 0.5
        [Gamma] _Metallic ("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap ("Metallic", 2D) = "white" {}

        _BumpScale ("Scale", Float) = 1.0
        _BumpMap ("Normal Map", 2D) = "bump" {}

        _Parallax ("Height Scale", Range(0.005, 0.08)) = 0.02
        _ParallaxMap ("Height Map", 2D) = "black" {}

        _OcclusionStrength ("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap ("Occlusion", 2D) = "white" {}

        _EmissionColor ("Color", Color) = (0,0,0)
        _EmissionMap ("Emission", 2D) = "white" {}

        _DetailMask ("Detail Mask", 2D) = "white" {}
        _DetailAlbedoMap ("Detail Albedo x2", 2D) = "grey" {}
        _DetailNormalMapScale ("Scale", Float) = 1.0
        _DetailNormalMap ("Normal Map", 2D) = "bump" {}

        [Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0

        // Blending state (used by StandardShaderGUI for Rendering Mode)
        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0

        // Per-scene ambient override
        _AmbientColor ("Ambient Color", Color) = (0.2,0.2,0.2,1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 300

        Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]

        CGPROGRAM
        #pragma surface surf CustomAmbient fullforwardshadows
        #pragma target 3.0
        #pragma exclude_renderers gles

        #pragma shader_feature _NORMALMAP
        #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
        #pragma shader_feature _EMISSION
        #pragma shader_feature _METALLICGLOSSMAP
        #pragma shader_feature _ _DETAIL_MULX2
        #pragma shader_feature _PARALLAXMAP

        #include "UnityPBSLighting.cginc"
        #include "UnityStandardUtils.cginc"
        #include "Lighting.cginc"
        #include "AutoLight.cginc"

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _MetallicGlossMap;
        sampler2D _ParallaxMap;
        sampler2D _OcclusionMap;
        sampler2D _EmissionMap;
        sampler2D _DetailMask;
        sampler2D _DetailAlbedoMap;
        sampler2D _DetailNormalMap;

        half _Glossiness;
        half _Metallic;
        half _BumpScale;
        half _Parallax;
        half _OcclusionStrength;
        half _DetailNormalMapScale;
        half _Cutoff;

        fixed4 _Color;
        fixed4 _EmissionColor;
        fixed4 _AmbientColor;

        struct Input {
            float2 uv_MainTex;
            float2 uv_DetailAlbedoMap;
            float3 viewDir;
        };

        inline void LightingCustomAmbient_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi) {
            LightingStandard_GI(s, data, gi);
            gi.indirect.diffuse = _AmbientColor.rgb;
        }

        inline half4 LightingCustomAmbient(SurfaceOutputStandard s, half3 viewDir, UnityGI gi) {
            return LightingStandard(s, viewDir, gi);
        }

        void surf(Input IN, inout SurfaceOutputStandard o) {
            float2 uv = IN.uv_MainTex;

            #ifdef _PARALLAXMAP
                half h = tex2D(_ParallaxMap, uv).g;
                float2 offset = ParallaxOffset(h, _Parallax, IN.viewDir);
                uv += offset;
            #endif

            fixed4 c = tex2D(_MainTex, uv) * _Color;
            o.Albedo = c.rgb;

            #ifdef _METALLICGLOSSMAP
                fixed4 mg = tex2D(_MetallicGlossMap, uv);
                o.Metallic = mg.r;
                o.Smoothness = mg.a;
            #else
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
            #endif

            #ifdef _NORMALMAP
                o.Normal = UnpackScaleNormal(tex2D(_BumpMap, uv), _BumpScale);
            #else
                o.Normal = half3(0, 0, 1);
            #endif

            #ifdef _DETAIL_MULX2
                fixed4 detailMask = tex2D(_DetailMask, uv);
                fixed4 detailAlbedo = tex2D(_DetailAlbedoMap, IN.uv_DetailAlbedoMap);
                o.Albedo *= 2.0 * lerp(fixed3(1,1,1), detailAlbedo.rgb, detailMask.r);
                #ifdef _NORMALMAP
                    half3 detailNormal = UnpackScaleNormal(tex2D(_DetailNormalMap, IN.uv_DetailAlbedoMap), _DetailNormalMapScale);
                    o.Normal = BlendNormals(o.Normal, detailNormal);
                #endif
            #endif

            o.Occlusion = lerp(1.0, tex2D(_OcclusionMap, uv).g, _OcclusionStrength);

            o.Emission = _EmissionColor.rgb * tex2D(_EmissionMap, uv).rgb;

            o.Alpha = c.a;

            #ifdef _ALPHATEST_ON
                clip(o.Alpha - _Cutoff);
            #endif
        }
        ENDCG
    }

    SubShader {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 150

        Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]

        CGPROGRAM
        #pragma surface surf CustomAmbient fullforwardshadows
        #pragma target 2.0

        #pragma shader_feature _NORMALMAP
        #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
        #pragma shader_feature _EMISSION
        #pragma shader_feature _METALLICGLOSSMAP
        #pragma shader_feature _ _DETAIL_MULX2

        #include "UnityPBSLighting.cginc"
        #include "Lighting.cginc"
        #include "AutoLight.cginc"

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _MetallicGlossMap;
        sampler2D _OcclusionMap;
        sampler2D _EmissionMap;
        sampler2D _DetailMask;
        sampler2D _DetailAlbedoMap;
        sampler2D _DetailNormalMap;

        half _Glossiness;
        half _Metallic;
        half _BumpScale;
        half _OcclusionStrength;
        half _DetailNormalMapScale;
        half _Cutoff;

        fixed4 _Color;
        fixed4 _EmissionColor;
        fixed4 _AmbientColor;

        struct Input {
            float2 uv_MainTex;
            float2 uv_DetailAlbedoMap;
        };

        inline void LightingCustomAmbient_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi) {
            LightingStandard_GI(s, data, gi);
            gi.indirect.diffuse = _AmbientColor.rgb;
        }

        inline half4 LightingCustomAmbient(SurfaceOutputStandard s, half3 viewDir, UnityGI gi) {
            return LightingStandard(s, viewDir, gi);
        }

        void surf(Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            #ifdef _METALLICGLOSSMAP
                fixed4 mg = tex2D(_MetallicGlossMap, IN.uv_MainTex);
                o.Metallic = mg.r;
                o.Smoothness = mg.a;
            #else
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
            #endif

            #ifdef _NORMALMAP
                o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex), _BumpScale);
            #else
                o.Normal = half3(0, 0, 1);
            #endif

            #ifdef _DETAIL_MULX2
                fixed4 detailMask = tex2D(_DetailMask, IN.uv_MainTex);
                fixed4 detailAlbedo = tex2D(_DetailAlbedoMap, IN.uv_DetailAlbedoMap);
                o.Albedo *= 2.0 * lerp(fixed3(1,1,1), detailAlbedo.rgb, detailMask.r);
                #ifdef _NORMALMAP
                    half3 detailNormal = UnpackScaleNormal(tex2D(_DetailNormalMap, IN.uv_DetailAlbedoMap), _DetailNormalMapScale);
                    o.Normal = BlendNormals(o.Normal, detailNormal);
                #endif
            #endif

            o.Occlusion = lerp(1.0, tex2D(_OcclusionMap, IN.uv_MainTex).g, _OcclusionStrength);
            o.Emission = _EmissionColor.rgb * tex2D(_EmissionMap, IN.uv_MainTex).rgb;
            o.Alpha = c.a;

            #ifdef _ALPHATEST_ON
                clip(o.Alpha - _Cutoff);
            #endif
        }
        ENDCG
    }

    FallBack "Standard"
    CustomEditor "StandardWithOverridesGUI"
}

