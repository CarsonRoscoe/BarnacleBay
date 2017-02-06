Shader "Custom/Lava"
{
  Properties
  {
    _TintColor("Tint Color", Color) = (0.500000,0.500000,0.500000,0.500000)
    _Radius("Radius", Range(-0.2, 1)) = 1
    _Spread("Spread", Range(0, 0.2)) = 0.2
    _MainTex("Diffuse Texture", 2D) = "white" {}
  }

    SubShader
  {
    Tags{ "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" "PreviewType" = "Plane" }

    Pass
  {
    Blend One One

    CGPROGRAM
#pragma vertex vert
#pragma fragment frag          
#include "UnityCG.cginc"

    uniform fixed4 _TintColor;

  uniform sampler2D _MainTex;
  uniform half4 _MainTex_ST;
  uniform fixed _Radius;
  uniform fixed _Spread;

  struct appdata_t
  {
    half4 vertex : POSITION;
    fixed2 uv : TEXCOORD0;
  };

  struct v2f
  {
    half4 pos : SV_POSITION;
    fixed2 uv : TEXCOORD0;
    fixed op : TEXCOORD1;
  };

  v2f o;

  v2f vert(appdata_t v)
  {
    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
    o.uv = v.uv;

    fixed dx = v.uv.x - 0.5;
    fixed dy = v.uv.y - 0.5;
    fixed curR = dx * dx + dy * dy;

    fixed maxR = (_Radius + _Spread);
    fixed minR = (_Radius - _Spread);

    fixed deltaR = maxR - minR;
    fixed opacity = 0;
    if (curR < minR)
    {
      opacity = 0;
    }
    else if (curR > maxR)
    {
      opacity = 1;
    }
    else
    {
      opacity = curR / deltaR - minR / deltaR;
    }

    o.op = opacity;
    return o;
  }

  fixed4 frag(v2f IN) : COLOR
  {
    if (IN.op <= 0)
    {
      discard;
    }
  return 2.0 * _TintColor * tex2D(_MainTex, IN.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw) * IN.op;
  }
    ENDCG
  }
  }
}