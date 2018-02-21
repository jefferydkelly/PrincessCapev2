using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class Browser : MonoBehaviour {
    
    List<string> inputFiles;
    string inputPath;
    [SerializeField]
    int maxButtons = 4;
    [SerializeField]
    float buttonStartY = 0;
    [SerializeField]
    float buttonSpacing = 150;
    [SerializeField]
    GameObject baseButton;

    [SerializeField]
    Text title;
    [SerializeField]
    Button up;
    [SerializeField]
    Button down;
    [SerializeField]
    Button save;
    [SerializeField]
    InputField fileInput;
	// Use this for initialization
	void Awake () {
       TextAsset[] inputJSON = Resources.LoadAll<TextAsset>("InputLayouts");

        inputFiles = new List<string>();

        inputPath = Application.persistentDataPath + "/InputLayouts";
        if (!Directory.Exists(inputPath)) {
            Directory.CreateDirectory(inputPath);
        }
        DirectoryInfo dir = new DirectoryInfo(inputPath);
        foreach (FileInfo f in dir.GetFiles()) {
            if (f.Extension == ".json" && f.Extension != ".json.meta") {
                inputFiles.Add(Path.GetFileNameWithoutExtension(f.Name));
            }
        }
        EventManager.StartListening("OpenBrowserSave", OpenSave);
        EventManager.StartListening("OpenBrowserLoad", OpenLoad);
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OpenSave() {
        gameObject.SetActive(true);
        title.text = "Save";

		save.gameObject.SetActive(true);
        fileInput.gameObject.SetActive(true);
        up.gameObject.SetActive(false);
        down.gameObject.SetActive(false);
    }

    void OpenLoad() {
        gameObject.SetActive(true);
        title.text = "Load";

        int numButs = Mathf.Min(inputFiles.Count, maxButtons);
        for (int i = 0; i < numButs; i++) {
            Button b = Instantiate(baseButton).GetComponent<Button>();
            b.transform.SetParent(transform);
            b.transform.localScale = Vector3.one;

            //Set the Y so that they are all equally spaced apart
            float y = (down.transform.position - up.transform.position).y * (i + 1) / (numButs + 1);
			b.transform.position = up.transform.position.SetY(up.transform.position.y + y);
            Text t = b.GetComponentInChildren<Text>();
            t.text = inputFiles[i];

			b.onClick.AddListener(() =>
			{
                ParseOption(t.text);
			});
        }

        save.gameObject.SetActive(false);
        fileInput.gameObject.SetActive(false);
        up.gameObject.SetActive(true);
        down.gameObject.SetActive(true);
    }

    public void CheckOriginal() {
        if (fileInput.text.Length > 0)
        {
            fileInput.image.color = Color.white;
            if (inputFiles.Contains(fileInput.text))
            {
                fileInput.textComponent.color = Color.red;
            }
            else
            {
                fileInput.textComponent.color = Color.black;
            }
        } else {
            fileInput.image.color = Color.red;
        }
    }

    public void SaveInput() {
        if (fileInput.text.Length > 0 && !inputFiles.Contains(fileInput.text)) {
            string path = inputPath + "/" + fileInput.text + ".json";
            File.WriteAllText(path, Controller.Instance.Info);
            inputFiles.Add(fileInput.text);
        }
    }

    void ParseOption(string opt) {
        string path = inputPath + "/" + opt + ".json";
        string option = File.ReadAllText(path);
        Dictionary<string, string> dic = PCLParser.ParseDictionary(option);
        Dictionary<string, KeyCode> keyCodes = new Dictionary<string, KeyCode>();
        foreach(KeyValuePair<string, string> kp in dic) {
            keyCodes.Add(kp.Key, (KeyCode)System.Enum.Parse(typeof(KeyCode), kp.Value));
        }
        Controller.Instance.SetKeys(keyCodes);
        EventManager.TriggerEvent("UpdateKeys");
        gameObject.SetActive(false);
    }
}
