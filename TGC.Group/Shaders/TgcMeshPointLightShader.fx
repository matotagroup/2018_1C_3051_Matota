/*
* Shader generico para TgcMesh con iluminacion dinamica por pixel (Phong Shading)
* utilizando un tipo de luz Point-Light con atenuacion por distancia
* Hay 3 Techniques, una para cada MeshRenderType:
*	- VERTEX_COLOR
*	- DIFFUSE_MAP
*	- DIFFUSE_MAP_AND_LIGHTMAP
*/

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//ShadowMap

float4x4 g_mViewLightProj;
float4x4 g_mProjLight;
float3 g_vLightPos; // posicion de la luz (en World Space) = pto que representa patch emisor Bj
float3 g_vLightDir; // Direcion de la luz (en World Space) = normal al patch Bj

texture g_txShadow; // textura para el shadow map
sampler2D g_samShadow =
sampler_state
{
    Texture = <g_txShadow>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
    Texture = (texDiffuseMap);
    ADDRESSU = WRAP;
    ADDRESSV = WRAP;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;
};

//Textura para Lightmap
texture texLightMap;
sampler2D lightMap = sampler_state
{
    Texture = (texLightMap);
};

//ShadowMap
#define SMAP_SIZE 1024
#define EPSILON 0.05f

float time;
//Material del mesh
float3 materialEmissiveColor; //Color RGB
float3 materialAmbientColor; //Color RGB
float4 materialDiffuseColor; //Color ARGB (tiene canal Alpha)
float3 materialSpecularColor; //Color RGB
float materialSpecularExp; //Exponente de specular

//Parametros de la Luz
float3 lightColor; //Color RGB de la luz
float4 lightPosition; //Posicion de la luz
float4 eyePosition; //Posicion de la camara
float lightIntensity; //Intensidad de la luz
float lightAttenuation; //Factor de atenuacion de la luz

//Parametros de la Luz para phong
float3 ambientColor; //Color RGB para Ambient de la luz
float3 diffuseColor; //Color RGB para Ambient de la luz
float3 specularColor; //Color RGB para Ambient de la luz
float specularExp; //Exponente de specular



/**************************************************************************************/
/* DIFFUSE_MAP */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_DIFFUSE_MAP
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float4 Color : COLOR;
    float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_DIFFUSE_MAP
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float3 WorldPosition : TEXCOORD1;
    float3 WorldNormal : TEXCOORD2;
    float3 LightVec : TEXCOORD3;
    float3 HalfAngleVec : TEXCOORD4;
};

//Vertex Shader
VS_OUTPUT_DIFFUSE_MAP vs_DiffuseMap(VS_INPUT_DIFFUSE_MAP input)
{
    VS_OUTPUT_DIFFUSE_MAP output;

	//Proyectar posicion
    output.Position = mul(input.Position, matWorldViewProj);

	//Enviar Texcoord directamente
    output.Texcoord = input.Texcoord;

	//Posicion pasada a World-Space (necesaria para atenuacion por distancia)
    output.WorldPosition = mul(input.Position, matWorld);

	/* Pasar normal a World-Space
	Solo queremos rotarla, no trasladarla ni escalarla.
	Por eso usamos matInverseTransposeWorld en vez de matWorld */
    output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;

	//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
    output.LightVec = lightPosition.xyz - output.WorldPosition;

	//ViewVec (V): vector que va desde el vertice hacia la camara.
    float3 viewVector = eyePosition.xyz - output.WorldPosition;

	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
    output.HalfAngleVec = viewVector + output.LightVec;

    return output;
}

//Input del Pixel Shader
struct PS_DIFFUSE_MAP
{
    float2 Texcoord : TEXCOORD0;
    float3 WorldPosition : TEXCOORD1;
    float3 WorldNormal : TEXCOORD2;
    float3 LightVec : TEXCOORD3;
    float3 HalfAngleVec : TEXCOORD4;
};

//Pixel Shader
float4 ps_DiffuseMap(PS_DIFFUSE_MAP input) : COLOR0
{
	//Normalizar vectores
    float3 Nn = normalize(input.WorldNormal);
    float3 Ln = normalize(input.LightVec);
    float3 Hn = normalize(input.HalfAngleVec);

	//Calcular intensidad de luz, con atenuacion por distancia
    float distAtten = length(lightPosition.xyz - input.WorldPosition) * lightAttenuation;
    float intensity = (lightIntensity / distAtten) * 2; //Dividimos intensidad sobre distancia (lo hacemos lineal pero tambien podria ser i/d^2)

	//Obtener texel de la textura
    float4 texelColor = tex2D(diffuseMap, input.Texcoord);

	//Componente Ambient
    float3 ambientLight = intensity * lightColor * materialAmbientColor;

	//Componente Diffuse: N dot L
    float3 n_dot_l = dot(Nn, Ln);
    float3 diffuseLight = intensity * lightColor * materialDiffuseColor.rgb * max(0.0, n_dot_l); //Controlamos que no de negativo

	//Componente Specular: (N dot H)^exp
    float3 n_dot_h = dot(Nn, Hn);
    float3 specularLight = n_dot_l <= 0.0
			? float3(0.0, 0.0, 0.0)
			: (intensity * lightColor * materialSpecularColor * pow(max(0.0, n_dot_h), materialSpecularExp));

	/* Color final: modular (Emissive + Ambient + Diffuse) por el color de la textura, y luego sumar Specular.
	   El color Alpha sale del diffuse material */
    float4 finalColor = float4(saturate(materialEmissiveColor + ambientLight + diffuseLight) * texelColor + specularLight, materialDiffuseColor.a);

    //efecto relampago: return finalColor * time * 100;
    return finalColor;
}

/*
* Technique DIFFUSE_MAP
*/
technique DIFFUSE_MAP
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_DiffuseMap();
        PixelShader = compile ps_3_0 ps_DiffuseMap();
    }
}

//ShadowMap

//Output del Vertex Shader
/*struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float3 Norm : TEXCOORD1; // Normales
    float3 Pos : TEXCOORD2; // Posicion real 3d
};*/

//-----------------------------------------------------------------------------
// Vertex Shader que implementa un shadow map
//-----------------------------------------------------------------------------
void VertShadow(float4 Pos : POSITION,
	//float3 Normal : NORMAL,
	out float4 oPos : POSITION,
	out float2 Depth : TEXCOORD0)
{
	// transformacion estandard
    oPos = mul(Pos, matWorld); // uso el del mesh
    oPos = mul(oPos, g_mViewLightProj); // pero visto desde la pos. de la luz

	// devuelvo: profundidad = z/w
    Depth.xy = oPos.zw;
}

//-----------------------------------------------------------------------------
// Pixel Shader para el shadow map, dibuja la "profundidad"
//-----------------------------------------------------------------------------
void PixShadow(float2 Depth : TEXCOORD0, out float4 Color : COLOR)
{
	// parche para ver el shadow map
	//float k = Depth.x/Depth.y;
	//Color = (1-k);
    Color = Depth.x / Depth.y;
}

technique RenderShadow
{
    pass p0
    {
        VertexShader = compile vs_3_0 VertShadow();
        PixelShader = compile ps_3_0 PixShadow();
    }
}



//Input del Vertex Shader
struct VS_INPUT_DIFFUSE_MAP_PHONG
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float4 Color : COLOR;
    float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_DIFFUSE_MAP_PHONG
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float3 WorldNormal : TEXCOORD1;
    float3 LightVec : TEXCOORD2;
    float3 HalfAngleVec : TEXCOORD3;
    float4 MeshPos : TEXCOORD4;
	float4 vPos : TEXCOORD5;
	float3 vNormal : TEXCOORD6;
	float4 vPosLight : TEXCOORD7;
};

//Vertex Shader
VS_OUTPUT_DIFFUSE_MAP_PHONG vs_DiffuseMapPhong(VS_INPUT_DIFFUSE_MAP_PHONG input)
{
    VS_OUTPUT_DIFFUSE_MAP_PHONG output;

    output.MeshPos = input.Position;

	//Proyectar posicion
    output.Position = mul(input.Position, matWorldViewProj);

	//Enviar Texcoord directamente
    output.Texcoord = input.Texcoord;

	/* Pasar normal a World-Space
	Solo queremos rotarla, no trasladarla ni escalarla.
	Por eso usamos matInverseTransposeWorld en vez de matWorld */
    output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;

	//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
    float3 worldPosition = mul(input.Position, matWorld);
    output.LightVec = lightPosition.xyz - worldPosition;

	//ViewVec (V): vector que va desde el vertice hacia la camara.
    float3 viewVector = eyePosition.xyz - worldPosition;

	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
    output.HalfAngleVec = viewVector + output.LightVec;

	//Parte del Shadow Map
	// propago la normal
    output.vNormal = mul(input.Normal, (float3x3) matWorldView);

	// propago la posicion del vertice en World space
    output.vPos = mul(input.Position, matWorld);

	// propago la posicion del vertice en el espacio de proyeccion de la luz
    output.vPosLight = mul(output.vPos, g_mViewLightProj);

    return output;
}

//Input del Pixel Shader
struct PS_DIFFUSE_MAP_PHONG
{
    float2 Texcoord : TEXCOORD0;
    float3 WorldNormal : TEXCOORD1;
    float3 LightVec : TEXCOORD2;
    float3 HalfAngleVec : TEXCOORD3;
    float4 MeshPos : TEXCOORD4;
	float4 vPos : TEXCOORD5;
	float3 vNormal : TEXCOORD6;
	float4 vPosLight : TEXCOORD7;
};

//Pixel Shader
float4 ps_DiffuseMapPhong(PS_DIFFUSE_MAP_PHONG input) : COLOR0
{

    //float multiplier = ((input.MeshPos.y < 0.5) ? 0 : 1);
    float multiplier = 1;
	//Normalizar vectores
    float3 Nn = normalize(input.WorldNormal);
    float3 Ln = normalize(input.LightVec);
    float3 Hn = normalize(input.HalfAngleVec);

	//Obtener texel de la textura
    float4 texelColor = tex2D(diffuseMap, input.Texcoord);

	//Componente Diffuse: N dot L
    float3 n_dot_l = dot(Nn, Ln);
    float3 diffuseLight = diffuseColor * max(0.0, n_dot_l); //Controlamos que no de negativo

	//Componente Specular: (N dot H)^exp
    float3 n_dot_h = dot(Nn, Hn);
    float3 specularLight = n_dot_l <= 0.0
			? float3(0.0, 0.0, 0.0)
			: specularColor * pow(max(0.0, n_dot_h), specularExp);

	//Color final: modular (Ambient + Diffuse) por el color de la textura, y luego sumar Specular.
    float4 finalColor = float4(saturate(ambientColor + diffuseLight) * texelColor + specularLight, texelColor.a);

    finalColor = float4(finalColor.rgb * 1.2, finalColor.a);

    return finalColor * multiplier;
}

/*
* Technique DIFFUSE_MAP
*/
technique DIFFUSE_MAP_PHONG
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_DiffuseMapPhong();
        PixelShader = compile ps_3_0 ps_DiffuseMapPhong();
    }
}

float4 ps_DiffuseMapPhongConShadow(PS_DIFFUSE_MAP_PHONG input) : COLOR0
{

    //float multiplier = ((input.MeshPos.y < 0.5) ? 0 : 1);
    float multiplier = 1;
	//Normalizar vectores
    float3 Nn = normalize(input.WorldNormal);
    float3 Ln = normalize(input.LightVec);
    float3 Hn = normalize(input.HalfAngleVec);

	//Obtener texel de la textura
    float4 texelColor = tex2D(diffuseMap, input.Texcoord);

	//Componente Diffuse: N dot L
    float3 n_dot_l = dot(Nn, Ln);
    float3 diffuseLight = diffuseColor * max(0.0, n_dot_l); //Controlamos que no de negativo

	//Componente Specular: (N dot H)^exp
    float3 n_dot_h = dot(Nn, Hn);
    float3 specularLight = n_dot_l <= 0.0
			? float3(0.0, 0.0, 0.0)
			: specularColor * pow(max(0.0, n_dot_h), specularExp);

	//Color final: modular (Ambient + Diffuse) por el color de la textura, y luego sumar Specular.
    float4 finalColor = float4(saturate(ambientColor + diffuseLight) * texelColor + specularLight, texelColor.a);

    finalColor = float4(finalColor.rgb * 1.2, finalColor.a);

	//Parte del Shadow Map
	float3 vLight = normalize(float3(input.vPos - g_vLightPos));
    float cono = dot(vLight, g_vLightDir);
    float4 K = 0.0;
    if (cono > 0.7)
    {
		// coordenada de textura CT
        float2 CT = 0.5 * input.vPosLight.xy / input.vPosLight.w + float2(0.5, 0.5);
        CT.y = 1.0f - CT.y;

		// sin ningun aa. conviene con smap size >= 512
        float I = (tex2D(g_samShadow, CT) + EPSILON < input.vPosLight.z / input.vPosLight.w) ? 0.0f : 1.0f;
       /* if (cono < 0.8)
            I *= 1 - (0.8 - cono) * 10;*/

        K = I;
    }

    finalColor.rgb *= 0.5+ 0.5 * K;

    return finalColor * multiplier;
}

/*
* Technique DIFFUSE_MAP
*/
technique DIFFUSE_MAP_PHONG_CON_SHADOW
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_DiffuseMapPhong();
        PixelShader = compile ps_3_0 ps_DiffuseMapPhongConShadow();
    }
}

//Cortar nave

//Input del Vertex Shader
struct VS_INPUT
{
    float4 Position : POSITION0;
    float4 Color : COLOR;
    float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float4 RealPos : TEXCOORD1;
};

VS_OUTPUT vs_corte(VS_INPUT input)
{
    VS_OUTPUT output;
    //Proyectar posicion
    output.RealPos = input.Position, matWorld;
    output.Position = mul(input.Position, matWorldViewProj);

	//Enviar Texcoord directamente
    output.Texcoord = input.Texcoord;

	//Posicion pasada a World-Space (necesaria para atenuacion por distancia)
    //output.WorldPosition = mul(input.Position, matWorld);

    return output;
}

struct PS_INPUT_CORTE
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float3 WorldPosition : TEXCOORD2;
    float4 PosMesh : TEXCOORD4;
};

float4 ps_corte(VS_OUTPUT input) : COLOR0
{
// Obtener el texel de textura
	// diffuseMap es el sampler, Texcoord son las coordenadas interpoladas
    float4 fvBaseColor = tex2D(diffuseMap, input.Texcoord);

    //Agarramos la parte de atras de la nave y de esa parte unicamente aquellos fragmentos que tengan un aspecto bastante rojo.
    if (input.RealPos.x > -33.0 || fvBaseColor.x < 0.9)
        discard;

    //Como el mesh aca se analiza sin haber pasado por los shaders de luz tiene un color muy fuerte en comparacion al de la escena, por eso rebajamos el color de la textura.
    return fvBaseColor * 0.8;

}

technique CortePropulsores
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_corte();
        PixelShader = compile ps_3_0 ps_corte();
    }
}

//Input del Vertex Shader
struct VS_INPUT_NO_EFFECTS
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_NO_EFFECTS
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float2 RealPos : TEXCOORD1;
    float4 Color : COLOR0;
};

//Vertex Shader
VS_OUTPUT_NO_EFFECTS vs_main(VS_INPUT_NO_EFFECTS Input)
{
    VS_OUTPUT_NO_EFFECTS Output;
    Output.RealPos = Input.Position;

	//Proyectar posicion
    Output.Position = mul(Input.Position, matWorldViewProj);
   
	//Propago las coordenadas de textura
    Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
    Output.Color = Input.Color;
    return Output;
}

//Pixel Shader
float4 ps_main(VS_OUTPUT_NO_EFFECTS Input) : COLOR0
{
    return tex2D(diffuseMap, Input.Texcoord);
}

// ------------------------------------------------------------------
technique NO_EFFECTS
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main();
        PixelShader = compile ps_3_0 ps_main();
    }
}
