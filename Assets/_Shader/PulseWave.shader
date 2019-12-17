Shader "Custom/PulseWave"
{
	//Made by Toreole (Torben Mietzner).
	//I claim copyright of this. For I'm very proud of it.
	//Please don't steal :C 
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		//_ImpactTime ("Impact", float) = 0
		_WaveSpeed ("Wave Speed", float ) = 100
		_WaveWidth ("Wave width", float) = 10
		_OffsetStrength ("Offset", float) = 0.05
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
			
			//OLD CODE: 
			//float _ImpactTime;

			//float pixelToCenter(float2 uv)
			//{
			//	uv.x = uv.x * _ScreenParams.x;
			//	uv.y = uv.y * _ScreenParams.y;

			//	return length(uv);
			//}
			
				//step 1: current travelled distance by wave.
				//float waveDist = (_Time.y - _ImpactTime) * _WaveSpeed;

				//the offset (UV space (0,0 -> 1,1)) to the center of the screen.
				//float2 offsetDir = toCenter(i.uv);
				//the magnitude of the offset vector
				//float m_distance = length(offsetDir); //old, pixels: pixelToCenter(offsetDir);

				//normalized offset.
				//float currentOffset = 1 - clamp(abs((waveDist - m_distance) / _WaveWidth), 0, 1);

				//try squaring the offset to make it smoother
				//currentOffset *= currentOffset;

				//the actual offset vector in the uv.
				//float2 uvOffset = normalize(offsetDir) * currentOffset * _OffsetStrength;

				//sample the offset position and do stuff with it.
                //fixed4 col = tex2D(_MainTex, i.uv + uvOffset);

			float2 toCenter (float2 uv, float2 center)
			{
				//simple offset.
				float2 vec = center - uv;
				//remap uv y due to resolution (aspect ratio)
				vec.y *= (_ScreenParams.y / _ScreenParams.x);
				return vec;
			}

			//view buffer
            sampler2D _MainTex;
			//variables for the shader / material
			float _WaveSpeed;
			float _WaveWidth;
			float _OffsetStrength;
			//NEEDS TO BE SET THROUGH A SCRIPT.
			float _Impacts[5];
			float2 _ImpactPoints[5];

            fixed4 frag (v2f idata) : SV_Target
            {
				//base color
                fixed4 color = tex2D(_MainTex, idata.uv);

			    for(uint i = 0; i < 5; i++)
				{
					//current time of the array.
					float mTime = _Impacts[i];
					//travelled wave distance. (screenspace)
					float wDist = (_Time.y - mTime) * _WaveSpeed;

					//offset and distance.
					float2 mOffsetDir = toCenter(idata.uv, _ImpactPoints[i]);
					float mDist = length(mOffsetDir);

					//normalized waveoffset factor
					float cOffset = 1 - clamp(abs((wDist - mDist) / _WaveWidth), 0, 1);
					cOffset *= cOffset;

					float2 mUVOffset = normalize(mOffsetDir) * cOffset * _OffsetStrength;

					fixed4 offsetColor = tex2D(_MainTex, idata.uv + mUVOffset);
					//overwrite the color with the new one, if that one has offset.
					color = lerp(color, offsetColor, cOffset > 0 ? 1 : 0);
				}
				//return the final color
                return color;
            }
            ENDCG
        }
    }
}
