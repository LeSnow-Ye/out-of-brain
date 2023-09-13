using UnityEngine;

namespace FibonacciLattices
{
    public class CircleLattice : LatticeBase
    {
        public override Vector3 GetPosition(int i)
        {
            var c = R / Mathf.Sqrt(N);
            var r = c * Mathf.Sqrt(i);
            var phi = CustomPhi ? 2 * Mathf.PI * Phi : GoldenAngle;

            var theta = i * phi;
            return new Vector2(r * Mathf.Cos(theta), r * Mathf.Sin(theta));
        }
    }
}