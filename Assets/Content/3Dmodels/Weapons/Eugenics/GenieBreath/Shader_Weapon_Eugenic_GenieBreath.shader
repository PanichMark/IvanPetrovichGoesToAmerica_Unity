Shader "Custom/EugenicGenie_Shader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _WindStrength ("Wind Strength", Range(0, 1)) = 0.4
        _WindSpeed ("Wind Speed", Range(0.1, 10)) = 2
        _WindDirection ("Wind Direction", Vector) = (1, 0, 0, 0)
        _NoiseScale ("Noise Scale", Range(0.5, 20)) = 5 // Плотность узора
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.2
        _NoiseSpeed ("Noise Speed", Range(0.1, 10)) = 3
        _EffectScale ("Effect Scale", Range(0.1, 10)) = 1.0 // НОВОЕ: Масштаб всего эффекта
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 localPos : TEXCOORD0;
            };

            // --- Свойства ---
            fixed4 _Color;
            float _WindStrength;
            float _WindSpeed;
            float4 _WindDirection;
            float _NoiseScale;
            float _NoiseIntensity;
            float _NoiseSpeed;
            float _EffectScale; // Масштаб

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                // Применяем масштаб к локальной позиции ПЕРЕД передачей во фрагментный шейдер.
                // Это главный секрет. Мы меняем "разрешение" пространства.
                o.localPos = v.vertex.xyz * _EffectScale;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color;
                
                // Используем уже масштабированную позицию из вертексного шейдера
                float3 pos = i.localPos;
                
                float timeWind = _Time.y * _WindSpeed;
                float timeNoise = _Time.y * _NoiseSpeed;
                
                float3 dir = normalize(_WindDirection.xyz);

                // Основная волна (структура линий)
                float wave = sin(dot(pos.xyz * _NoiseScale + timeWind * dir.xyz, dir.xyz));

                // Плавный переливающийся шум
                float noise = sin(pos.x * 10.0 + timeNoise * 5.0) *
                              cos(pos.y * 12.0 + timeNoise * 7.0) *
                              sin(pos.z * 8.0 + timeNoise * 3.0);
                
                noise = noise * 0.5 + 0.5; 

                // Смешивание волны и шума
                float finalWave = lerp(wave, noise * 2.0 - 1.0, _NoiseIntensity);
                
                float alphaMask = finalWave * finalWave; 
                
                col.a *= (1 - alphaMask * _WindStrength);

                return col;
            }
            ENDCG
        }
    }
}