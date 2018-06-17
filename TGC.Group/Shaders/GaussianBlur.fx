// ---------------------------------------------------------
// Ejemplo toon Shading
// ---------------------------------------------------------

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/
float screen_dx; // tamaño de la pantalla en pixels
float screen_dy;

texture g_RenderTarget;
sampler RenderTarget =
sampler_state
{
    Texture = <g_RenderTarget>;
    ADDRESSU = CLAMP;
    ADDRESSV = CLAMP;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;
};


void VSCopy(float4 vPos : POSITION, float2 vTex : TEXCOORD0, out float4 oPos : POSITION, out float2 oScreenPos : TEXCOORD0)
{
    oPos = vPos;
    oScreenPos = vTex;
    oPos.w = 1;
}

// Gaussian Blur

static const int kernel_r = 6;
static const int kernel_size = 13;
static const float Kernel[kernel_size] =
{
    0.002216, 0.008764, 0.026995, 0.064759, 0.120985, 0.176033, 0.199471, 0.176033, 0.120985, 0.064759, 0.026995, 0.008764, 0.002216,
};

void BlurH(float2 screen_pos : TEXCOORD0, out float4 Color : COLOR)
{
    // Esta optimizacion mejora muchisimo los fps porque si el color de la textura es negro (el color del clear screen) entonces hace un discard y se evita muchisimos bucles. se aplica aca y en el vertical.
    float4 colorbase = tex2D(RenderTarget, screen_pos);

    if (colorbase.x == 0 && colorbase.y == 0 && colorbase.z == 0)
        discard;

        Color = 0;
    for (int i = 0; i < kernel_size; ++i)
        Color += tex2D(RenderTarget, screen_pos + float2((float) (i - kernel_r) / screen_dx, 0)) * Kernel[i];
    Color.a = 1;
}

void BlurV(float2 screen_pos : TEXCOORD0, out float4 Color : COLOR)
{

    float4 colorbase = tex2D(RenderTarget, screen_pos);

    if (colorbase.x == 0 && colorbase.y == 0 && colorbase.z == 0)
        discard;

    Color = 0;
    for (int i = 0; i < kernel_size; ++i)
        Color += tex2D(RenderTarget, screen_pos + float2(0, (float) (i - kernel_r) / screen_dy)) * Kernel[i];
    Color.a = 1;
}

technique GaussianBlurSeparable
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 VSCopy();
        PixelShader = compile ps_3_0 BlurH();
    }
    pass Pass_1
    {
        VertexShader = compile vs_3_0 VSCopy();
        PixelShader = compile ps_3_0 BlurV();
    }
}

float4 PSDownFilter4(in float2 Tex : TEXCOORD0) : COLOR0
{
    float4 colorbase = tex2D(RenderTarget, Tex);

    if (colorbase.x == 0 && colorbase.y == 0 && colorbase.z == 0)
        discard;

    float4 Color = 0;
    for (int i = 0; i < 4; i++)
        for (int j = 0; j < 4; j++)
            Color += tex2D(RenderTarget, Tex + float2((float) i / screen_dx, (float) j / screen_dy));

    return Color / 16;
}

technique DownFilter4
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 VSCopy();
        PixelShader = compile ps_3_0 PSDownFilter4();
    }
}