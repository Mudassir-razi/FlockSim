using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AutomataController))]
public class AutomataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        AutomataController handler = (AutomataController)target;

        if (GUI.changed)
        {
            handler.Changed();
        }
    }
}
