using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FibonacciLattices
{
    public class PointWithRepulsion : MonoBehaviour
    {
        private RandomLattice _lattice;
        
        // Start is called before the first frame update
        void Start()
        {
            _lattice = GetComponentInParent<RandomLattice>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_lattice.Enable2dRepulsion)
            {
                var transformPosition = transform.position;
                var points = Physics2D.OverlapCircleAll(transformPosition, _lattice.RepulsionRadius * _lattice.R);
                var force = Vector3.zero;
                foreach (var point in points)
                {
                    var v = transformPosition - point.transform.position;
                    if (v == Vector3.zero)
                        continue;
                    force += v.normalized / v.sqrMagnitude;
                }

                if (_lattice.RepulsionBorder)
                {
                    transformPosition = transform.localPosition;
                    var vBorder = transformPosition.normalized * (_lattice.R + 0.1f) - transformPosition;
                    force += vBorder.normalized / vBorder.sqrMagnitude;
                }

                if (force.sqrMagnitude > _lattice.RepulsionDamping * _lattice.RepulsionDamping)
                {
                    force -= force.normalized * _lattice.RepulsionDamping;
                    force = Vector3.ClampMagnitude(force, 1f);
                    transform.position += force * (_lattice.RepulsionIntensity * Time.deltaTime);
                }
            }
        }
    }

}
