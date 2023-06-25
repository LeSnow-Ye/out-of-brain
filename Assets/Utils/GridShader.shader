Shader "Unlit/GridShader"
{
    Properties
    {
        _GridWidth ("Grid Width", float) = 1
        _GridHeight ("Grid Height", float) = 1
        _OffsetX ("Offset X", float) = 0
        _OffsetY ("Offset Y", float) = 0
        _LineWidth ("Line Width", float) = 0.05
        _AxisWidth ("Axis Width", float) = 0.05
        _LineColor ("Line Color", Color) = (1,1,1,1)
        _AxisColor ("Line Color", Color) = (1,1,1,1)
        _BackgroundColor ("Background Color", Color) = (1,1,1,1)
        
    }
    SubShader
    {
        Tags { "Queue" = "Background" "IgnoreProjector" = "True" "RenderType"="Transparent" "PreviewType" = "Plane"}

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
            };

            struct v2f
            {
                float2 posWS  : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            float _GridWidth;
            float _GridHeight;
            float _OffsetX;
            float _OffsetY;
            float _LineWidth; 
            float _AxisWidth; 
            float4 _LineColor;
            float4 _AxisColor;
            float4 _BackgroundColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.posWS = mul(unity_ObjectToWorld, v.vertex).xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 pos = i.posWS + _LineWidth / 2 - float2(_OffsetX, _OffsetY);
                float2 posAxis = i.posWS + _AxisWidth / 2 - float2(_OffsetX, _OffsetY);
                float x = _GridWidth * (pos.x / _GridWidth - floor(pos.x / _GridWidth));
                float y = _GridHeight * (pos.y / _GridHeight - floor(pos.y / _GridHeight));

                if ((posAxis.x >= 0 && pos.x <= _AxisWidth) || (posAxis.y >= 0 && pos.y <= _AxisWidth))
                    return _AxisColor;
                
                if (x <= _LineWidth || y <= _LineWidth)
                    return _LineColor;

                return _BackgroundColor;
            }
            ENDCG
        }
    }
}
