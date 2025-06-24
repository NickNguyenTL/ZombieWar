Shader "Custom/SurfaceTintColor" {
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

    // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
    // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
    // #pragma instancing_options assumeuniformscaling
    UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
    UNITY_INSTANCING_BUFFER_END(Props)

    void vert (inout appdata_full v)
    {

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