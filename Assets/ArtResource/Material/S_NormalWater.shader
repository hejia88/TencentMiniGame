Shader "Bump/001_normal"
{
Properties{
	_MainColor("MainColor",Color)=(1,1,1,1)
	_TopColor("TopColor", Color) = (1,1,0,1)
	_FresnelPower("FresnelPower", Float) = 1.0
	_SpecularColor("SpecularColor",Color)=(1,1,1,1)
	_MainTex("MainTex",2D)="white"{}
	_BumpTex("BumpTex",2D)="bump"{}
	_BumpScale("BumpScale",Float)=1.0
	_Gloss("Gloss",Range(8.0,256))=20
	_TimeSpeed("TimeSpeed", Float) = 1.0
	}
SubShader{
	Pass{
		Tags{"RenderType"="Opaque" "LightMode"="ForwardBase"}
		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#include "Lighting.cginc"
		#define PI 3.14159265359

		fixed4 _MainColor;
		fixed4 _TopColor;
		float _FresnelPower;
		fixed4 _SpecularColor;
		sampler2D _MainTex;
		float4 _MainTex_ST;
		sampler2D _BumpTex;
		float4 _BumpTex_ST;
		float _BumpScale;
		float _Gloss;
		float _TimeSpeed;

		struct a2v{
			float4 vertex:POSITION;
			float4 texcoord:TEXCOORD0;
			float3 normal:NORMAL;
			float4 tangent:TANGENT;
		};

		struct v2f{
			float4 pos:SV_POSITION;
			float4 uv:TEXCOORD0;
			float3 lightDir:TEXCOORD1;
			float3 viewDir:TEXCOORD2;
			float fresnelValue:TEXCOORD3;
		};

		v2f vert(a2v v){
			v2f o;
			o.pos=UnityObjectToClipPos(v.vertex);
			//主纹理与法线纹理通常使用同一组纹理坐标
			o.uv.xy=v.texcoord.xy*_MainTex_ST.xy+_MainTex_ST.zw;
			o.uv.zw=v.texcoord.xy*_BumpTex_ST.xy+_BumpTex_ST.zw;
			// o.uv.zw *= 0.5;
			// o.uv.zw.x -= _Time.y * _TimeSpeed;
			//内置宏，取得切线空间旋转矩阵
			TANGENT_SPACE_ROTATION;
			o.lightDir=mul(rotation,ObjSpaceLightDir(v.vertex).xyz);
			o.viewDir=mul(rotation,ObjSpaceViewDir(v.vertex).xyz);

			float3 viewDir = ObjSpaceViewDir ( v.vertex );
			o.fresnelValue = pow(1 - saturate(dot(v.normal, viewDir)), _FresnelPower);

			return o;
		}

		fixed4 frag(v2f i):SV_Target{
			fixed3 tangentLightDir=normalize(i.lightDir);
			fixed3 tangentViewDir=normalize(i.viewDir);
// _Time.y * _TimeSpeed
			fixed2 uv = i.uv.zw;
			float smallTiling = 1;
			float bigTiling = 0.25;
			fixed3 tangentNormal;

			uv.x -= _Time.y * _TimeSpeed;
			fixed3 tangentNormalSmall =UnpackNormal(tex2D(_BumpTex,uv * smallTiling));
			tangentNormalSmall.xy*=_BumpScale;
			tangentNormalSmall.z=sqrt(1.0-saturate(dot(tangentNormalSmall.xy,tangentNormalSmall.xy)));

			uv = i.uv.zw;
			uv.x += _Time.y * _TimeSpeed;
			fixed3 tangentNormalBig =UnpackNormal(tex2D(_BumpTex,uv * bigTiling));
			tangentNormalBig.xy*=_BumpScale;
			tangentNormalBig.z=sqrt(1.0-saturate(dot(tangentNormalBig.xy,tangentNormalBig.xy)));

			tangentNormal = BlendNormals(tangentNormalSmall, tangentNormalBig);

			// fixed3 albedo=_MainColor.rgb*tex2D(_MainTex,i.uv.xy);
			fixed3 albedo= lerp(_MainColor.rgb, _TopColor.rgb, i.fresnelValue);
			fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.xyz*albedo;

			//改进版 BRDF 函数
			fixed3 diffuse=_LightColor0.rgb*albedo*max(0,saturate(dot(tangentNormal,tangentLightDir)))/PI;
			fixed3 halfDir=normalize(tangentLightDir+tangentViewDir);
			fixed3 specular=_LightColor0.rgb*_SpecularColor.rgb*pow(max(0,dot(tangentNormal,halfDir)),_Gloss)*max(0,saturate(dot(tangentNormal,tangentLightDir)))*(_Gloss+8)/(8*PI);

			return fixed4(ambient+diffuse+specular,1.0);
		}


	ENDCG
	}
  }
}