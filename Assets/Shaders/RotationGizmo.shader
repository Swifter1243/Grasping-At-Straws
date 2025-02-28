Shader "Unlit/RotationGizmo"
{
    Properties
    {
        _HandleRadius ("Handle Radius", Float) = 0.5
        _HandleArrowSize ("Handle Arrow Size", Float) = 0.1
        _HandleArrowDistance ("Handle Arrow Distance", Float) = 0.6
        _HandleWidth ("Handle Width", Float) = 0.1
        _HandleStart ("Handle Start", Float) = 0.9
        _AxisColor ("Axis Color", Color) = (1,0,0)
        [ToggleUI] _InUse ("In Use", Int) = 0
        _ClickPosition ("Click Position", Vector) = (1,1,0,0)
        _Opacity ("Opacity", Range(0,1)) = 1
        _Visibility ("Visibility", Range(0,1)) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent+1"
        }
        ZTest Always
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float3 localPos : TEXCOORD0;
                float3 targetPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            float _HandleRadius;
            float _HandleArrowDistance;
            float _HandleArrowSize;
            float _HandleWidth;
            float _HandleStart;
            float3 _AxisColor;
            bool _InUse;
            float2 _ClickPosition;
            float _Opacity;
            float _Visibility;

            v2f vert(appdata v)
            {
                v2f o;

                float3 localCamPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
                float3 targetPos = _InUse ? float3(_ClickPosition.x, 0, _ClickPosition.y) : localCamPos;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.localPos = -v.vertex * 0.2;
                o.targetPos = targetPos;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {

                float alignment = dot(normalize(-i.localPos), normalize(i.targetPos));
                float angle = acos(dot(normalize(-i.localPos.xz), normalize(i.targetPos.xz) * 0.99999));

                float saw = 1 - (angle - _HandleArrowDistance) / _HandleArrowSize;
                float spike = saw * (angle > _HandleArrowDistance && angle < _HandleArrowDistance + _HandleArrowSize);

                float viewAlignment = smoothstep(_HandleStart, 1, alignment);

                float handleDist = abs(_HandleRadius - length(i.localPos.xz));
                handleDist -= spike * _HandleArrowSize * 0.3;

                float handleCircle = 1 - step(_HandleWidth + _InUse * _HandleWidth, handleDist);
                float handle = handleCircle * viewAlignment;

                float4 regularCol = float4(_AxisColor, handle * 0.8 * _Opacity * _Visibility);

                float4 inUseCol = lerp(float4(_AxisColor * 0.01, 0.8 * handleCircle), float4(_AxisColor, handleCircle), handle);

                return _InUse ? inUseCol : regularCol;
            }
            ENDCG
        }
    }
}
