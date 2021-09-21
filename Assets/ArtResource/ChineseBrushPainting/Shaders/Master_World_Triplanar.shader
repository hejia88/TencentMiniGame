// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Master_World_Triplanar" {
    Properties
	{
		_Color ("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex("Main", 2D) = "white" {}
		_ColorIntensity("Color Intensity",Range(0,1))=0
		_ColorPow("Color Pow", Range(0,2)) = 1
		[Toggle] _ColorFilterOn ("Color Filter On?", Int) = 1
		_NoiseMap("Noise", 2D) = "grey" {}
		[PowerSlider(5.0)] _NoiseIntensity("Noise Intensity",Range(0,1))=0
		_NoiseBias("Noise Bias",Range(-1, 1))=0
		[NoScaleOffset] _BumpTex("Normal", 2D) = "bump" {}
		_BumpScale ("Bump Scale", Range(0, 10)) = 1.0
		_Thred("Edge Thred" , Range(0.01,1)) = 0.25
		_Range("Edge Range" , Range(0,10)) = 1		
		[PowerSlider(3.0)]_Pow("Edge Intensity Pow",Range(0,10))=1
		_SilhouetteRampTex ("Silhouette Ramp Texture", 2D) = "white" {}
		// _SilhouetteTex ("Silhouette Texture", 2D) = "white" {}
		[NoScaleOffset] _BrushTex("Brush Texture", 2D) = "white" {}
		// 当Triplanar关闭的时候，用的是Noise的uv
		[Toggle] _TriplanarOn ("Triplanar On?", Int) = 1
		_BrushOffsetX("Brush Offset X",Range(0,1))=0
		_BrushOffsetY("Brush Offset Y",Range(0,1))=0
		_BrushOffsetZ("Brush Offset Z",Range(0,1))=0
		_TextureScale("Texture Scale", float) = 1
        _TriplanarBlendSharpness("Triplanar Blend Sharpness", float) = 1

		[Enum(Opacity,1,Darken,2,Lighten,3,Multiply,4,Screen,5,Overlay,6,SoftLight,7)]
		_BlendType("Blend Type", Int) = 1
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode ("CullMode", int) = 2
	}
	SubShader {
		Pass {
			Cull [_CullMode]
			Tags { "LightMode"="ForwardBase" }
		
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Lighting.cginc"
			#include "MyTriplanar.cginc"

            fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _ColorIntensity;
			int _ColorFilterOn;
			fixed _ColorPow;
			sampler2D _NoiseMap;
			float4 _NoiseMap_ST;
			fixed _NoiseIntensity;
			fixed _NoiseBias;
			sampler2D _BumpTex;
			fixed _BumpScale;
            float _Thred;
            float _Range;
            float _Pow;
			sampler2D _SilhouetteRampTex;
			// sampler2D _SilhouetteTex;
			int _TriplanarOn;
            int _BlendType;
			sampler2D _BrushTex;
			// fixed _BrushOffsetX, _BrushOffsetY, _BrushOffsetZ;
			// float _TextureScale;
            // float _TriplanarBlendSharpness;

			
			struct a2v {
				float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
				// float3 vdotn : TEXCOORD1;
				float4 TtoW0 : TEXCOORD1;  
				float4 TtoW1 : TEXCOORD2;  
				float4 TtoW2 : TEXCOORD3;
				// float3 worldPos : TEXCOORD4;
				float3 worldNormal : TEXCOORD4;
				float3 objectPos : TEXCOORD5;
				float3 objectNormal : TEXCOORD6;
			};
			
            v2f vert(a2v v)
	        {
		        v2f o;
		        o.pos = UnityObjectToClipPos(v.vertex);
		        o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.texcoord, _NoiseMap);
		        // float3 viewDir = normalize(mul(unity_WorldToObject, float4(_WorldSpaceCameraPos.xyz, 1)).xyz - v.vertex);
		        // o.vdotn = dot(normalize(viewDir), v.normal);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; 
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.objectPos = v.vertex;
				o.objectNormal = v.normal;
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);  
				fixed3 worldBinormal = cross(o.worldNormal, worldTangent) * v.tangent.w; 

				// Compute the matrix that transform directions from tangent space to world space
				// Put the world position in w component for optimization
				o.TtoW0 = float4(worldTangent.x, worldBinormal.x, o.worldNormal.x, worldPos.x);
				o.TtoW1 = float4(worldTangent.y, worldBinormal.y, o.worldNormal.y, worldPos.y);
				o.TtoW2 = float4(worldTangent.z, worldBinormal.z, o.worldNormal.z, worldPos.z);

		        return o;
	        }

			inline fixed GetLuminance (fixed4 inColor)
			{
				return dot (inColor.rgb , fixed3(0.299,0.587,0.114));
			}
		
			fixed4 frag(v2f i) : SV_Target {
				fixed2 Noise = tex2D(_NoiseMap, i.uv.zw).rr;
				Noise = (Noise * 2 - 1 + _NoiseBias) * _NoiseIntensity;
                fixed4 mainTex = tex2D(_MainTex, i.uv.xy + Noise) * _Color;
				// Get the position in world space		
				float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
				// Compute the light and view dir in world space
				// fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				// Get the normal in tangent space
				fixed3 bump = UnpackNormal(tex2D(_BumpTex, i.uv.xy));
				bump.xy *= _BumpScale;
				bump.z = sqrt(1.0 - saturate(dot(bump.xy, bump.xy)));
				// Transform the normal from tangent space to world space
				bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));
				// fixed3 viewbump = mul((float3x3)UNITY_MATRIX_IT_MV, bump);
				// fixed4 curvature= length(fwidth(float4(viewbump,1)));
				// // curvature = curvature > _Cuvthreshold ? tanh((curvature + 1) / 2) : 1;
				// curvature = lerp(1, tanh((curvature + 1) / 2), smoothstep(-0.02, 0.02, curvature - 0.06));
                // return curvature;
				fixed vdotn = abs(dot(viewDir, bump));
	            // fixed texGrey = (mainTex.r + mainTex.g + mainTex.b)*0.33;
				fixed luminanceColor = GetLuminance(mainTex);
				fixed3 texGrey = (luminanceColor, luminanceColor, luminanceColor);
				texGrey = lerp(texGrey, mainTex.rgb, _ColorIntensity);
				if (_ColorFilterOn){
					texGrey.r = pow(texGrey.r, 0.3);
	            	texGrey.r *= 1 - cos(texGrey.r * 3.14);
					texGrey.g = pow(texGrey.g, 0.3);
	    	        texGrey.g *= 1 - cos(texGrey.g * 3.14);
					texGrey.b = pow(texGrey.b, 0.3);
	            	texGrey.b *= 1 - cos(texGrey.b * 3.14);
				}
	           

				// float3 objectNormal = normalize(mul(unity_WorldToObject, bump));
				float3 objectNormal = normalize(mul(bump, unity_ObjectToWorld));
				// fixed4 brushTex = ObjectTriplanar(i.objectPos, i.objectNormal, _BrushTex);
				// fixed4 brushTex = ObjectTriplanar(worldPos, bump, _BrushTex);
				fixed4 brushTex = WorldTriplanar(worldPos, bump, _BrushTex, _BrushOffsetX, _BrushOffsetY, _BrushOffsetZ, _TextureScale, _TriplanarBlendSharpness);
				brushTex = brushTex * _TriplanarOn + tex2D(_BrushTex, i.uv.zw) * (1 - _TriplanarOn);
	            /////////////// fixed4 brushTex = tex2D(_BrushTex, i.uv.zw);
				
	            // fixed brushGrey = (brushTex.r + brushTex.g + brushTex.b)*0.33;
				fixed3 brushGrey = brushTex.rgb;
                fixed3 blend;
                if (_BlendType == 1)
                    blend = texGrey * 0.5 + brushGrey * 0.5;
                else if (_BlendType == 2)
                    blend = texGrey < brushGrey ? texGrey : brushGrey;
                else if (_BlendType == 3)
                    blend = texGrey > brushGrey ? texGrey : brushGrey;
                else if (_BlendType == 4)
                    blend = texGrey * brushGrey;
                else if (_BlendType == 5)
                    blend = 1 - (1 - texGrey)*(1 - brushGrey);
                else if (_BlendType == 6)
                    blend = brushGrey >0.5 ? 1 - 2 * (1 - texGrey)*(1 - brushGrey) : 2 * texGrey * brushGrey;
                else if (_BlendType == 7)
                    blend = brushGrey >0.5 ? (2 * brushGrey - 1)*(texGrey - texGrey * texGrey) + texGrey : (2 * brushGrey - 1)*(sqrt(texGrey) - texGrey) + texGrey;
                fixed4 col = fixed4(blend, 1);
                fixed edge = vdotn / _Range;
                edge = edge > _Thred ? 1 : edge;
                edge = pow(edge, _Pow);
				// return fixed4(edge, edge, edge, 1);
				fixed4 edgeColor = tex2D(_SilhouetteRampTex, fixed2(edge, 0.5));
				// fixed4 edgeTex = ObjectTriplanar(i.objectPos, i.objectNormal, _SilhouetteTex);
                // fixed4 edgeColor = fixed4(edge, edge, edge, 1);
                col = edgeColor > col ? col : edgeColor * (1 - edge) + col * edge;
				// col = edgeTex > col ? col : edgeTex * (1 - edgeColor) + col * edgeColor;
				col = pow(col, _ColorPow);
                return col;
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
