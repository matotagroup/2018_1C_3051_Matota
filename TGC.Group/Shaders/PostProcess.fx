/*
* Shaders para efectos de Post Procesadosss
*/

/**************************************************************************************/
/* DEFAULT */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_DEFAULT
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_DEFAULT
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
};

//Vertex Shader
VS_OUTPUT_DEFAULT vs_merge(VS_INPUT_DEFAULT Input)
{
    VS_OUTPUT_DEFAULT Output;

	//Proyectar posicion
    Output.Position = float4(Input.Position.xy, 0, 1);

	//Las Texcoord quedan igual
    Output.Texcoord = Input.Texcoord;

    return (Output);
}

//Textura del Render target 2D
texture escenaTextura;
sampler Escena = sampler_state
{
    Texture = (escenaTextura);
    MipFilter = NONE;
    MinFilter = NONE;
    MagFilter = NONE;
};

texture propulsoresTextura;
sampler Propulsores = sampler_state
{
    Texture = (propulsoresTextura);
    MipFilter = NONE;
    MinFilter = NONE;
    MagFilter = NONE;
};

texture shadowTexture; // textura para el shadow map
sampler2D shadowScene =
sampler_state
{
	Texture = <shadowTexture>;
	MinFilter = NONE;
	MagFilter = NONE;
	MipFilter = NONE;
};
//Input del Pixel Shader
struct PS_INPUT_DEFAULT
{
    float2 Texcoord : TEXCOORD0;
};

//Pixel Shader
float4 ps_merge(PS_INPUT_DEFAULT Input) : COLOR0
{
    return tex2D(Escena, Input.Texcoord) + tex2D(Propulsores, Input.Texcoord)+ tex2D(shadowScene,Input.Texcoord);
}

technique TechniqueMerge
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_merge();
        PixelShader = compile ps_3_0 ps_merge();
    }
}