Shader "Custom/BlurShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DistortionAmount ("Distortion Amount", Range(0, 1)) = 1
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float _DistortionAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Initialize final color as black
                fixed4 col = fixed4(0, 0, 0, 1);
                // Radius of sample points
                float sqrtDistortion = sqrt(_DistortionAmount) * sqrt(_DistortionAmount);
                float radius = sqrtDistortion * 0.10;
                // Number of sample points
                int samples = 35;

                // Loop over sample points
                for (int s = 0; s < samples; s++)
                {
                    // Radial offset within radius
                    float2 offset = float2(cos(2.0 * 3.14159 * s / samples), sin(2.0 * 3.14159 * s / samples)) * radius * sqrtDistortion;
                    // Sample texture
                    fixed4 sample = tex2D(_MainTex, i.uv + offset);
                    // Accumulate sample color
                    col += sample;
                }

                // Average color
                col /= samples;

                return col;
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float _DistortionAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Initialize final color as black
                fixed4 col = fixed4(0, 0, 0, 1);
                // Radius of sample points
                float sqrtDistortion = sqrt(_DistortionAmount) * sqrt(_DistortionAmount);
                float radius = sqrtDistortion * 0.05;
                // Number of sample points
                int samples = 20;

                // Loop over sample points
                for (int s = 0; s < samples; s++)
                {
                    // Radial offset within radius
                    float2 offset = float2(cos(2.0 * 3.14159 * (s + samples) / (2.0 * samples)), sin(2.0 * 3.14159 * (s + samples) / (2.0 * samples))) * radius * sqrtDistortion;
                    // Sample texture
                    fixed4 sample = tex2D(_MainTex, i.uv + offset);
                    // Accumulate sample color
                    col += sample;
                }

                // Average color
                col /= samples;

                // Combine with previous pass
                fixed4 prevCol = tex2D(_MainTex, i.uv);
                col = (col + prevCol) / 2.0;

                return col;
            }
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float _DistortionAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Initialize final color as black
                fixed4 col = fixed4(0, 0, 0, 1);
                // Radius of sample points
                float sqrtDistortion = sqrt(_DistortionAmount);
                float radius = sqrtDistortion * 0.05;
                // Number of sample points
                int samples = 25;

                // Loop over sample points
                for (int s = 0; s < samples; s++)
                {
                    // Radial offset within radius
                    float2 offset = float2(cos(2.0 * 3.14159 * (s + samples) / (2.0 * samples)), sin(2.0 * 3.14159 * (s + samples) / (2.0 * samples))) * radius * sqrtDistortion;
                    // Sample texture
                    fixed4 sample = tex2D(_MainTex, i.uv + offset);
                    // Accumulate sample color
                    col += sample;
                }

                // Average color
                col /= samples;

                // Combine with previous pass
                fixed4 prevCol = tex2D(_MainTex, i.uv);
                col = (col + prevCol) / 2.0;

                return col;
            }
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float _DistortionAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Initialize final color as black
                fixed4 col = fixed4(0, 0, 0, 1);
                // Radius of sample points
                float sqrtDistortion = sqrt(_DistortionAmount);
                float radius = sqrtDistortion * 0.03;
                // Number of sample points
                int samples = 20;

                // Loop over sample points
                for (int s = 0; s < samples; s++)
                {
                    // Radial offset within radius
                    float2 offset = float2(cos(2.0 * 3.14159 * (s + samples) / (2.0 * samples)), sin(2.0 * 3.14159 * (s + samples) / (2.0 * samples))) * radius * sqrtDistortion;
                    // Sample texture
                    fixed4 sample = tex2D(_MainTex, i.uv + offset);
                    // Accumulate sample color
                    col += sample;
                }

                // Average color
                col /= samples;

                // Combine with previous pass
                fixed4 prevCol = tex2D(_MainTex, i.uv);
                col = (col + prevCol) / 2.0;

                return col;
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float _DistortionAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Initialize final color as black
                fixed4 col = fixed4(0, 0, 0, 1);
                // Radius of sample points
                float sqrtDistortion = sqrt(_DistortionAmount);
                float radius = sqrtDistortion * 0.04;
                // Number of sample points
                int samples = 35;

                // Loop over sample points
                for (int s = 0; s < samples; s++)
                {
                    // Radial offset within radius
                    float2 offset = float2(cos(2.0 * 3.14159 * (s + samples) / (2.0 * samples)), sin(2.0 * 3.14159 * (s + samples) / (2.0 * samples))) * radius * sqrtDistortion;
                    // Sample texture
                    fixed4 sample = tex2D(_MainTex, i.uv + offset);
                    // Accumulate sample color
                    col += sample;
                }

                // Average color
                col /= samples;

                // Combine with previous pass
                fixed4 prevCol = tex2D(_MainTex, i.uv);
                col = (col + prevCol) / 2.0;

                return col;
            }
            ENDCG
        }
    }
}
