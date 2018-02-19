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
        int index = 0;
        int i = 0;
        foreach (FileInfo f in dir.GetFiles("*.json")){
            if (f.Extension != ".json.meta")
            {
                string json = File.ReadAllText(f.FullName);
				string[] lines = json.Split('\n');
                string mapName = PCLParser.ParseLine(lines[1]);
                if (f.Name == theDoor.NextScene) {
                    index = i;
                }
                mapsAndFiles.Add(mapName, f.Name);
                mapNames.Add(mapName);
                i++;
            }

        }

        selectedOption = index;
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
