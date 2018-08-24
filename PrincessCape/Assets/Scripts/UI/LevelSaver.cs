using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LevelSaver : MonoBehaviour {

    [SerializeField]
    InputField input;
    List<string> baseFileNames;
    List<string> customFileNames;
    [SerializeField]
    Button saveButton;
    [SerializeField]
    Text warningText;
    string customLevelPath = "";
    //A list of all the names of levels in the custom level directory if it exists

    // Use this for initialization
    void Awake()
    {
        baseFileNames = new List<string>();
        customFileNames = new List<string>();

        TextAsset[] texts = Resources.LoadAll<TextAsset>("Levels");

        foreach (TextAsset t in texts)
        {
            baseFileNames.Add(t.name.ToLower());
        }

        customLevelPath = Application.persistentDataPath + "/CustomLevels";

        if (!Directory.Exists(customLevelPath))
        {
            Directory.CreateDirectory(customLevelPath);
        }
        DirectoryInfo dir = new DirectoryInfo(customLevelPath);

        foreach (FileInfo f in dir.GetFiles())
        {
            if (f.Extension == ".json" && f.Extension != ".json.meta")
            {
                customFileNames.Add(f.Name.ToLower());

            }
        }
    }

    private void OnEnable()
    {
        warningText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Makes sure the input is valid.  
    /// </summary>
    public void CheckInput() {
        if (!CanSaveToFile) {
            input.textComponent.color = Color.red;
            saveButton.gameObject.SetActive(false);
            warningText.gameObject.SetActive(true);
            warningText.text = HasBaseFileName ? "A file with that name exists.  It cannot be overwritten." : "The filename contains an inappropriate character like / or .";

        } else {
            input.textComponent.color = Color.black;
            saveButton.gameObject.SetActive(true);
            warningText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Saves the level.
    /// </summary>
    public void SaveLevel() {
        if (customFileNames.Contains(input.text.ToLower())) {
            if (!warningText.text.Contains("overwrite")) {
                warningText.gameObject.SetActive(true);
                warningText.text = "The file name already exists./n Would you like to overwrite it?";
                return;
            }
        }
       
        File.WriteAllText(customLevelPath + "/" + input.text + ".json", Map.Instance.SaveToFile());
        warningText.gameObject.SetActive(false);
        Cancel();

    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:LevelSaver"/> can be saved to file.
    /// </summary>
    /// <value><c>true</c> if can save to file; otherwise, <c>false</c>.</value>
    bool CanSaveToFile {
        get {
            return !( HasBaseFileName || HasInappropriateCharacter);
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:LevelSaver"/> has the same name as a base file.
    /// </summary>
    /// <value><c>true</c> if has base file name; otherwise, <c>false</c>.</value>
    bool HasBaseFileName {
        get {
            return baseFileNames.Contains(input.text.ToLower());
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:LevelSaver"/> has an inappropriate character.
    /// </summary>
    /// <value><c>true</c> if has an inappropriate character; otherwise, <c>false</c>.</value>
    bool HasInappropriateCharacter {
        get {
            return input.text.Contains(".") || input.text.Contains("/");
        }
    }

    /// <summary>
    /// Goes back to the Level Editor.
    /// </summary>
    public void Cancel() {
        LevelEditor.Instance.EndSave();
    }
}
