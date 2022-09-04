Shader "PWS/Details/PW_Details_LitBasic_BuiltIn"
{
    Properties
    {
        [Header(PBR)]
        [HDR]_BaseColor ("Base Color", Color) = (1,1,1,1)
        _BaseMap ("Base Map (RGB)", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "Normal" {}
        _MaskMap ("PBR Mask Map", 2D) = "white" {}
        _MaskMapMod("Mask Map modifiers", Vector) = (1,1,1,1)
        _Thickness("Thickness", Range(0, 1)) = 0
        [Space]
        
        [Header(Alpha)]
        _Cutoff ("Alpha Cut Off", Range(0,1)) = 0.05
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"  "Queue" = "Geometry"}
        LOD 200
        Cull Back
        
        CGPROGRAM
        
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows addshadow 

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        #include "PW_FloraIncludes.hlsl"
        
        
        #pragma multi_compile_instancing
        #pragma instancing_options procedural:setup

        struct Input
        {
            float2 uv_BaseMap;
            float facing: VFACE;
            float4 screenPos;
            float3 worldPos;
            float3 worldNormal; 
            INTERNAL_DATA
        };

        sampler2D _BaseMap, _NormalMap, _MaskMap;
        half4 _BaseColor, _ColorA, _ColorB;
        half _Cutoff;
        float4 _MaskMapMod;
        
        // Fragment "shader" 
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo & Alpha test
            fixed4 base = tex2D(_BaseMap, IN.uv_BaseMap);
            clip(base.a - _Cutoff);
            
            // Dither Fade
            float2 vpos = IN.screenPos.xy / IN.screenPos.w * _ScreenParams.xy;
            float fade;
            float dist = length(float3(unity_ObjectToWorld._m03,unity_ObjectToWorld._m13,unity_ObjectToWorld._m23) - _WorldSpaceCameraPos);
            DistanceFade_float(dist,fade);
            DitherCrossFade(vpos,fade);

            // PBR masks
            ColorVariationMix_float(_ColorA,_ColorB,base.rgb,base.rgb);
            base *= _BaseColor;

            // Flip Normal for double sided rendering
            half3 normal = UnpackNormal(tex2D(_NormalMap, IN.uv_BaseMap));
            float4 mask = saturate(tex2D (_MaskMap, IN.uv_BaseMap) * _MaskMapMod);

            // Output surface struct
            o.Albedo = base.rgb;
            o.Normal = normal;
            o.Metallic = mask.r;
            o.Occlusion = mask.g;
            o.Smoothness = mask.a;
            o.Alpha = base.a;
            
        }
        ENDCG
    }
    FallBack "Diffuse"
}
