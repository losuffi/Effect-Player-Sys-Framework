using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System;
using System.Linq;

[CustomEditor(typeof(EffectSequence))]
public class SequenceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawDefaultInspector();

        if(GUILayout.Button("Editor Actors"))
        {
            SequenceBuilder.OtherCall((EffectSequence)target);
        }
    }
}