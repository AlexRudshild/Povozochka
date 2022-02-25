Shader "test/grid"
{
    Properties
    {
        [PerRendererData]
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Color" , Color) = (1,1,1,1)
        [space]
        _TilingGrid("Tiling Grid", Vector) = (1, 1, 0, 0)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        
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
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            fixed4 _Color;
            sampler2D _MainTex;    
            fixed2 _TilingGrid;        

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.color = v.color;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {                           
                fixed4 texColor = tex2D(_MainTex, i.uv)*i.color;                                
                return texColor;
            }
            ENDCG
        }
    }
}