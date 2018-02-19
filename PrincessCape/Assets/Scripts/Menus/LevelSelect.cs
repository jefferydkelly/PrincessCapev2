using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class LevelSelect : MainMenu {
    [SerializeField]
    List<Text> buttonText;

    List<string> mapNames;
    Dictionary<string, string> mapsAndFiles;

    int topIndex = 0;
	// Use this for initialization
	void Start () {
		mapNames = new List<string>();
		DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Levels");
		mapsAndFiles = new Dictionary<string, string>();
		foreach (FileInfo f in dir.GetFiles("*.json"))
		{
			if (f.Extension != ".json.meta")
			{
				string json = File.ReadAllText(f.FullName);
				string[] lines = json.Split('\n');
				string mapName = PCLParser.ParseLine(lines[1]);
				mapsAndFiles.Add(mapName, f.Name);
				mapNames.Add(mapName);
			
			}

		}

        UpdateText();
	}
	
    public void Increment() {
        topIndex = (topIndex + 1) % mapNames.Count;
        UpdateText();
    }

    public void Decrement() {
        topIndex--;
        if (topIndex < 0) {
            topIndex += mapNames.Count;
        }
        topIndex = topIndex % mapNames.Count;
        UpdateText();
    }

    void UpdateText() {
		for (int i = 0; i < buttonText.Count; i++)
		{
            buttonText[i].text = mapNames[(topIndex + i) % mapNames.Count];
		}
    }
}
