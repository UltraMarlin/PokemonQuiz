Shader "Custom/DoubleWaveShader"
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
                float sqrtDistortionAmount = sqrt(_DistortionAmount);
                float waveAmount = 70.0 * sqrtDistortionAmount;
                float2 uv = i.uv;
                uv.y += sin((uv.x - 0.5) * waveAmount) * sqrtDistortionAmount * 0.1;
                return tex2D(_MainTex, uv);
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
                float sqrtDistortionAmount = sqrt(_DistortionAmount);
                float waveAmount = sqrtDistortionAmount * 60.0;
                float2 uv = i.uv;
                uv.x += sin((uv.y - 0.5) * waveAmount) * sqrtDistortionAmount * 0.07;
                return tex2D(_MainTex, uv);
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
                float2 uv = i.uv;
                fixed4 color = tex2D(_MainTex, uv);

                if (_DistortionAmount > 0) {
                    float2 offset = 1.0 / _ScreenParams.xy;
                    color += tex2D(_MainTex, uv + float2(-offset.x, -offset.y));
                    color += tex2D(_MainTex, uv + float2(-offset.x, offset.y));
                    color += tex2D(_MainTex, uv + float2(offset.x, -offset.y));
                    color += tex2D(_MainTex, uv + float2(offset.x, offset.y));
                    return color / 5.0;
                } else {
                    return color;
                }
            }
            ENDCG
        }
    }
}
