Shader "GuyBenda/DiceShader"
{
    Properties
    {
        _Tex1 ("Texture 1", 2D) = "white" {}
        _Color1 ("Color 1", Color) = (1,1,1,1)
        _Tex2 ("Texture 2", 2D) = "white" {}
        _Color2 ("Color 2", Color) = (1,1,1,1)
        _Tex3 ("Texture 3", 2D) = "white" {}
        _Color3 ("Color 3", Color) = (1,1,1,1)

    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite On ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _Tex1;
            sampler2D _Tex2;
            sampler2D _Tex3;
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;

            fixed4 frag (v2f input) : SV_Target
            {
                fixed4 tex1 = tex2D(_Tex1, input.uv);
                fixed4 tex2 = tex2D(_Tex2, input.uv);
                fixed4 tex3 = tex2D(_Tex3, input.uv);
                
                tex1 = tex1 * _Color1;
                tex2 = tex2 * _Color2;
                tex3 = tex3 * _Color3;
                
                fixed4 output = lerp(tex1, tex2, tex2.a);
                output = lerp(output, tex3, tex3.a);
                
                return output;
            }
            ENDCG
        }
    }
}
