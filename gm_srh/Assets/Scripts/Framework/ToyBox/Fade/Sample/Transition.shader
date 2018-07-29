Shader "Unlit/rtransition"
{
    Properties
    {
        [NoScaleOffset]_MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _Scale ("Scale", Range(0, 1)) = 0
    }
 
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
 
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Fog{ Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha
 
        Pass
        {
            CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
 
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                half2 texcoord  : TEXCOORD0;
            };
 
            fixed4 _Color;
            uniform fixed _Scale;
            sampler2D _MainTex;
            
 
            // 頂点シェーダーの基本
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                float scale = _Scale % 1.0;
                // 中心UVを設定
                float2 pivot_uv = float2(0.5, 0.5); // ←UV方向間違ってる？
                float2 r = (IN.texcoord.xy - pivot_uv) * (1 / scale);
                OUT.texcoord = r + pivot_uv;
                

                return OUT;
            }
 
            // 通常のフラグメントシェーダー
            fixed4 frag(v2f IN) : SV_Target
            {
                
                half alpha = tex2D(_MainTex, IN.texcoord).a;
                //Alpha反転
                alpha = saturate((-alpha + 1));
                
                if(_Scale >= 1){
                    return fixed4(_Color.r, _Color.g, _Color.b, 0);
                }
                return fixed4(_Color.r, _Color.g, _Color.b, alpha);
                
            }
            ENDCG
        }
    }
 
    FallBack "UI/Default"
}
