using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SaveInputMenu : MonoBehaviour {

    string inputPath;
    List<string> inputFiles;
    [SerializeField]
    InputField fileInput;
    [SerializeField]
    Text messageText;

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
        gameObject.SetActive(false);
    }
    public void CheckOriginal()
	{
		if (fileInput.text.Length > 0)
		{
			fileInput.image.color = Color.white;
            if (IsInputFile(fileInput.text))
			{
				fileInput.textComponent.color = Color.red;
                messageText.text = "A file with this name already exists";
			}
			else
			{
				fileInput.textComponent.color = Color.black;
                messageText.text = "";
			}
		}
		else
		{
			fileInput.image.color = Color.red;
            messageText.text = "Please enter a name";
		}
	}

	public void SaveInput()
	{
		if (fileInput.text.Length > 0 && !IsInputFile(fileInput.text))
		{
            string path = CreatePathForLayout(fileInput.text);
			File.WriteAllText(path, Controller.Instance.Info);
			inputFiles.Add(fileInput.text);
		}
	}

	 bool IsInputFile(string s)
	{
		return inputFiles.Contains(s);
	}

	string CreatePathForLayout(string s)
	{
		return string.Format("{0}/{1}.json", inputPath, s);
	}

}
