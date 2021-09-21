// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ChinesePainting/MyMountain_Texture" 
{
	Properties 
	{
		[Header(OutLine)]
		// Stroke Color
		_StrokeColor ("Stroke Color", Color) = (0,0,0,1)
		// Noise Map
		_OutlineNoise ("Outline Noise Map", 2D) = "white" {}
		_Outline ("Outline", Range(0, 1)) = 0.1
		[Toggle] _FixOutlineWidth("Fix Outline Width?", Int) = 0
		// Outside Noise Width
		_OutsideNoiseWidth ("Outside Noise Width", Range(1, 2)) = 1.3
		_OutsideStrokeDryness ("Outside Stroke Dryness", Range(0.3, 0.6)) = 0.5
		_MaxOutlineZOffset ("Max Outline Z Offset", Range(-10,10)) = 0.5

		[Header(Interior)]
		_MainTex("Main", 2D) = "white" {}
		_ColorIntensity("Color Intensity",Range(0,1))=0
		_ColorBias("Color Bias", Range(-1, 1)) = 0
		_Ramp ("Ramp Texture", 2D) = "white" {}
		[NoScaleOffset] _BumpTex("Normal", 2D) = "bump" {}
		_BumpScale ("Bump Scale", Range(0, 10)) = 1.0
		// _CurvatureMap ("Curvature Ramp", 2D) = "white" {}
		_CurvatureColor ("Curvature Color", Color) = (0.3,0.3,0.3,1)
		_CurveFactor ("Curvature Factor", Range(0,40))=0.3
		// _Cuvthreshold ("Curvature Threshold", Range(0,1))=1

		// Interior Noise Level
		[Space(10)]
		[Toggle] _TriplanarOn ("Triplanar On?", Int) = 1
		_InteriorNoiseLevel ("Interior Noise Level", Range(0, 1)) = 0.15
		_InteriorNoise ("Interior Noise Map", 2D) = "white" {}
		_BrushOffsetXNoise("Brush Offset X Noise",Range(0,1))=0
		_BrushOffsetYNoise("Brush Offset Y Noise",Range(0,1))=0
		_BrushOffsetZNoise("Brush Offset Z Noise",Range(0,1))=0
		_TextureScaleNoise("Noise Texture Scale", float) = 1
        _TriplanarBlendSharpnessNoise("Noise Triplanar Blend Sharpness", float) = 1
		
		// Stroke Map
		_StrokeTex ("Brush Noise Tex", 2D) = "white" {}
		_StrokePow("Brush Power", Range(0, 5))=0.5
		_BrushOffsetXStroke("Brush Offset X Stroke",Range(0,1))=0
		_BrushOffsetYStroke("Brush Offset Y Stroke",Range(0,1))=0
		_BrushOffsetZStroke("Brush Offset Z Stroke",Range(0,1))=0
		_TextureScaleStroke("Brush Texture Scale", float) = 1
        _TriplanarBlendSharpnessStroke("Brush Triplanar Blend Sharpness", float) = 1
		[Enum(Opacity,1,Darken,2,Lighten,3,Multiply,4,Screen,5,Overlay,6,SoftLight,7)]
		_BlendType("Blend Type", Int) = 1

		// Guassian Blur
		radius ("Guassian Blur Radius", Range(0,60)) = 30
        resolution ("Resolution", float) = 800
        hstep("HorizontalStep", Range(0,1)) = 0.5
        // vstep("VerticalStep", Range(0,1)) = 0.5

		_Thred("Edge Thred" , Range(0.01,1)) = 0.25
		_Range("Edge Range" , Range(0,10)) = 1		
		[PowerSlider(3.0)]_Pow("Edge Intensity Pow",Range(0,10))=1
		_SilhouetteRampTex ("Silhouette Ramp Texture", 2D) = "black" {}

	}
    SubShader 
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry"}
		// the interior pass
		Pass 
		{
			NAME "INTERIOR"
			Tags { "LightMode"="ForwardBase" }
		
			Cull Back
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityShaderVariables.cginc"
			#include "MyTriplanar.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BumpTex;
			fixed _BumpScale;
			fixed _ColorIntensity;
			// float4 _StrokeColor;
			sampler2D _Ramp;
			// sampler2D _CurvatureMap;
			float4 _CurvatureColor;
			fixed _CurveFactor;
			// fixed _Cuvthreshold;
			sampler2D _StrokeTex;
			fixed _StrokePow;
			float4 _StrokeTex_ST;
			fixed _ColorBias;
			int _BlendType;
			float radius;
            float resolution;
            //the direction of our blur
            //hstep (1.0, 0.0) -> x-axis blur
            //vstep(0.0, 1.0) -> y-axis blur
            //for example horizontaly blur equal:
            //float hstep = 1;
            //float vstep = 0;
            float hstep;
            // float vstep;
			float _InteriorNoiseLevel;
			int _TriplanarOn;
			sampler2D _InteriorNoise;
			float4 _InteriorNoise_ST;
			fixed _BrushOffsetXNoise, _BrushOffsetYNoise, _BrushOffsetZNoise;
			float _TextureScaleNoise, _TriplanarBlendSharpnessNoise;
			fixed _BrushOffsetXStroke, _BrushOffsetYStroke, _BrushOffsetZStroke;
			float _TextureScaleStroke, _TriplanarBlendSharpnessStroke;

			float _Thred;
            float _Range;
            float _Pow;
			sampler2D _SilhouetteRampTex;
			
			struct a2v 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 tangent : TANGENT;
			}; 
		
			struct v2f 
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				// float3 worldPos : TEXCOORD2;
				float4 uv2 : TEXCOORD3;
				SHADOW_COORDS(4)
				UNITY_FOG_COORDS(5)
				float4 TtoW0 : TEXCOORD6;  
				float4 TtoW1 : TEXCOORD7;  
				float4 TtoW2 : TEXCOORD8;
			};
            

			v2f vert (a2v v) 
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex);
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				o.uv2.xy = TRANSFORM_TEX (v.texcoord, _InteriorNoise);
				o.uv2.zw = TRANSFORM_TEX(v.texcoord, _StrokeTex);
				o.worldNormal  = UnityObjectToWorldNormal(v.normal);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);  
				fixed3 worldBinormal = cross(o.worldNormal, worldTangent) * v.tangent.w;
				o.TtoW0 = float4(worldTangent.x, worldBinormal.x, o.worldNormal.x, worldPos.x);
				o.TtoW1 = float4(worldTangent.y, worldBinormal.y, o.worldNormal.y, worldPos.y);
				o.TtoW2 = float4(worldTangent.z, worldBinormal.z, o.worldNormal.z, worldPos.z);

				TRANSFER_SHADOW(o);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			inline fixed GetLuminance (fixed4 inColor)
			{
				return dot (inColor.rgb , fixed3(0.299,0.587,0.114));
			}
			
			float4 frag(v2f i) : SV_Target 
			{
				fixed4 mainTex = tex2D(_MainTex, i.uv.xy);
				fixed luminanceColor = GetLuminance(mainTex);
				fixed3 texGrey = (luminanceColor, luminanceColor, luminanceColor);
				texGrey = lerp(texGrey, mainTex.rgb, _ColorIntensity);
				float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
				fixed3 bump = UnpackNormal(tex2D(_BumpTex, i.uv));
				bump.xy *= _BumpScale;
				bump.z = sqrt(1.0 - saturate(dot(bump.xy, bump.xy)));
				// Transform the normal from tangent space to world space
				bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));
				fixed3 worldNormal = bump;
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				// fixed3 viewbump = mul((float3x3)UNITY_MATRIX_IT_MV, worldNormal);
				// fixed4 curvature= length(fwidth(float4(viewbump,1)))*0.5+0.5;
				fixed4 curvature = length(fwidth(worldNormal)) / (length(fwidth(worldPos)) * _CurveFactor);
				// return curvature;
				curvature = clamp(1-curvature, 0, 1);
				curvature = lerp(dot(_CurvatureColor, fixed3(0.299,0.587,0.114)), 1, curvature);
				// return curvature;
				// curvature = curvature > _Cuvthreshold ? tanh((curvature + 1) / 2) : 1;
				// curvature = lerp(1, tanh((curvature) / 2), smoothstep(-0.02, 0.02, curvature - _Cuvthreshold));
				// curvature = tex2D(_CurvatureMap, fixed2(curvature.x, 0.5));
                // return 1-curvature;
				// Perlin Noise
				// For the bias of the coordiante
				// float4 burn = tex2D(_InteriorNoise, i.uv2.xy);
				fixed4 burn = WorldTriplanar(worldPos, worldNormal, _InteriorNoise, _BrushOffsetXNoise, _BrushOffsetYNoise, _BrushOffsetZNoise, _TextureScaleNoise, _TriplanarBlendSharpnessNoise) * _TriplanarOn + tex2D(_InteriorNoise, i.uv2.xy) * (1 - _TriplanarOn);
				// return burn;
				// a little bit disturbance on normal vector
				fixed diff =  dot(worldNormal, worldLightDir);
				// fixed curvature = dot(float3(0.2, 0.7, 0.02), fixed3(diff, diff, diff));
				diff = (diff * 0.5 + 0.5);
				fixed4 k = WorldTriplanar(worldPos, worldNormal, _StrokeTex, _BrushOffsetXStroke, _BrushOffsetYStroke, _BrushOffsetZStroke, _TextureScaleStroke, _TriplanarBlendSharpnessStroke) * _TriplanarOn + tex2D(_StrokeTex, i.uv2.zw) * (1 - _TriplanarOn);
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				fixed vdotn = (dot(viewDir, worldNormal));
				fixed edge = vdotn / _Range;
                // edge = edge > _Thred ? 1 : edge;
                edge = pow(edge, _Pow);
				// return fixed4(edge, edge, edge, 1);
				fixed4 edgeColor = tex2D(_SilhouetteRampTex, fixed2(edge, 0.5));
				k = pow(k, _StrokePow);
				k = lerp(k, 1, saturate(edgeColor.r));
				float2 cuv = float2(diff, diff) + (burn.xy -0.5) * _InteriorNoiseLevel + _ColorBias;

				// This iniminate the bias of the uv movement?
				if (cuv.x > 0.95)
				{
					cuv.x = 0.95;
					cuv.y = 1;
				}
				if (cuv.y >  0.95)
				{
					cuv.x = 0.95;
					cuv.y = 1;
				}
				cuv = clamp(cuv, 0, 1);

				//Guassian Blur
				float4 sum = float4(0.0, 0.0, 0.0, 0.0);
                float2 tc = cuv;
                //blur radius in pixels
                float blur = radius/resolution/4;     
                sum += tex2D(_Ramp, float2(tc.x - 4.0*blur*hstep, 0.25)) * 0.0162162162;
                sum += tex2D(_Ramp, float2(tc.x - 3.0*blur*hstep, 0.25)) * 0.0540540541;
                sum += tex2D(_Ramp, float2(tc.x - 2.0*blur*hstep, 0.25)) * 0.1216216216;
                sum += tex2D(_Ramp, float2(tc.x - 1.0*blur*hstep, 0.25)) * 0.1945945946;
                sum += tex2D(_Ramp, float2(tc.x, 0.25)) * 0.2270270270;
                sum += tex2D(_Ramp, float2(tc.x + 1.0*blur*hstep, 0.25)) * 0.1945945946;
                sum += tex2D(_Ramp, float2(tc.x + 2.0*blur*hstep, 0.25)) * 0.1216216216;
                sum += tex2D(_Ramp, float2(tc.x + 3.0*blur*hstep, 0.25)) * 0.0540540541;
                sum += tex2D(_Ramp, float2(tc.x + 4.0*blur*hstep, 0.25)) * 0.0162162162;
				sum *= curvature * fixed4(texGrey,1);

                if (_BlendType == 1)
                    sum = sum * 0.5 + k * 0.5;
                else if (_BlendType == 2)
                    sum = sum < k ? sum : k;
                else if (_BlendType == 3)
                    sum = sum > k ? sum : k;
                else if (_BlendType == 4)
                    sum = sum * k;
                else if (_BlendType == 5)
                    sum = 1 - (1 - sum)*(1 - k);
                else if (_BlendType == 6)
                    sum = k >0.5 ? 1 - 2 * (1 - sum)*(1 - k) : 2 * sum * k;
                else if (_BlendType == 7)
                    sum = k >0.5 ? (2 * k - 1)*(sum - sum * sum) + sum : (2 * k - 1)*(sqrt(sum) - sum) + sum;
					
				UNITY_APPLY_FOG(i.fogCoord, sum);

				return float4(sum.rgb, 1.0);
			}
			ENDCG
		}

		// the first outline pass
		Pass 
		{
			NAME "OUTLINE"
			Cull Front
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			
			float _Outline;
			int _FixOutlineWidth;
			float4 _StrokeColor;
			sampler2D _OutlineNoise;
			float4 _OutlineNoise_ST;
			half _MaxOutlineZOffset;

			struct a2v 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			}; 
			
			struct v2f 
			{
			    float4 pos : SV_POSITION;
				UNITY_FOG_COORDS(0)
			};
			
			v2f vert (a2v v) 
			{
				// fetch Perlin noise map here to map the vertex
				// add some bias by the normal direction
				float2 outlineUV = TRANSFORM_TEX(v.vertex, _OutlineNoise);
				float4 burn = tex2Dlod(_OutlineNoise, float4(outlineUV, 0, 0));

				v2f o = (v2f)0;
				float3 scaledir = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				// scaledir += 0.5;
				scaledir.z = 0.01;
				scaledir = normalize(scaledir);

				// camera space
				float4 position_cs = mul(UNITY_MATRIX_MV, v.vertex);
				position_cs /= position_cs.w;

				float3 viewDir = normalize(position_cs.xyz);
				float3 offset_pos_cs = position_cs.xyz + viewDir * _MaxOutlineZOffset;
                
                // unity_CameraProjection[1].y = fov/2
				float linewidth = -position_cs.z / unity_CameraProjection[1].y;
				linewidth = sqrt(linewidth) * (1 - _FixOutlineWidth) + _FixOutlineWidth;
				position_cs.xy = offset_pos_cs.xy + scaledir.xy * linewidth * burn.x * _Outline ;
				position_cs.z = offset_pos_cs.z;

				o.pos = mul(UNITY_MATRIX_P, position_cs);
				UNITY_TRANSFER_FOG(o,o.pos);

				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target 
			{
				fixed4 c = _StrokeColor;
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}
			ENDCG
		}
		
		// the second outline pass for random part, a little bit wider than last one
		Pass 
		{
			NAME "OUTLINE 2"
			Cull Front
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			
			float _Outline;
			int _FixOutlineWidth;
			float4 _StrokeColor;
			sampler2D _OutlineNoise;
			float4 _OutlineNoise_ST;
			float _OutsideNoiseWidth;
			float _OutsideStrokeDryness;
			half _MaxOutlineZOffset;

			struct a2v 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0; 
			}; 
			
			struct v2f 
			{
			    float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
			};
			
			v2f vert (a2v v) 
			{
				// fetch Perlin noise map here to map the vertex
				// add some bias by the normal direction
				float2 outlineUV = TRANSFORM_TEX(v.vertex, _OutlineNoise);
				float4 burn = tex2Dlod(_OutlineNoise, float4(outlineUV, 0, 0));
				// float4 burn = tex2Dlod(_OutlineNoise, v.vertex);

				v2f o = (v2f)0;
				float3 scaledir = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				// scaledir += 0.5;
				scaledir.z = 0.01;
				scaledir = normalize(scaledir);

				float4 position_cs = mul(UNITY_MATRIX_MV, v.vertex);
				position_cs /= position_cs.w;

				float3 viewDir = normalize(position_cs.xyz);
				float3 offset_pos_cs = position_cs.xyz + viewDir * _MaxOutlineZOffset;

				float linewidth = -position_cs.z / unity_CameraProjection[1].y;
				linewidth = sqrt(linewidth) * (1 - _FixOutlineWidth) + _FixOutlineWidth;
				position_cs.xy = offset_pos_cs.xy + scaledir.xy * linewidth * burn.y * _Outline * 1.1 * _OutsideNoiseWidth ;
				position_cs.z = offset_pos_cs.z;

				o.pos = mul(UNITY_MATRIX_P, position_cs);

				o.uv = TRANSFORM_TEX(v.texcoord, _OutlineNoise);
				UNITY_TRANSFER_FOG(o,o.pos);

				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target 
			{
				// clip random outline here
				fixed4 c = _StrokeColor;
				fixed3 burn = tex2D(_OutlineNoise, i.uv).rgb;
				if (burn.x > _OutsideStrokeDryness)
					discard;
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}
			ENDCG
		}
		
		
	}
	FallBack "Diffuse"
}
