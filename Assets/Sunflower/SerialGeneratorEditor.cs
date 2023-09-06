using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SerialGenerator))]
public class SerialGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var generator = target as SerialGenerator;
        if (GUILayout.Button("Generate") && generator is not null)
        {
            generator.Generate();
        }
        
    }
}
