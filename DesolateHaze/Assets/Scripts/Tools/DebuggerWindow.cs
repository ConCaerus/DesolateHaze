using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;


public class DebuggerWindow : EditorWindow {
    [MenuItem("Window/Debugger")]
    public static void showWindow() {
        GetWindow<DebuggerWindow>("Debugger");
    }


    private void OnGUI() {
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Clear Save")) 
            Saver.clearSave();
        GUILayout.EndHorizontal();
    }
}

#endif