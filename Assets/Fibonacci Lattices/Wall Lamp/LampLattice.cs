using System;
using UnityEngine;

namespace FibonacciLattices
{
    public class LampLattice : CircleLattice
    {
        public override void UpdatePoint(int i)
        {
            var point = transform.GetChild(i).gameObject;
            point.transform.localScale = BasicSize * GetSize(i);
            point.transform.localPosition = GetPosition(i);
            
            var lamp = point.GetComponent<Lamp>();
            lamp.Index = i;
            lamp.N = N;
            lamp.TargetColor = GetColor(i);
        }
    }
}