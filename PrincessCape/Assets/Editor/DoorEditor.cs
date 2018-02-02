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
        foreach (FileInfo f in dir.GetFiles("*.pcl")){
            if (f.Extension != ".pcl.meta")
            {
                string json = File.ReadAllText(f.FullName);
				string[] lines = json.Split('\n');
                string mapName = PCLParser.ParseLine(lines[1]);
                mapsAndFiles.Add(mapName, f.FullName);
                mapNames.Add(mapName);
            }
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
