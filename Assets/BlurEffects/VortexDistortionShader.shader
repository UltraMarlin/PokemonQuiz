Shader "Custom/VortexDistortionShader"
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
                // Offset the UV coordinates to move the origin to the center of the texture
                float2 uv = i.uv - 0.5;

                float waveFactor = sin(uv.y * 10.0 * uv.x * 5.0 * _DistortionAmount);
                uv += float2(waveFactor, waveFactor) * _DistortionAmount;
                
                // Offset the UV coordinates back to the original range
                uv += 0.5;

                fixed4 distortedColor = tex2D(_MainTex, uv);
                return distortedColor;
            }
            ENDCG
        }
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
                float2 center = 0.5;
                float2 offset = i.uv - center;
                float distance = length(offset);
                float angle = atan2(offset.y, offset.x) + _DistortionAmount * distance * 30.0;
                float amplitude = _DistortionAmount * sqrt(distance) * 0.5;
                float2 distortedUV = i.uv + float2(sin(angle), cos(angle)) * amplitude;
                
                fixed4 distortedColor = tex2D(_MainTex, distortedUV);
                return distortedColor;
            }
            ENDCG
        }
    }
}
