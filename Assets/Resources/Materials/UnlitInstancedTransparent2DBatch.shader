// UnlitInstancedTransparent2DBatch.shader
Shader "Custom/UnlitInstancedTransparent2DBatch"
{
    Properties
    {
        _MainTexArray ("Texture Array", 2DArray) = "" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.1
        _TextureIndex ("Texture Index", Float) = 0
        _TextureWidth ("Image Width", Float) = 1
        _TextureHeight ("Image Height", Float) = 1
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent" 
            "IgnoreProjector" = "True" 
        }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma multi_compile _ _ALPHATEST_ON
            #pragma target 3.5
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                nointerpolation float texIndex : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            UNITY_DECLARE_TEX2DARRAY(_MainTexArray);
            float _Cutoff;
            
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
                UNITY_DEFINE_INSTANCED_PROP(float, _TextureIndex)
                UNITY_DEFINE_INSTANCED_PROP(float, _TextureWidth)
                UNITY_DEFINE_INSTANCED_PROP(float, _TextureHeight)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                float2 scale = float2( UNITY_ACCESS_INSTANCED_PROP(Props, _TextureWidth),  UNITY_ACCESS_INSTANCED_PROP(Props, _TextureHeight));
                float4 scaledVertex = float4(v.vertex.x * scale.x, v.vertex.y * scale.y, v.vertex.z, v.vertex.w);

                o.vertex = UnityObjectToClipPos(scaledVertex);
                o.uv = v.uv;
                o.texIndex = UNITY_ACCESS_INSTANCED_PROP(Props, _TextureIndex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float2 uv = i.uv;

                fixed4 texColor = UNITY_SAMPLE_TEX2DARRAY(_MainTexArray, float3(uv, i.texIndex));
                fixed4 instanceColor = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                fixed4 finalColor = texColor * instanceColor;
                
                #if defined(_ALPHATEST_ON)
                    clip(finalColor.a - _Cutoff);
                #endif
                
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                return finalColor;
            }
            ENDCG
        }
    }
}