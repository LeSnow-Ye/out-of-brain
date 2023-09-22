using System;
using System.Collections;
using UnityEngine;

namespace FibonacciLattices
{
    public class CircleLattice : LatticeBase
    {
        [Header("Circle Lattice")]
        public bool FixedC = false;
        public bool Animation = false;
        public float C;
        public float AnimationDuration = 5f;
        public int TargetN = 800;

        public override Vector3 GetPosition(int i)
        {
            var c = FixedC ? C : R / Mathf.Sqrt(N);
            var r = c * Mathf.Sqrt(i);
            var phi = CustomPhi ? 2 * Mathf.PI * Phi : GoldenAngle;

            var theta = i * phi;
            return new Vector2(r * Mathf.Cos(theta), r * Mathf.Sin(theta));
        }

        public override void Update()
        {
            base.Update();
            if (!Animation || !Application.isPlaying)
                return;
            
            var t = (Time.time - 1f) / AnimationDuration;
            N = (int)Mathf.SmoothStep(0, TargetN, t);
        }
    }
}