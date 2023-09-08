using System;
using UnityEngine;

namespace FibonacciLattices
{
    public class SphereLattice : LatticeBase
    {
        private void Start()
        {
            _3D = true;
        }

        public override Vector3 GetPosition(int i)
        {
            var (x, y) = (i / GoldenRatio % 1, (float)i / (N - 1));
            var (phi, theta) = (2 * Mathf.PI * x, Mathf.Acos(1 - 2 * y));
            return new Vector3(Mathf.Cos(phi) * Mathf.Sin(theta), Mathf.Sin(phi) * Mathf.Sin(theta), Mathf.Cos(theta)) * R + transform.position;
        }
    }
}