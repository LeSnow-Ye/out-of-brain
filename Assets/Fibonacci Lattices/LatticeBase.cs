using System;
using UnityEngine;

namespace FibonacciLattices
{
    [ExecuteInEditMode]
    public abstract class LatticeBase : MonoBehaviour
    {
        protected const float GoldenRatio = 1.61803398875f;
        protected const float GoldenAngle = 2.39996322972f;

        protected bool _3D = false;
        
        public int N = 1000;
        public float Phi = 137.5f;
        public float R = 4;

        public bool CustomPhi = false;
        public bool LiveUpdate = false;

        public GameObject Point;

        [Header("Color and Size")]
        
        public bool EnableColorGradient = false;
        public Gradient ColorGradient;
        public bool EnableSizeCurve = false;
        public AnimationCurve SizeCurve;
        public float SizeScale = 0.1f;
        public Vector3 BasicSize = Vector3.one;
        public Color BasicColor = Color.white;

        public abstract Vector3 GetPosition(int i);
        public virtual Color GetColor(int i) => EnableColorGradient ? ColorGradient.Evaluate((float)i / N) : BasicColor;
        public virtual float GetSize(int i) => EnableSizeCurve ? SizeCurve.Evaluate((float)i / N) : SizeScale;

        public virtual void UpdatePoint(int i)
        {
            var child = transform.GetChild(i);
            var point = child.gameObject;
            point.transform.localScale = BasicSize * GetSize(i);
            point.transform.localPosition = GetPosition(i);
            switch (_3D)
            {
                case true when Application.isPlaying:
                    point.GetComponent<MeshRenderer>().material.color = GetColor(i);
                    break;
                case false:
                    point.GetComponent<SpriteRenderer>().color = GetColor(i);
                    break;
            }
        }
        
        
        public void UpdateLattice()
        {
            // Insure number of points
            if (N > transform.childCount)
            {
                for (int i = 0; i < N - transform.childCount; i++)
                {
                    var point = Instantiate(Point, Vector3.zero, Quaternion.identity, this.transform);
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
                if (i >= transform.childCount)
                    return;
                UpdatePoint(i);
            }
        }

        public void ClearLattice()
        {
            for (int i = transform.childCount; i > 0; --i)
                DestroyImmediate(transform.GetChild(0).gameObject);
        }

        public virtual void Update()
        {
            if (LiveUpdate)
            {
                UpdateLattice();
            }
        }
    }
}
