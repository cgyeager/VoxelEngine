float4x4 World;
float4x4 View;
float4x4 Projection;

float4 EyePosition;

texture modelTexture;
sampler2D texSampler = sampler_state {
	Texture = (modelTexture);
	MagFilter = Linear;
	MinFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexShaderInput {
	float4 Position : POSITION0;
	float4 Normal   : NORMAL0;
	float4 Color    : COLOR0;
};

struct VertexShaderOutput {
	float4 Position      : POSITION0;
	float4 Normal        : NORMAL0;
	float4 Color         : TEXCOORD0;
	float4 View          : TEXCOORD1;
	float4 WorldPosition : TEXCOORD2;
	float4 EyeSpacePos   : TEXCOORD3;
};

struct FogParameters
{
	float4 color;
	float start;
	float end;
	float density;
};

float GetFogFactor(FogParameters params, float fogCoord) {
	float result = exp(-pow(params.density * fogCoord, 2.f));
	//float result = (params.end - fogCoord) / (params.end - params.start);
	result = 1.f - saturate(result);

	return result;
}


VertexShaderOutput VertexShader_Main(VertexShaderInput input) {

	VertexShaderOutput output;

	output.WorldPosition = mul(input.Position, World); 
	float4 ViewPosition = mul(output.WorldPosition, View);
	output.Position = mul(ViewPosition, Projection);
	output.Color = input.Color;

	float4 normal = normalize(mul(input.Normal, World));
	output.Normal = normal;

	output.View = normalize(EyePosition - output.WorldPosition);
	output.EyeSpacePos = mul(input.Position, World);
	output.EyeSpacePos = mul(input.Position, View);

	return output;
}

float4 PixelShader_Main(VertexShaderOutput input):COLOR0 {
	float4 Ambient = input.Color;
	float4 Diffuse = input.Color;
	float4 Specular = float4(1.f, 1.f, 1.f, 1.f);

	float4 normal = input.Normal;
	float4 LightDir = normalize(float4(3., 30.75, -4.25, 0.f) - input.WorldPosition);
	//float4 LightDir = normalize(float4(0.25, .55, 0.25, 0.f));

	float lightAngle = saturate(dot(LightDir, input.Normal));

	float4 refl = normalize(reflect(-LightDir, normal));

	Ambient *= 0.25f;

	Diffuse *= lightAngle;

	Specular *= pow(saturate(dot(refl, input.View)), 128);
	Specular *= 0.5f;
	float fogCoord = abs(input.EyeSpacePos.z / input.EyeSpacePos.w);

	float4 OutColor = Ambient + Diffuse + Specular;
	FogParameters params;
	params.color = float4(.7, .9, 1.0, 1.0);
	params.start = 1000.f;
	params.end = 2000.f;
	params.density = .0004f;

	OutColor = 	lerp(OutColor, params.color, GetFogFactor(params, fogCoord));
	return OutColor;
}

technique Something {
	pass Pass1 {
		VertexShader = compile vs_4_0 VertexShader_Main();
		PixelShader = compile ps_4_0 PixelShader_Main();
	}
}