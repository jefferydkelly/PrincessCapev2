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
		mapsAndFiles = new Dictionary<string, string>();
        TextAsset[] texts = Resources.LoadAll<TextAsset>("Levels");

        foreach(TextAsset t in texts)
		{
			string json = t.text;
			string[] lines = json.Split('\n');
			string mapName = PCLParser.ParseLine(lines[1]);
            mapsAndFiles.Add(mapName, t.name);
			mapNames.Add(mapName);
        

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
