using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FibonacciLattices
{
    public class RandomLattice : LatticeBase
    {
        [Header("Repulsion")]
        public float RepulsionRadius = 2f;
        public bool Enable2dRepulsion = false;
        public bool RepulsionBorder = false;
        public float RepulsionIntensity = 0.2f;
        public float RepulsionDamping = 0.03f;
        
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

        public override float GetSize(int i) => SizeScale;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, R);
        }
    }
}