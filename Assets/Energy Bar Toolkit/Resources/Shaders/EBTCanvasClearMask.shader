// Shader by Tim C
// http://forum.unity3d.com/threads/ugui-masking-blocked-by-background-meshes.270122/
Shader "Energy Bar Toolkit/Canvas Mask Clear"
{
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
     
        Stencil
        {
            Ref 0
            Comp Always
            Pass Zero
            ReadMask 255
            WriteMask 255
        }
 
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Fog { Mode Off }
        Blend Zero One
        ColorMask 0
 
        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
         
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
            };
         
            fixed4 _Color;
 
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
                OUT.color = IN.color;
                return OUT;
            }
 
            fixed4 frag(v2f IN) : SV_Target
            {
                return IN.color;
            }
        ENDCG
        }
    }
}