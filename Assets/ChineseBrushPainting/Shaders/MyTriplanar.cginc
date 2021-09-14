// sampler2D _BrushTex;
fixed _BrushOffsetX, _BrushOffsetY, _BrushOffsetZ;
float _TextureScale;
float _TriplanarBlendSharpness;

fixed4 WorldTriplanar(float3 worldPos, float3 worldNormal, sampler2D triTex, fixed BrushOffsetX, fixed BrushOffsetY, fixed BrushOffsetZ, float TextureScale, float TriplanarBlendSharpness)
{
    // !!!Previously, these three ain't exist
    worldPos.x += BrushOffsetX * TextureScale;
    worldPos.y += BrushOffsetY * TextureScale;
    worldPos.z += BrushOffsetZ * TextureScale;

    // find our UVs for each axis based on world position of the fragment
    // !!!I don't know but I changed this place
    half2 yUV = worldPos.xz / TextureScale;
    half2 xUV = worldPos.zy / TextureScale;
    half2 zUV = worldPos.xy / TextureScale;

    // do texture samples from our albedo mao with each of the 3 UV set's we've just made.
    half3 yDiff = tex2D(triTex, yUV);
    half3 xDiff = tex2D(triTex, xUV);
    half3 zDiff = tex2D(triTex, zUV);

    // get the absolute value of the world normal.
    // put the blend weights to the power of BlendSharpness, the higher the value,
    // the sharpness the transition between the planar maps will be.
    half3 blendWeights = pow(abs(worldNormal), TriplanarBlendSharpness);

    // divide our blend mask by the sum of it's components, this will make x+y+z = 1
    blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);

    // finally, blend together all three samples based on the blend mask.
    fixed4 brushTex = fixed4(xDiff * blendWeights.x + yDiff * blendWeights.y + zDiff * blendWeights.z, 1.0);
    return brushTex;
}
fixed4 ObjectTriplanar(float3 worldPos, float3 worldNormal, sampler2D triTex)
{
    worldPos.x += _BrushOffsetX;
    worldPos.y += _BrushOffsetY;
    worldPos.z += _BrushOffsetZ;

    // find our UVs for each axis based on world position of the fragment
    half2 yUV = worldPos.xz / _TextureScale;
    half2 xUV = worldPos.zy / _TextureScale;
    half2 zUV = worldPos.xy / _TextureScale;

    // do texture samples from our albedo mao with each of the 3 UV set's we've just made.
    half3 yDiff = tex2D(triTex, yUV);
    half3 xDiff = tex2D(triTex, xUV);
    half3 zDiff = tex2D(triTex, zUV);

    // get the absolute value of the world normal.
    // put the blend weights to the power of BlendSharpness, the higher the value,
    // the sharpness the transition between the planar maps will be.
    half3 blendWeights = pow(abs(worldNormal), _TriplanarBlendSharpness);

    // divide our blend mask by the sum of it's components, this will make x+y+z = 1
    blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);

    // finally, blend together all three samples based on the blend mask.
    fixed4 brushTex = fixed4(xDiff * blendWeights.x + yDiff * blendWeights.y + zDiff * blendWeights.z, 1.0);
    return brushTex;
}