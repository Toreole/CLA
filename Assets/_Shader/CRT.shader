Shader "Hidden/CRT"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_NoiseTex ("NoiseTexture", 2D) = "" {}
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

            sampler2D _MainTex;
			sampler2D _NoiseTex;

			float scanline(float2 uv) {
				return sin(_ScreenParams.y * uv.y * 0.7 - _Time.y * 10.0);
			}

			float slowscan(float2 uv) {
				return sin(_ScreenParams.y * uv.y * 0.02 + _Time.y * 6.0);
			}

			float2 colorShift(float2 uv) {
				return float2(
					uv.x,
					uv.y + _SinTime.w*0.02
				);
			}

			float noise(float2 uv) {
				return clamp(tex2D(_NoiseTex, uv.xy + _Time.y*6.0).r +
					tex2D(_NoiseTex, uv.xy - _Time.y*4.0).g, 0.96, 1.0);
			}

			// from https://www.shadertoy.com/view/4sf3Dr
			// Thanks, Jasper
			float2 crt(float2 coord, float bend)
			{
				// put in symmetrical coords
				coord = (coord - 0.5) * 2.0;

				coord *= 0.5;	
	
				// deform coords
				coord.x *= 1.0 + pow((abs(coord.y) / bend), 2.0);
				coord.y *= 1.0 + pow((abs(coord.x) / bend), 2.0);

				// transform back to 0.0 - 1.0 space
				coord  = (coord / 1.0) + 0.5;

				return coord;
			}

			float2 colorshift(float2 uv, float amount, float rand) {
	
				return float2(
					uv.x,
					uv.y + amount * rand // * sin(uv.y * iResolution.y * 0.12 + iTime)
				);
			}

			float2 scandistort(float2 uv) {
				float scan1 = clamp(cos(uv.y * 2.0 + _Time.y), 0.0, 1.0);
				float scan2 = clamp(cos(uv.y * 2.0 + _Time.y + 4.0) * 10.0, 0.0, 1.0) ;
				float amount = scan1 * scan2 * uv.x; 
	
				uv.x -= 0.05 * lerp(tex2D(_NoiseTex, float2(uv.x, amount)).r * amount, amount, 0.9);

				return uv;
	 
			}

			float vignette(float2 uv) {
				uv = (uv - 0.5) * 0.98;
				return clamp(pow(cos(uv.x * 3.1415), 1.2) * pow(cos(uv.y * 3.1415), 1.2) * 50.0, 0.0, 1.0);
			}

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(1,1,1,1);
				
				float2 uv = i.uv.xy; // / _ScreenParams.xy;
				float2 sd_uv = scandistort(uv);
				float2 crt_uv = crt(sd_uv, 2.0);
		
				//float rand_r = sin(iTime * 3.0 + sin(iTime)) * sin(iTime * 0.2);
				//float rand_g = clamp(sin(iTime * 1.52 * uv.y + sin(iTime)) * sin(iTime* 1.2), 0.0, 1.0);
				float4 rand = tex2D(_NoiseTex, float2(_Time.y * 0.01, _Time.y * 0.02));
	
				col.r = tex2D(_MainTex, crt(colorshift(sd_uv, 0.025, rand.r), 2.0)).r;
				col.g = tex2D(_MainTex, crt(colorshift(sd_uv, 0.01, rand.g), 2.0)).g;
				col.b = tex2D(_MainTex, crt(colorshift(sd_uv, 0.024, rand.b), 2.0)).b;	
		
				float sline = scanline(crt_uv);
				float sscan = slowscan(crt_uv);
				float4 scanline_color = float4(sline, sline, sline, 1);
				float4 slowscan_color = float4(sscan, sscan, sscan, 1);
	
				fixed4 fragColor =  lerp(col,  lerp(scanline_color, slowscan_color, 0.5), 0.05);// *
					//vignette(uv) *
					//noise(uv);

                return fragColor;
            }
            ENDCG
        }
    }
}
