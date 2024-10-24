using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SlimeControl))]
public class SlimeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SlimeControl handler = (SlimeControl)target;

        if (GUILayout.Button("Reboot simulation"))
        {
            handler.Restart();
        }

        if(GUI.changed)
        {
            handler.Change();
        }
    }
}
