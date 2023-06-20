Shader "Custom/PixelationShader"
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
                float pixelSize = lerp(1, 100, _DistortionAmount);

                // Get the aspect ratio of the screen
                float aspectRatio = _ScreenParams.y / _ScreenParams.x;

                // Adjust the blockSize to account for the aspect ratio
                float2 blockSize = pixelSize / float2(_ScreenParams.x * aspectRatio, _ScreenParams.y);
    
                // Offset the UV coordinates to move the origin to the center of the texture
                float2 uv = i.uv - 0.5;

                // Round the UV coordinates to the nearest whole number after scaling by blockSize
                uv = round(uv / blockSize) * blockSize;

                // Offset the UV coordinates back to the original range
                uv += 0.5;

                fixed4 pixelatedColor = tex2D(_MainTex, uv);
                return pixelatedColor;
            }

            ENDCG
        }
    }
}
