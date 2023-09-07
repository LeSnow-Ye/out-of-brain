using UnityEngine;

namespace FibonacciLattices
{
    public class SquareLattice : LatticeBase
    {
        public override Vector3 GetPosition(int i)
        {
            return (new Vector2(i / GoldenRatio % 1, (float)i / (N - 1)) - Vector2.one * 0.5f) * R;
        }
    }
}