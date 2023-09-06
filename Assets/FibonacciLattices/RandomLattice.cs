using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FibonacciLattices
{
    [ExecuteInEditMode]
    public class RandomLattice : LatticeBase
    {
        public override Vector3 GetPosition(int i)
        {
            Vector2 position;
            do
            {
                position = new Vector2(Random.Range(-R,R), Random.Range(-R,R));
            } while (position.magnitude > R);
            
            return position;
        }

        public override Color GetColor(int i) => Color.white;

        public override float GetSize(int i) => PointSize;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, R);
        }
    }
}