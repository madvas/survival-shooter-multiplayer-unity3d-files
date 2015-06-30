// Unlit alpha-blended shader with tint color.
// - no lighting
// - no lightmap support
Shader "Energy Bar Toolkit/Unlit/Transparent Tint Pre Depth Based" {
    Properties {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend One OneMinusSrcAlpha
        Lighting Off
        Fog { Mode Off }
        ZWrite On
        Cull Off
        ColorMaterial AmbientAndDiffuse

        Pass {
            SetTexture [_MainTex] {
            	combine texture * primary
            }
        }
    }
}
