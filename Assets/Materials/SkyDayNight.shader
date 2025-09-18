// SkyDayNight.shader
Shader "Custom/SkyDayNight"
{
    Properties
    {
        _TimeOfDay("Time of Day (0-1)", Range(0,1)) = 0.5
        _DayColor("Day Sky Color", Color) = (0.3,0.6,1,1)
        _NightColor("Night Sky Color", Color) = (0,0,0,1)
        _SunColor("Sun Color", Color) = (1,0.85,0.4,1)
        _MoonColor("Moon Color", Color) = (0.95,0.95,1,1)
        _SunSize("Sun Size", Range(0,0.005)) = 0.002
        _MoonSize("Moon Size", Range(0,0.005)) = 0.0015
        _SunGlow("Sun Glow", Range(0,0.01)) = 0.003
        _MoonGlow("Moon Glow", Range(0,0.01)) = 0.002
        _HorizonColor("Horizon Color", Color) = (0.8,0.9,1,1)
        _HorizonSharpness("Horizon Sharpness", Range(0,10)) = 3.0
        _SunIntensity("Sun Intensity", Range(1,5)) = 3.0
        _MoonIntensity("Moon Intensity", Range(1,3)) = 2.0
        _StarIntensity("Star Intensity", Range(0,1)) = 0.3
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Opaque" "PreviewType"="Skybox" }
        Cull Off 
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata 
            { 
                float4 vertex : POSITION; 
            };
            
            struct v2f 
            { 
                float4 pos : SV_POSITION; 
                float3 viewDir : TEXCOORD0;
            };

            float _TimeOfDay;
            float4 _DayColor, _NightColor, _SunColor, _MoonColor;
            float _SunSize, _MoonSize, _SunGlow, _MoonGlow;
            float4 _HorizonColor;
            float _HorizonSharpness;
            float _SunIntensity, _MoonIntensity, _StarIntensity;


            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                
               
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(worldPos - _WorldSpaceCameraPos);
                
                return o;
            }

            float celestialBody(float3 viewDir, float3 celestialDir, float size, float glow)
            {
                float dist = 1.0 - dot(viewDir, celestialDir);
                float body = smoothstep(size + glow, size, dist);
                float halo = smoothstep(size + glow * 2.0, size, dist) * 0.2;
                return saturate(body + halo);
            }

            float4 frag(v2f i) : SV_Target
            {
                float3 viewDir = normalize(i.viewDir);
                
                float dayFactor = smoothstep(0.25, 0.75, _TimeOfDay);
                float nightFactor = 1.0 - dayFactor;
                
                
                float horizon = 1.0 - abs(viewDir.y);
                horizon = pow(horizon, _HorizonSharpness);
                
               
                float4 skyColor = lerp(_NightColor, _DayColor, dayFactor);
                skyColor = lerp(skyColor, _HorizonColor, horizon * 0.5);
                
                
                float sunAngle = _TimeOfDay * 2.0 * 3.14159;
                float3 sunDir = normalize(float3(sin(sunAngle), cos(sunAngle) * 0.5 + 0.5, 1));
                float3 moonDir = normalize(float3(sin(sunAngle + 3.14159), cos(sunAngle + 3.14159) * 0.5 + 0.5, 1));
                
              
                float sun = celestialBody(viewDir, sunDir, _SunSize, _SunGlow);
                float4 sunColor = _SunColor * sun * _SunIntensity;
                

                float moon = celestialBody(viewDir, moonDir, _MoonSize, _MoonGlow);
                float4 moonColor = _MoonColor * moon * _MoonIntensity;
              
                float stars = 0.0;
                if (nightFactor > 0.1)
                {
                    
                    float2 starUV = viewDir.xz / (viewDir.y + 0.5);
                    float starValue = hash(floor(starUV * 200.0));
                    stars = step(0.995, starValue) * nightFactor * _StarIntensity;
                }
          
                sunColor *= dayFactor;
                moonColor *= nightFactor;
                
          
                float4 finalColor = skyColor + sunColor + moonColor + float4(stars, stars, stars, 0);
                
                return finalColor;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}