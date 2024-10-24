using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoidControl))]

public class BoidControlEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BoidControl boidhandler = (BoidControl)target;

        if (GUI.changed)
        {
            boidhandler.Changed();
        }
    }
}
