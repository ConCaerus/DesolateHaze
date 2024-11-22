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
        if(GUILayout.Button("Previous Checkpoint")) {
            Saver.untriggerLastCheckpoint(FindFirstObjectByType<CheckpointManager>(), PlayerMovement.I);
            TransitionCanvas.I.loadGame(CheckpointManager.I.aType);
        }
        if(GUILayout.Button("Clear Save"))
            SaveData.wipe();
        if(GUILayout.Button("Next Checkpoint")) {
            Saver.triggerNextCheckpoint(FindFirstObjectByType<CheckpointManager>(), PlayerMovement.I);
            TransitionCanvas.I.loadGame(CheckpointManager.I.aType);
        }
        GUILayout.EndHorizontal();
    }
}

#endif