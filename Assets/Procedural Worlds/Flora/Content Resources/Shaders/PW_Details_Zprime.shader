Shader "PWS/Details/Zprime"
{
    Properties
    {
        [Header(PBR)]
        _MipBias ("Mip Bias", Range(-8,8)) = 0
        [HDR]_BaseColor ("Base Color", Color) = (1,1,1,1)
        _BaseMap ("Base Map (RGB)", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "Normal" {}
        _MaskMap ("PBR Mask Map", 2D) = "white" {}
        _MaskMapMod("Mask Map modifiers", Vector) = (1,1,1,1)
        _Thickness("Thickness", Range(0, 1)) = 0
        [Space]
        
        [Header(Alpha)]
        _Cutoff ("Alpha Cut Off", Range(0,1)) = 0.05
        [Space]
        
        //wind
        [Header(Wind Options)]
        [Toggle(_PW_SF_WIND_ON)]        
        _PW_SF_WIND("Enable Wind", Int) = 0
        _PW_WindTreeFlex(" Wind Detail Flex", Vector) = (0.8,1.15,0.1,0)
        _PW_WindTreeFrequency (" Wind Detail Frequency", Vector) = (0.25,0.5,1.3,0)
        _PW_WindTreeWidthHeight (" Wind Detail Height", Vector) = (1,1,0.66,0.66)
    }
    SubShader // HDRP
    {
         Tags{ "RenderPipeline" = "HDRenderPipeline" "RenderType" = "HDUnlitShader" }
         Pass
        {
            Name "DepthForwardOnly"
            Tags{ "LightMode" = "DepthForwardOnly" }
            cull off
            colormask 0
            
            CGPROGRAM

            #pragma multi_compile_instancing
            #pragma shader_feature_local _PW_SF_WIND_ON
            #pragma instancing_options procedural:setup assumeuniformscaling nolodfade 
            
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Assets/Procedural Worlds/Flora/Content Resources/Shaders/PW_FloraIncludes.hlsl"
            #include "Assets/Procedural Worlds/Flora/Content Resources/Shaders/PW_FloraWInd.cginc"
            

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
                float2 distFade: TEXCOORD2;
            };

            //General
            sampler2D _BaseMap, _NormalMap, _MaskMap;
            half4 _BaseColor, _ColorA, _ColorB, _BaseMap_ST;
            half _Cutoff;
            float4 _MaskMapMod;
            float _Thickness,_MipBias;

            //wind
            float2 _PW_WindTreeWidthHeight;
            float4 _PW_WindTreeFlex, _PW_WindTreeFrequency;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                WindCalculations_float(v.vertex.xyz,_PW_WindTreeWidthHeight.xy,_PW_WindTreeFlex,_PW_WindTreeFrequency,v.vertex.xyz);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv,_BaseMap);
                //o.screenPos = ComputeScreenPos(o.vertex);
                //o.distFade.x = length(unity_ObjectToWorld._m03_m13_m23 - _WorldSpaceCameraPos.xyz);
                // DistanceFade_float(o.distFade.x, o.distFade.y);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float baseAlpha = tex2Dbias(_BaseMap,float4(i.uv,0,_MipBias)).a;
                //float4 screenPos = i.screenPos;
                //screenPos.xy /=  screenPos.w;
                //float ditherAlpha;
                //DitherCrossFade_float(screenPos,i.distFade.y,i.distFade.x,_Cutoff,ditherAlpha);
                clip(baseAlpha - _Cutoff);
                return float4(1,0,1,1);
            }
            ENDCG
        }
    }
    SubShader // URP 
    {
         Tags { "RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" "ShaderModel"="4.5"}
         Pass
        {
            Name "Unlit"
            cull off
            colormask 0
            
            CGPROGRAM

            #pragma multi_compile_instancing
            #pragma shader_feature_local _PW_SF_WIND_ON
            #pragma instancing_options procedural:setup assumeuniformscaling nolodfade 
            
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Assets/Procedural Worlds/Flora/Content Resources/Shaders/PW_FloraIncludes.hlsl"
            #include "Assets/Procedural Worlds/Flora/Content Resources/Shaders/PW_FloraWInd.cginc"
            

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                // float4 screenPos : TEXCOORD1;
                // float2 distFade: TEXCOORD2;
            };

            //General
            sampler2D _BaseMap, _NormalMap, _MaskMap;
            half4 _BaseColor, _ColorA, _ColorB, _BaseMap_ST;
            half _Cutoff;
            float4 _MaskMapMod;
            float _Thickness,_MipBias;

            //wind
            float2 _PW_WindTreeWidthHeight;
            float4 _PW_WindTreeFlex, _PW_WindTreeFrequency;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                WindCalculations_float(v.vertex.xyz,_PW_WindTreeWidthHeight.xy,_PW_WindTreeFlex,_PW_WindTreeFrequency,v.vertex.xyz);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv,_BaseMap);
                //o.screenPos = ComputeScreenPos(o.vertex);
                //o.distFade.x = length(unity_ObjectToWorld._m03_m13_m23 - _WorldSpaceCameraPos.xyz);
                // DistanceFade_float(o.distFade.x, o.distFade.y);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float baseAlpha = tex2Dbias(_BaseMap,float4(i.uv,0,_MipBias)).a;
                //float4 screenPos = i.screenPos;
                //screenPos.xy /=  screenPos.w;
                //float ditherAlpha;
                //DitherCrossFade_float(screenPos,i.distFade.y,i.distFade.x,_Cutoff,ditherAlpha);
                clip(baseAlpha - _Cutoff);
                return float4(1,0,1,1);
            }
            ENDCG
        }
    }
    SubShader //Builtin
    {
         Tags { "RenderType"="Opaque" }
         Pass
         {
            Name "Zprime"
            cull off
            colormask 0
            
            CGPROGRAM

            #pragma multi_compile_instancing
            #pragma shader_feature_local _PW_SF_WIND_ON
            #pragma instancing_options procedural:setup assumeuniformscaling nolodfade 
            
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Assets/Procedural Worlds/Flora/Content Resources/Shaders/PW_FloraIncludes.hlsl"
            #include "Assets/Procedural Worlds/Flora/Content Resources/Shaders/PW_FloraWInd.cginc"
            

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                // float4 screenPos : TEXCOORD1;
                // float2 distFade: TEXCOORD2;
            };

            //General
            sampler2D _BaseMap, _NormalMap, _MaskMap;
            half4 _BaseColor, _ColorA, _ColorB, _BaseMap_ST;
            half _Cutoff;
            float4 _MaskMapMod;
            float _Thickness,_MipBias;

            //wind
            float2 _PW_WindTreeWidthHeight;
            float4 _PW_WindTreeFlex, _PW_WindTreeFrequency;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                WindCalculations_float(v.vertex.xyz,_PW_WindTreeWidthHeight.xy,_PW_WindTreeFlex,_PW_WindTreeFrequency,v.vertex.xyz);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv,_BaseMap);
                //o.screenPos = ComputeScreenPos(o.vertex);
                //o.distFade.x = length(unity_ObjectToWorld._m03_m13_m23 - _WorldSpaceCameraPos.xyz);
                // DistanceFade_float(o.distFade.x, o.distFade.y);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float baseAlpha = tex2Dbias(_BaseMap,float4(i.uv,0,_MipBias)).a;
                //float4 screenPos = i.screenPos;
                //screenPos.xy /=  screenPos.w;
                //float ditherAlpha;
                //DitherCrossFade_float(screenPos,i.distFade.y,i.distFade.x,_Cutoff,ditherAlpha);
                clip(baseAlpha - _Cutoff);
                return float4(1,0,1,1);
            }
            ENDCG
        }
    }
}
