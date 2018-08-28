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

	List<LevelSelectEntry> maps;

    int topIndex = 0;
	// Use this for initialization
	void Start () {
		
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
			});
        }

        ShowLevels(true);
	}

    /// <summary>
    /// Decrements the index of the topmost element of the list and updates the text of the buttons accordingly.
    /// </summary>
	public void Decrement()
    {
        topIndex = Mathf.Max(0, topIndex - 1);
        UpdateText();
    }

    /// <summary>
    /// Increments the index of the topmost element of the list and updates the text of the buttons accordingly.
    /// </summary>
	public void Increment()
	{
        topIndex = Mathf.Min(maps.Count - numButtons, topIndex + 1);
        UpdateText();
	}


    /// <summary>
    /// Updates the text of the buttons.
    /// </summary>
	void UpdateText() {
        
        if (maps.Count > 0)
        {
            foreach (Button b in buttons)
            {
                b.gameObject.SetActive(false);
            }
            for (int i = 0; i < Mathf.Min(maps.Count, buttons.Count); i++)
            {
                buttons[i].gameObject.SetActive(true);
                buttonText[i].text = maps[(topIndex + i) % maps.Count].Name;
            }
        }
    }

    /// <summary>
    /// Shows the levels to be selected for loading.
    /// </summary>
    /// <param name="showBase">If set to <c>true</c> shows the base levels.  Otherwise, it shows the custom levels.</param>
    public void ShowLevels(bool showBase) {
        
		maps.Clear();
        if (showBase) {
			TextAsset[] texts = Resources.LoadAll<TextAsset>("Levels");

			foreach (TextAsset t in texts)
			{
				string[] text = t.text.Split('\n');

				if (PCLParser.ParseBool(text[3]))
				{
                    AddMap(t.name + ".json", t.text);
				}
			}
			
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
                    StreamReader stream = f.OpenText();
                    string text = stream.ReadToEnd();
                    AddMap(f.FullName, text);
                    stream.Close();

				}
			}

		}

        maps.Sort(delegate (LevelSelectEntry a, LevelSelectEntry b)
        {
            return a.ID.CompareTo(b.ID);

        });
        UpdateText();
    }

    /// <summary>
    /// Adds the map to the list of maps.
    /// </summary>
    /// <param name="fileName">File name.</param>
    /// <param name="text">The text of the level file.</param>
    void AddMap(string fileName, string text) {
        LevelSelectEntry map = new LevelSelectEntry(fileName, text);
        maps.Add(map);
    }
}

struct LevelSelectEntry {
	string mapName;
	string mapFile;
    string mapText;
	int id;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:LevelSelectEntry"/> struct.
    /// </summary>
    /// <param name="file">File name.</param>
    /// <param name="text">The text of the file.</param>
	public LevelSelectEntry(string file, string text) {
        string[] lines = text.Split('\n');
        mapName = PCLParser.ParseLine(lines[1]);
		mapFile = file;
        id = PCLParser.ParseInt(lines[2]);
        mapText = text;
	}

    /// <summary>
    /// Gets the name of the map.
    /// </summary>
    /// <value>The name.</value>
	public string Name {
		get {
			return mapName;
		}
	}

    /// <summary>
    /// Gets the file path of the map.
    /// </summary>
    /// <value>The file.</value>
	public string File {
		get {
			return mapFile;
		}
	}

    /// <summary>
    /// Gets the id number of the map.
    /// </summary>
    /// <value>The identifier.</value>
	public int ID {
		get {
			return id;
		}
	}

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text {
        get {
            return mapText;
        }
    }
}
