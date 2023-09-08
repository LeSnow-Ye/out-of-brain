using UnityEngine;
using UnityEditor;

namespace FibonacciLattices
{
    [CustomEditor(typeof(LatticeBase), true)]
    public class LatticeBaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var generator = target as LatticeBase;
            if (GUILayout.Button("Clear Lattice") && generator is not null)
                generator.ClearLattice();
            if (GUILayout.Button("Update Lattice") && generator is not null)
                generator.UpdateLattice();
        }
    }
}