Shader "Unlit/Lighting"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _NormalMap("Normal",2D) = "Bump"{}
        _NormalScale("NormalScale",Float) = 1
        _AO("AO",2D) = "white"{}
    }
        SubShader
        {
            Tags{"Queue" = "Transparent""IngnoreProjector" = "True""RenderType" = "Transparent"}
            Cull Off
            ZWrite off
            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                Tags{"LightMode" = "ForwardBase"}

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fwdbase            
                #include "Lighting.cginc"
                #include "UnityCG.cginc"

                struct appdata
                {
                     float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float4 normal:NORMAL;
                    float4 tangent:TANGENT;
                };

                struct v2f
                {
                    float4 uv : TEXCOORD0;
                    UNITY_FOG_COORDS(1)
                    float4 vertex : SV_POSITION;
                    float3 lightDir:TEXCOORD2;
                    float3 wposition:TEXCOORD3;
                    float3 wnormal:TEXCOORD4;
                    //LIGHTING_COORDS(3, 4)
                    float3 vertexlight:TEXCOORD5;
                    float4 aouv:TEXCOORD6;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                sampler2D _NormalMap;
                float4 _NormalMap_ST;
                float _NormalScale;
                sampler2D _AO;
                float4 _AO_ST;


                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.wposition = UnityObjectToWorldDir(v.vertex);
                    o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                    o.uv.zw = TRANSFORM_TEX(v.uv, _NormalMap);
                    o.aouv.xy = TRANSFORM_TEX(v.uv, _AO);
                    float3 binormal = normalize(cross(v.normal, v.tangent.xyz)) * v.tangent.w;
                    float3x3 rotation = float3x3(v.tangent.xyz, binormal, v.normal.xyz);
                    o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
                    UNITY_TRANSFER_FOG(o, o.vertex);

                    o.wnormal = UnityObjectToWorldNormal(v.normal);
                     #ifdef LIGHTMAP_OFF
                        float3 shLight = ShadeSH9(v.normal);
                        o.vertexlight = shLight;
                    #ifdef VERTEXLIGHT_ON
                            float3 vertexlight = Shade4PointLights(unity_4LightPosX0, unity_4LightPosY0,
                                unity_4LightPosZ0, unity_LightColor[0].rgb, unity_LightColor[1].rgb,
                                unity_LightColor[2].rgb, unity_LightColor[3].rgb, unity_4LightAtten0,S
                                o.wposition, o.wnormal);
                            o.vertexlight += vertexlight;
                    #endif

                    #endif
                    return o;
                    }

                    fixed4 frag(v2f i) : SV_TARGET
                    {



                     fixed4 col = tex2D(_MainTex, i.uv.xy);
                    float4 nor = tex2D(_NormalMap, i.uv.zw);
                    float ao = tex2D(_AO, i.aouv.xy).x;
                    float3 normal = UnpackNormal(nor);
                    normal.xy *= _NormalScale;
                    normal = normalize(normal);
                    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
                    fixed3 diffuse = _LightColor0.rgb * abs(dot(normal, normalize(i.lightDir))) * col.rgb;
                    UNITY_APPLY_FOG(i.fogCoord, col);
                    fixed3 color = ambient + diffuse * i.vertexlight*ao;
                    return fixed4(color * col.a, col.a);
                    }
                    ENDCG
            }
                    Pass
                    {
                        //
                    Tags{"LightMode" = "ForwardAdd"}
                    Blend OneMinusDstColor One

                    CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                        //
                    #pragma multi_compile_fwdadd            
                    #include "Lighting.cginc"
                    #include "Autolight.cginc"

                    struct appdata
                    {
                        float4 vertex : POSITION;
                        float2 uv : TEXCOORD0;
                        float3 normal:NORMAL;
                        float4 tangent:TANGENT;
                    };

                    struct v2f
                    {

                        float4 uv : TEXCOORD0;
                        float4 vertex : SV_POSITION;
                        float3 lightDir:TEXCOORD1;
                        float2 aouv:TEXCOORD4;
                        LIGHTING_COORDS(3, 4)
                    };

                    sampler2D _MainTex;
                    float4 _MainTex_ST;
                    sampler2D _NormalMap;
                    float4 _NormalMap_ST;
                    float _NormalScale;
                    sampler2D _AO;
                    float4 _AO_ST;

                    v2f vert(appdata v)
                    {
                        v2f o;
                        o.vertex = UnityObjectToClipPos(v.vertex);
                        o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                        o.uv.zw = TRANSFORM_TEX(v.uv, _NormalMap);
                        o.aouv.xy = TRANSFORM_TEX(v.uv, _AO);
                        TANGENT_SPACE_ROTATION;
                        o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
                        TRANSFER_VERTEX_TO_FRAGMENT(o);
                        return o;
                    }
                            fixed4 frag(v2f i) : SV_TARGET
                            {
                            fixed4 col = tex2D(_MainTex, i.uv.xy);
                            float4 nor = tex2D(_NormalMap, i.uv.zw);
                            float ao = tex2D(_AO, i.aouv.xy).x;
                            float3 normal = UnpackNormal(nor);
                            normal.xy *= _NormalScale;
                            normal = normalize(normal);
                            fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
                            fixed3 diffuse = _LightColor0.rgb * abs(dot(normal, normalize(i.lightDir))) * col.rgb;
                            fixed atten = LIGHT_ATTENUATION(i);
                            fixed3 color = diffuse * atten * ao;
                            return fixed4(color * col.a, col.a);
                            }
                            ENDCG
                        }
        }
}