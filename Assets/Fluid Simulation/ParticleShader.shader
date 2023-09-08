Shader "Unlit/FluidSimulationParticleShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent+300" "IgnoreProjector" = "True" "RenderType"="Transparent" "PreviewType" = "Plane"}

        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

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
                uint id : SV_INSTANCEID;
            };

            StructuredBuffer<float2> Particles;

            float4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v, uint instanceID : SV_INSTANCEID)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex + float4(Particles[instanceID].xy, 0, 0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.id = instanceID;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col * _Color;
            }
            ENDCG
        }
    }
}
