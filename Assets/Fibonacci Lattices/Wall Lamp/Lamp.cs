using System;
using UnityEngine;
using UnityEditor;

namespace FibonacciLattices
{
    public class Lamp : MonoBehaviour
    {
        public int Index;
        public int N;
        public Color TargetColor;
        public float MinIntensity;
        public float MaxIntensity;
        public float LampLightUpDuration;
        public float WallLightUpDuration;
        public float WallStartLightUpTime;
        
        private float lightUpTime = float.MaxValue;
        private Material mat;

        private float GetIntensity(float time)
        {
            var t = (time - lightUpTime) / LampLightUpDuration;
            return Mathf.SmoothStep(MinIntensity, MaxIntensity, t);
        }

        private Color GetHdrColor(float intensity)
        {
            var factor = Mathf.Pow(2, intensity);
            return TargetColor * factor;
        }

        private float GetLightUpTime()
        {
            return Mathf.SmoothStep(WallLightUpDuration, 0, (float)Index / N) + WallStartLightUpTime;
        }
        
        private void Start()
        {
            mat = GetComponent<MeshRenderer>().material;
            lightUpTime = GetLightUpTime();
        }
        
        private void Update()
        {
            var color = GetHdrColor(GetIntensity(Time.time));
            mat.color = color;
        }
    }
}