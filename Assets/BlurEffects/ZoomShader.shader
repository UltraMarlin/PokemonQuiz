Shader "Custom/ZoomShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DistortionAmount ("Distortion Amount", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _DistortionAmount;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Calculate the zoom based on the _DistortionAmount with quadratic deceleration
                float adjustedDistortion = 1.0 - pow(1.0 - _DistortionAmount, 2.3); // This will make it start slow and accelerate as _DistortionAmount decreases
                float zoomFactor = lerp(1.0, 0.05, adjustedDistortion);
                float2 centeredUV = i.uv - 0.5; // Translate UVs to [-0.5, 0.5] range
                centeredUV *= zoomFactor; // Zoom in/out based on the zoomFactor
                centeredUV += 0.5; // Translate UVs back to [0, 1] range

                return tex2D(_MainTex, centeredUV);
            }

            ENDCG
        }
    }
}
