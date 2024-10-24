using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Lines))]
public class LineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Lines handler = (Lines)target;

        if (GUI.changed)
        {
            handler.Restart();
        }
    }
}
