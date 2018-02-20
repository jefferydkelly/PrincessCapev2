using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(Door))]
public class DoorEditor : Editor {
    List<string> mapNames;
    Dictionary<string, string> mapsAndFiles;
    Door theDoor;
    int selectedOption = 0;

    private void OnEnable()
    {
        
        theDoor = target as Door;
        mapNames = new List<string>();
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Levels");
        mapsAndFiles = new Dictionary<string, string>();

        int i = 0;
        TextAsset[] textFiles = Resources.LoadAll<TextAsset>("Levels");
        foreach (TextAsset text in textFiles){
            string[] lines = text.text.Split('\n');
            string mapName = PCLParser.ParseLine(lines[1]);
            if (text.name + ".json" == theDoor.NextScene) {
                selectedOption = i;
            }
            mapsAndFiles.Add(mapName, text.name + ".json");
            mapNames.Add(mapName);
            i++;


        }
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Next Level");
        selectedOption = EditorGUILayout.Popup(selectedOption, mapNames.ToArray());
        theDoor.NextScene = mapsAndFiles[mapNames[selectedOption]];
        GUILayout.EndHorizontal();
    }
}
