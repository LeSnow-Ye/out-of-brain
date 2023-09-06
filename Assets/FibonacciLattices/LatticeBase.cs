using System;
using UnityEngine;

namespace FibonacciLattices
{
    public abstract class LatticeBase : MonoBehaviour
    {
        protected const double GoldenRatio = 1.61803398875;
        protected const double GoldenAngle = 2.39996322972;

        public int N = 1000;
        public float Phi = 137.5f;
        public float R = 3;
        public float PointSize = 0.02f;

        public bool LiveUpdate = false;
        
        
        public float RepulsionRadius = 2f;
        public bool Enable2dRepulsion = false;
        public float RepulsionIntensity = 0.2f;
        public float RepulsionDamping = 0.03f;
        
        public GameObject Point;
    
        public abstract Vector3 GetPosition(int i);
        public abstract Color GetColor(int i);
        public abstract float GetSize(int i);

        public void UpdateLattice()
        {
            // Insure number of points
            if (N > transform.childCount)
            {
                for (int i = 0; i < N - transform.childCount; i++)
                {
                    var point = Instantiate(Point, Vector3.zero, Quaternion.identity, this.transform);
                    point.GetComponent<Point>().Lattice = this;
                }
            }
            else if (N < transform.childCount)
            {
                for (int i = 0; i < transform.childCount - N; i++)
                    DestroyImmediate(transform.GetChild(0).gameObject);
            }
        
            // Update points
            for (int i = 0; i < N; i++)
            {
                var point = transform.GetChild(i).gameObject;
                point.GetComponent<Renderer>().sharedMaterial.color = GetColor(i);
                point.transform.localScale = Vector3.one * GetSize(i);
                point.transform.localPosition = GetPosition(i);
            }
        }

        public void ClearLattice()
        {
            for (int i = transform.childCount; i > 0; --i)
                DestroyImmediate(transform.GetChild(0).gameObject);
        }

        private void Update()
        {
            if (LiveUpdate)
            {
                UpdateLattice();
            }
        }
    }
}
