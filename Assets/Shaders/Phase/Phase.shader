Shader "Custom/Phase" {
  Properties{
    _Color("Color", Color) = (1,1,1,.5)
    _MainTex("Color (RGB) Alpha (A)", Color) = (1,1,1,.5)
    _Glossiness("Smoothness", Range(0,1)) = 0.7
    _Metallic("Metallic", Range(0,1)) = 0.7

    _Speed("Speed",Range(0.0,10)) = 0
    _Amount("Amount", Range(0.0,200)) = 0
    _XDistance("XDistance", Range(0, 2)) = 0.0
    _YDistance("YDistance", Range(0, 2)) = 0.0
    _ZDistance("ZDistance", Range(0, 2)) = 0.0
  }
    SubShader{
      Tags { "RenderType" = "Transparent" }
      LOD 200

      CGPROGRAM

      // add "addshadow" to let unity know you're displacing verts
      // this will ensure their ShadowCaster + ShadowCollector passes use the vert function and have the correct positions
      #pragma surface surf Standard fullforwardshadows vertex:vert addshadow

      #pragma target 3.0

      sampler2D _MainTex;

      struct Input {
        float2 uv_MainTex;
      };

      half _Glossiness;
      half _Metallic;
      fixed4 _Color;

      half _Speed;
      half _Amount;
      half _XDistance;
      half _YDistance;
      half _ZDistance;

      void vert(inout appdata_full v)
      {
        v.vertex.x += sin(_Time.y * _Speed + v.vertex.y * _Amount) * _XDistance;
        v.vertex.y += sin(_Time.y * _Speed + v.vertex.x * _Amount) * _YDistance;
        v.vertex.z += sin(_Time.y * _Speed + v.vertex.y * _Amount) * _ZDistance;
      }

      void surf(Input IN, inout SurfaceOutputStandard o) {
        // Albedo comes from a texture tinted by color
        fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
        o.Albedo = c.rgba;
        // Metallic and smoothness come from slider variables
        o.Metallic = _Metallic;
        o.Smoothness = _Glossiness;
        o.Alpha = c.a;
      }
      ENDCG
    }
      FallBack "Diffuse"
}
