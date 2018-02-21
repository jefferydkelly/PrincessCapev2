using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LoadInputBrowser : MonoBehaviour {
	string inputPath;
	List<string> inputFiles;
    List<Text> buttonTexts;
    [SerializeField]
    GameObject baseButton;
    [SerializeField]
    int maxButtons = 4;
    [SerializeField]
    Button up;
    [SerializeField]
    float buttonStartY = 50;
    [SerializeField]
    float buttonSpacing = 25;

    int topIndex = 0;
    int numButs = 0;
    private void Awake()
    {
		inputFiles = new List<string>();

		inputPath = Application.persistentDataPath + "/InputLayouts";
		if (!Directory.Exists(inputPath))
		{
			Directory.CreateDirectory(inputPath);
		}
		DirectoryInfo dir = new DirectoryInfo(inputPath);
		foreach (FileInfo f in dir.GetFiles())
		{
			if (f.Extension == ".json" && f.Extension != ".json.meta")
			{
				inputFiles.Add(Path.GetFileNameWithoutExtension(f.Name));
			}
		}

		numButs = Mathf.Min(inputFiles.Count, maxButtons);
        buttonTexts = new List<Text>();
		for (int i = 0; i < numButs; i++)
		{
			Button b = Instantiate(baseButton).GetComponent<Button>();
			b.transform.SetParent(transform);
			b.transform.localScale = Vector3.one;

            //Set the Y so that they are all equally spaced apart
            float y = buttonStartY - buttonSpacing * i;//up.transform.position.y - initialOffset;
            b.transform.localPosition = up.transform.localPosition.SetY(y);
			Text t = b.GetComponentInChildren<Text>();
			//t.text = inputFiles[i];
            buttonTexts.Add(t);
			b.onClick.AddListener(() =>
			{
				ParseOption(t.text);
			});
		}

        UpdateButtons();
    }
  

	void ParseOption(string opt)
	{
		string path = inputPath + "/" + opt + ".json";
		string option = File.ReadAllText(path);
		Dictionary<string, string> dic = PCLParser.ParseDictionary(option);
		Dictionary<string, KeyCode> keyCodes = new Dictionary<string, KeyCode>();
		foreach (KeyValuePair<string, string> kp in dic)
		{
			keyCodes.Add(kp.Key, (KeyCode)System.Enum.Parse(typeof(KeyCode), kp.Value));
		}
		Controller.Instance.SetKeys(keyCodes);
		EventManager.TriggerEvent("UpdateKeys");
		gameObject.SetActive(false);
	}

    public void Decrement() {
        topIndex = Mathf.Max(0, topIndex - 1);
        UpdateButtons();
    }

    public void Increment() {
        topIndex = Mathf.Min(inputFiles.Count - numButs, topIndex + 1);
        UpdateButtons();
    }

    void UpdateButtons() {
		for (int i = 0; i < numButs; i++)
		{
			buttonTexts[i].text = inputFiles[topIndex + i];
		}
    }
}
