Shader "Custom/Steepness" {
    Properties {
        _MainTex ("Main Texture", 2D) = "white" {}
        _SteepTex ("Steepness Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
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

        fixed4 _Color;
        fixed _Shininess;

        sampler2D _MainTex;
        sampler2D _SteepTex;
        fixed _SteepnessThreshold;
        fixed _SteepnessBlend;
        
        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };
        
        void surf(Input IN, inout SurfaceOutput o) {
            // Sample textures
            fixed4 c1 = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 c2 = tex2D(_SteepTex, IN.uv_MainTex);

            float3 worldNormal = WorldNormalVector (IN, o.Normal);
            float steepness = 1-abs(worldNormal.y);  // Assuming steepness is based on the absolute y-component of the normal

            // Blend textures based on steepness threshold
            float blendFactor = smoothstep(_SteepnessThreshold - _SteepnessBlend, _SteepnessThreshold + _SteepnessBlend, steepness);
            fixed4 result = lerp(c1, c2, blendFactor);

            o.Albedo = result.rgb * _Color.rgb;
            o.Alpha = result.a * _Color.a;
            
            o.Specular = _Shininess;
            o.Gloss = result.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
