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
    [SerializeField]
    float startY = 150.0f;
    [SerializeField]
    float spacing = 100.0f;

    List<Text> buttonText;
	List<Button> buttons;

    /*
    List<string> mapNames;
    Dictionary<string, string> mapsAndFiles;
    */
	List<LevelSelectEntry> maps;

    int topIndex = 0;
	// Use this for initialization
	void Start () {
		/*
        mapNames = new List<string>();
		mapsAndFiles = new Dictionary<string, string>();
		*/
		maps = new List<LevelSelectEntry>();
		buttons = new List<Button>();
        buttonText = new List<Text>();

        for (int i = 0; i < numButtons; i++) {
            Button b = Instantiate(baseButton).GetComponent<Button>();
            b.transform.SetParent(transform);
            b.transform.localScale = Vector3.one;
            b.transform.localPosition = new Vector3(0, startY - (i * spacing));
           
            Text t = b.GetComponentInChildren<Text>();
            buttonText.Add(t);
			buttons.Add(b);

			b.onClick.AddListener(() => {
                if (Game.Instance.IsInLevelEditor)
                {
                    LevelEditor.Instance.LoadLevel(maps[topIndex + buttons.IndexOf(b)].File);
                }
                else
                {
                    Game.Instance.LoadScene(maps[topIndex + buttons.IndexOf(b)].File);
                }
                //Game.Instance.LoadScene(mapsAndFiles[t.text] + ".json");
			});
        }

        ShowLevels(true);
	}

	public void Decrement()
	{
		topIndex = Mathf.Max(0, topIndex - 1);
        UpdateText();
	}

	public void Increment()
	{
        topIndex = Mathf.Min(maps.Count - numButtons, topIndex + 1);
        UpdateText();
	}


	void UpdateText() {
		for (int i = 0; i < buttonText.Count; i++)
		{
			buttonText[i].text = maps[(topIndex + i) % maps.Count].Name;
		}
    }

    public void ShowLevels(bool showBase) {
		/*
        mapNames.Clear();
        mapsAndFiles.Clear();
        */
		maps.Clear();
        if (showBase) {
			TextAsset[] texts = Resources.LoadAll<TextAsset>("Levels");

			foreach (TextAsset t in texts)
			{
				string[] text = t.text.Split('\n');

				if (PCLParser.ParseBool(text[3]))
				{
					//AddMap(t.name, t.text)
					LevelSelectEntry map = new LevelSelectEntry(PCLParser.ParseLine(text[1]), t.name + ".json", PCLParser.ParseInt(text[2]));
					maps.Add(map);
				}
			}
			maps.Sort(delegate (LevelSelectEntry a, LevelSelectEntry b)
			{
				return a.ID.CompareTo(b.ID);

			});
		} else {
			string inputPath = Application.persistentDataPath + "/CustomLevels";
			if (!Directory.Exists(inputPath))
			{
				Directory.CreateDirectory(inputPath);
			}
			DirectoryInfo dir = new DirectoryInfo(inputPath);

			foreach (FileInfo f in dir.GetFiles())
			{
				if (f.Extension == ".json" && f.Extension != ".json.meta")
				{
                    //AddMap(f.FullName, File.ReadAllText(f.FullName));

				}
			}

		}
        UpdateText();
    }

    /*
    void AddMap(string fileName, string json) {
		string[] lines = json.Split('\n');
		string mapName = PCLParser.ParseLine(lines[1]);
		mapsAndFiles.Add(mapName,fileName);
		mapNames.Add(mapName);
    }*/
}

struct LevelSelectEntry {
	string mapName;
	string mapFile;
	int id;

	public LevelSelectEntry(string name, string file, int di) {
		mapName = name;
		mapFile = file;
		id = di;
	}

	public string Name {
		get {
			return mapName;
		}
	}

	public string File {
		get {
			return mapFile;
		}
	}

	public int ID {
		get {
			return id;
		}
	}
}
