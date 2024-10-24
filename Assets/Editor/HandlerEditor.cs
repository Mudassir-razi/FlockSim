using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Handler))]
public class ComputeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Handler handler = (Handler)target;

        if (GUI.changed)
        {
            handler.Changed();
        }
    }
}
