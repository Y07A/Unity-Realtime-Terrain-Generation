Shader "Custom/SteepnessColor" {
    Properties {
        _MainColor ("Main Color", Color) = (1,1,1,1) // New property for main color
        _SteepColor ("Steepness Color", Color) = (1,1,1,1) // New property for steepness color
        _Shininess ("Shininess", Range (0.03, 1)) = 0.078125
        _SteepnessThreshold ("Steepness Threshold", Range(0,1)) = 0.5
        _SteepnessBlend ("Steepness Blend", Range(0,1)) = 0.1
    }
    SubShader {
        Tags {"Queue"="Geometry" "RenderType"="Opaque"}
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert
        #pragma multi_compile _ LOD_FADE_CROSSFADE

        fixed4 _MainColor; // Color to replace _MainTex
        fixed4 _SteepColor; // Color to replace _SteepTex

        fixed _Shininess;
        fixed _SteepnessThreshold;
        fixed _SteepnessBlend;
        
        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };
        
        void surf(Input IN, inout SurfaceOutput o) {
            // Sample colors directly
            fixed4 c1 = _MainColor;
            fixed4 c2 = _SteepColor;

            float3 worldNormal = WorldNormalVector (IN, o.Normal);
            float steepness = 1 - abs(worldNormal.y);  // Assuming steepness is based on the absolute y-component of the normal

            // Blend colors based on steepness threshold
            float blendFactor = smoothstep(_SteepnessThreshold - _SteepnessBlend, _SteepnessThreshold + _SteepnessBlend, steepness);
            fixed4 result = lerp(c1, c2, blendFactor);

            o.Albedo = result.rgb;
            o.Alpha = result.a;
            
            o.Specular = _Shininess;
            o.Gloss = result.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
