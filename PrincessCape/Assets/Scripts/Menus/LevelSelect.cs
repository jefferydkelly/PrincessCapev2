using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class LevelSelect : MainMenu {
    [SerializeField]
    GameObject baseButton;

    [SerializeField]
    int numButtons;

    List<Text> buttonText;

    List<string> mapNames;
    Dictionary<string, string> mapsAndFiles;

    int topIndex = 0;
	// Use this for initialization
	void Start () {
        mapNames = new List<string>();
		mapsAndFiles = new Dictionary<string, string>();
        buttonText = new List<Text>();

        for (int i = 0; i < numButtons; i++) {
            Button b = Instantiate(baseButton).GetComponent<Button>();
            b.transform.SetParent(transform);
            b.transform.localScale = Vector3.one;
            b.transform.localPosition = new Vector3(0, -150 + (i * 100));
           
            Text t = b.GetComponentInChildren<Text>();
            buttonText.Add(t);

			b.onClick.AddListener(() => {
                Game.Instance.LoadScene(mapsAndFiles[t.text] + ".json");
			});
        }
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

	public void Decrement()
	{
		topIndex = Mathf.Max(0, topIndex - 1);
        UpdateText();
	}

	public void Increment()
	{
        topIndex = Mathf.Min(mapNames.Count - numButtons, topIndex + 1);
        UpdateText();
	}


	void UpdateText() {
		for (int i = 0; i < buttonText.Count; i++)
		{
            buttonText[i].text = mapNames[(topIndex + i) % mapNames.Count];
		}
    }
}
