Shader "Animation/StandInstanceing" 
{
 Properties {
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		[NoScaleOffset] _BumpMap ("Normalmap", 2D) = "bump" {}
}
SubShader { 
	Tags { "RenderType"="Opaque" }
	LOD 250
	
CGPROGRAM

#include "AnimationInstancingBase.cginc"
#pragma vertex vert 
#pragma multi_compile_instancing
//DECLARE_VERTEX_SKINNING

#pragma surface surf Standard exclude_path:prepass nolightmap noforwardadd halfasview interpolateview addshadow

sampler2D _MainTex;
sampler2D _BumpMap;
half _Glossiness;
half _Metallic;
fixed4 _Color;

struct Input {
	float2 uv_MainTex;
};



void surf (Input IN, inout SurfaceOutputStandard o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = c.rgb * _Color.rgb;
	o.Metallic = _Metallic;
	o.Smoothness = _Glossiness;
	o.Alpha = c.a;
	o.Normal = UnpackNormal (tex2D(_BumpMap, IN.uv_MainTex));
}
ENDCG
}
//FallBack "Mobile/VertexLit"

}
