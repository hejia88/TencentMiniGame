Shader "Ground_Unlit"
{
    Properties
    {
        [Header(General)]
        _Tiling ("Tiling", Range(1.0, 500.0)) = 1.0

        [Header(Tex)]
        [NoScaleOffset]
        _BaseGroundDiffuse ("Base Ground Diffuse", 2D) = "white" {}
        [NoScaleOffset]
        _RiverDiffuse ("River Diffuse", 2D) = "white" {}
        [NoScaleOffset]
        _RoadDiffuse ("Road Diffuse", 2D) = "white" {}
        [NoScaleOffset]
        _ZoomDiffuse ("Zoom Diffuse", 2D) = "white" {}
        [NoScaleOffset]
        _RiverMask ("River Mask", 2D) = "white" {}
        [NoScaleOffset]
        _RoadMask ("Road Mask", 2D) = "white" {}
        [NoScaleOffset]
        _ZoomMask ("Zoom Mask", 2D) = "white" {}

        [NoScaleOffset]
        _NoiseTex ("Noise", 2D) = "white" {}
    
        _NoiseSelfTiling ("Noise Self Tiling", Range(0.0, 300.0)) = 0.5
        _NoiseBigTiling ("Noise Big Tiling", Range(0.0, 5.0)) = 5
        _NoiseSmallTiling ("Noise Small Tiling", Range(0.0, 5.0)) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float _Tiling;
            float _NoiseBigTiling;
            float _NoiseSmallTiling;
            float _NoiseSelfTiling;

            sampler2D _BaseGroundDiffuse;
            sampler2D _RiverDiffuse;
            sampler2D _RoadDiffuse;
            sampler2D _ZoomDiffuse;

            sampler2D _RiverMask;
            sampler2D _RoadMask;
            sampler2D _ZoomMask;

            sampler2D _NoiseTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 colNoise = tex2D(_NoiseTex, i.uv * _NoiseSelfTiling);

                float2 originalUV = i.uv * _Tiling;
                float2 bigUV = originalUV * _NoiseBigTiling;
                float2 smallUV = originalUV * _NoiseSmallTiling;


                // sample the texture
                // fixed4 colStoneBigUV = tex2D(_BaseGroundDiffuse, bigUV);
                // fixed4 colStoneSmallUV = tex2D(_BaseGroundDiffuse, smallUV);
                // fixed4 colGrassBigUV = tex2D(_RiverDiffuse, bigUV);
                // fixed4 colGrassSmallUV = tex2D(_RiverDiffuse, smallUV);
                
                // fixed4 colStone = lerp(colStoneBigUV, colStoneSmallUV, colNoise.x);
                // fixed4 colGrass = lerp(colGrassBigUV, colGrassSmallUV, colNoise.x);

                fixed4 colBaseGround = tex2D(_BaseGroundDiffuse, originalUV);
                fixed4 colRiver = tex2D(_RiverDiffuse, originalUV);
                fixed4 colRoad = tex2D(_RoadDiffuse, originalUV);
                fixed4 colZoom = tex2D(_ZoomDiffuse, originalUV);

                fixed4 colRiverMask = tex2D(_RiverMask, i.uv);
                fixed4 colRoadMask = tex2D(_RoadMask, i.uv);
                fixed4 colZoomMask = tex2D(_ZoomMask, i.uv);

                fixed4 col = colBaseGround;
                col = lerp(col, colRoad, 1-colRoadMask.x);
                col = lerp(col, colZoom, 1-colZoomMask.x);
                col = lerp(col, colRiver, 1-colRiverMask.x);

                // col = colNoise;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
