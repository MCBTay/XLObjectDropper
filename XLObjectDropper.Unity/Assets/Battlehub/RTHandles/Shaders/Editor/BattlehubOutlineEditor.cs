using System;
using UnityEditor;
using UnityEngine;

public class BattlehubOutlineEditor : MaterialEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Material targetMat = target as Material;
       
        bool zt = Convert.ToBoolean(targetMat.GetFloat("_ZTest"));
        EditorGUI.BeginChangeCheck();
        zt = EditorGUILayout.Toggle("ZTest", zt);
        if (EditorGUI.EndChangeCheck())
        {
            targetMat.SetFloat("_ZTest", zt ? 4.0f : 0.0f);
        }
    }
}


