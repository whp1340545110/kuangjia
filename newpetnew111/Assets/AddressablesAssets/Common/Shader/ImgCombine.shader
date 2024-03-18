Shader "Custom/ImgCombine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SecondTex ("_SecondTex", 2D) = "white" {}
        //startUV,endUV
        _Pos ("Pos", Vector) = (0,0,0,0)
        _BlendFactor ("_BlendFactor", float) = 1
    }
    CGINCLUDE
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

        v2f vert(appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;
            return o;
        }

        sampler2D _MainTex;
        sampler2D _SecondTex;
        float4 _Pos;
        float _BlendFactor;

        fixed4 frag(v2f i) : SV_Target
        {
            fixed4 mainCol = tex2D(_MainTex, i.uv);
            float secTexUVX = saturate((i.uv.x - _Pos.x) / (_Pos.z - _Pos.x));
            float secTexUVY = saturate((i.uv.y - _Pos.y) / (_Pos.w - _Pos.y));
            fixed4 secCol = tex2D(_SecondTex, float2(secTexUVX, secTexUVY));
            float isInSecArea = i.uv.x > _Pos.x && i.uv.x < _Pos.z && i.uv.y > _Pos.y && i.uv.y < _Pos.w;
            fixed4 col = lerp(mainCol,fixed4(secCol * secCol.a + mainCol * (1 - secCol.a)), isInSecArea);
            return col;
        }

        fixed4 frag2Clear(v2f i) : SV_Target
        {
            return 0;
        }

        fixed4 frag2Black(v2f i) : SV_Target
        {
            return fixed4(0, 0, 0, 1);
        }

        fixed4 frag2White(v2f i) : SV_Target
        {
            return 1;
        }
    ENDCG
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            //必须加Name，否则Graphics.Blit会出错
            Name "Combine"//0
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
        Pass
        {
            Name "Clear2Clear"//1
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag2Clear
            ENDCG
        }
        Pass
        {
            Name "Clear2Black"//2
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag2Black
            ENDCG
        }
        Pass
        {
            Name "Clear2White"//3
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag2White
            ENDCG
        }
    }
}