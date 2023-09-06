Shader "Unlit/RippleShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _N ("RI", float) = 1.33
        _WaveSpeed ("Wave Speed", float) = 1
        _WaveLength ("Wave Length", float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "PreviewType" = "Plane" }
        
        Cull Off
        ZWrite Off
        
        LOD 100

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
                float4 scrPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _N;
            float _WaveSpeed;
            float _WaveLength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.scrPos = ComputeScreenPos(o.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                i.uv.x += _CosTime * 0.1;

                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
