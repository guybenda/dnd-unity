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
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityCustomRenderTexture.cginc"

            sampler2D _Tex1;
            sampler2D _Tex2;
            sampler2D _Tex3;
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;

            fixed4 frag (v2f_customrendertexture input) : SV_Target
            {
                fixed4 tex1 = tex2D(_Tex1, input.globalTexcoord);
                fixed4 tex2 = tex2D(_Tex2, input.globalTexcoord);
                fixed4 tex3 = tex2D(_Tex3, input.globalTexcoord);
                
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
