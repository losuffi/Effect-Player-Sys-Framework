using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EffectSequence))]
public class SequenceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Editor Actors"))
        {
            SequenceBuilder.OtherCall((EffectSequence)target);
        }
    }
}