Shader "Custom/SurfaceCustomShader" {
    Properties {
        _Color ("Tint", Color) = (1.0, 1.0, 1.0, 1.0)
        _MainTex ("Texture", 2D) = "white" {}     
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _BumpMap  ("Normal Map", 2D) = "bump" {}
        _EmissionMap ("Emission Texture", 2D) = "white" {}
        _Emission("Emission", float) = 0
        [HDR]_EmissionColor("Emission color", Color) = (0,0,0)
        
        _DissolveTexture("Dissolve Texture", 2D) = "white" {}
        _Amount("Amount", Range(0,1)) = 0

        // VAT properties
        _VATTex ("VAT Texture", 2D) = "white" {}
        _VertexCount ("Vertex Count", Float) = 0
        _FrameCount ("Frame Count", Float) = 0
        _CurrentFrame ("Current Frame", Float) = 0
        _AABBMin ("AABB Min", Vector) = (0,0,0,0)
        _AABBMax ("AABB Max", Vector) = (1,1,1,0)
    }
    SubShader {
      //Tags { "RenderType" = "Opaque" }
      Tags { "Queue"="Transparent" "RenderType"="TransparentCutout" }
      CGPROGRAM
    // Physically based Standard lighting model, and enable shadows on all light types
    #pragma surface surf Standard fullforwardshadows vertex:vert
      // Use shader model 3.0 target, to get nicer looking lighting
    #pragma target 3.0

    sampler2D _MaskTex;            
    sampler2D _BumpMap;     

    struct Input {
        float2 uv_MainTex;
        float2 uv_BumpMap;
        INTERNAL_DATA
    }; 

    sampler2D _MainTex;
    sampler2D _EmissionMap;
    half _Glossiness;
    half _Metallic;
    fixed4 _EmissionColor;
    float _Emission;
    fixed4 _Color;

    //Dissolve properties
	sampler2D _DissolveTexture;
	half _Amount;

    // VAT properties
    sampler2D _VATTex;
    float _VertexCount;
    float _FrameCount;
    float _CurrentFrame;
    float3 _AABBMin;
    float3 _AABBMax;

    // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
    // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
    // #pragma instancing_options assumeuniformscaling
    UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
    UNITY_INSTANCING_BUFFER_END(Props)

    // Custom appdata with vertex index in TEXCOORD1.x
    struct appdata_vat
    {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
        float4 tangent : TANGENT;
        float2 texcoord : TEXCOORD0;
        float2 texcoord1 : TEXCOORD1;
        float2 texcoord2 : TEXCOORD2;
        float2 texcoord3 : TEXCOORD3;
        float4 color : COLOR;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    void vert (inout appdata_vat v)
    {
        if(_VertexCount <= 0 || _FrameCount <= 0)  
        {  
            // If VAT counts are not set, just return the original vertex position  
            return;  
        }
        float vertexIndex = v.texcoord1.x;
        float2 vatUV;
        vatUV.x = (vertexIndex + 0.5) / _VertexCount;
        vatUV.y = (_CurrentFrame + 0.5) / _FrameCount;

        float3 normPos = tex2Dlod(_VATTex, float4(vatUV, 0, 0)).rgb;
        float3 animatedPos = lerp(_AABBMin, _AABBMax, normPos);

        v.vertex.xyz = animatedPos;
    }

    void surf (Input IN, inout SurfaceOutputStandard o) 
    {
        //Dissolve function
		half dissolve_value = tex2D(_DissolveTexture, IN.uv_MainTex).r;
		clip(dissolve_value - _Amount);
        
        fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
        o.Albedo = c.rgb;
        o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

        // Metallic and smoothness come from slider variables
        o.Metallic = _Metallic;
        o.Smoothness = _Glossiness;
        o.Emission = _EmissionColor.rgb * tex2D(_EmissionMap, IN.uv_MainTex) * _Emission;
    }
    ENDCG
    } 
    Fallback "Diffuse"
}