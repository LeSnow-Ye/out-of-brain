using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FibonacciLattices
{
    public class Point : MonoBehaviour
    {
        public LatticeBase Lattice;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (Lattice.Enable2dRepulsion)
            {
                var transformPosition = transform.position;
                var points = Physics2D.OverlapCircleAll(transformPosition, Lattice.RepulsionRadius);
                var force = Vector3.zero;
                foreach (var point in points)
                {
                    var v = transformPosition - point.transform.position;
                    if (v == Vector3.zero)
                        continue;
                    force += v.normalized / v.sqrMagnitude;
                }

                var vBorder = transformPosition.normalized * (Lattice.R + 0.1f) - transformPosition;
                force += vBorder.normalized / vBorder.sqrMagnitude;

                if (force.sqrMagnitude > Lattice.RepulsionDamping * Lattice.RepulsionDamping)
                {
                    force -= force.normalized * Lattice.RepulsionDamping;
                    force = Vector3.ClampMagnitude(force, 1f);
                    transform.position += force * (Lattice.RepulsionIntensity * Time.deltaTime);
                }
            }
        }
    }

}
