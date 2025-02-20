sampler2D uImage0 : register(s0);
float2 uFlowSpeed; // X/Y flow speed
float uTime; // Shader time
float uDistortionStrength; // Ripple intensity
matrix uTransformMatrix;

texture uTexture;
sampler tex = sampler_state
{
    texture = <uTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

texture uNoise;
sampler Noise = sampler_state
{
    texture = <uTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

struct VertexShaderInput
{
    float2 Coord : TEXCOORD0;
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float2 Coord : TEXCOORD0;
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    output.Position = mul(input.Position, uTransformMatrix);
    output.Color = input.Color;
    output.Coord = input.Coord;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : SV_Target
{
    // Calculate distortion
    float2 noiseOffset = tex2D(Noise, input.Coord + uTime * 0.5).xy;
    noiseOffset = (noiseOffset - 0.5) * 2 * uDistortionStrength;
    
    // Create scrolling UVs with distortion
    float2 scrollUV = input.Coord + (uTime * uFlowSpeed) + noiseOffset;
    
    // Sample texture with scrolling and distortion
    float4 texColor = tex2D(tex, frac(scrollUV));
    
    // Combine with VertexStrip color data
    float4 finalColor = texColor * input.Color;
    
    // Add distortion-based alpha variation
    finalColor.a *= saturate(texColor.a + length(noiseOffset) * 0.5);
    
    return finalColor;
}


technique WaterTechnique
{
    pass WaterPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
        VertexShader = compile vs_2_0 VertexShaderFunction();
    }
}