using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    static UIManager instance;
    [SerializeField]
    MessageBox mainText;
	[SerializeField]
	SpeakerBox speakerText;
    [SerializeField]
	Text minorText;
    [SerializeField]
    ItemBox itemOneBox;
	[SerializeField]
	ItemBox itemTwoBox;
    [SerializeField]
    InteractionBox interaction;
    [SerializeField]
    Image loadingScreen;
    Timer loadFadeoutTimer;
    private void Awake()
    {
        instance = this;
        Timer showTimer = new Timer(2.0f);
        showTimer.OnTick.AddListener(() =>
        {
            minorText.gameObject.SetActive(true);
            SetMinorText("Press Any Key To Continue");
        });
        EventManager.StartListening("EndOfLine", () =>
        {
            showTimer.Start();
        });

		Controller.Instance.AnyKey.AddListener(() =>
		{
			showTimer.Stop();
			minorText.gameObject.SetActive(false);
		});
        //EventManager.StartListening("AnyKey", );

        interaction.Text = "";
        minorText.gameObject.SetActive(false);
        ToggleLoadingScreen();

        loadFadeoutTimer = new Timer(1.0f / 30.0f, 30);
        loadFadeoutTimer.OnTick.AddListener(()=> {
            loadingScreen.color = loadingScreen.color.SetAlpha(1 - loadFadeoutTimer.RunPercent);
        });

        loadFadeoutTimer.OnComplete.AddListener(()=> {
            loadingScreen.color = loadingScreen.color.SetAlpha(0);
        });


        EventManager.StartListening("LevelLoaded", ()=> {
            loadFadeoutTimer.Start();
        });

        EventManager.StartListening("LevelOver", ToggleLoadingScreen);
        UpdateKeys();
    }

    /// <summary>
    /// Gets the instance of UI Manager.
    /// </summary>
    /// <value>The instance.</value>
    public static UIManager Instance
    {
        get
        {
            if (instance == null) {
                instance = FindObjectOfType<UIManager>();
            }
            return instance;
        }
    }

    /// <summary>
    /// Sets the main text.
    /// </summary>
    /// <param name="line">Line.</param>
    public void SetMainText(string line)
    {
        mainText.Message = new List<string>() { line };
    }

    /// <summary>
    /// Sets the main text.
    /// </summary>
    /// <param name="message">Message.</param>
    public void SetMainText(List<string> message)
    {
        mainText.Message = message;
    }

    /// <summary>
    /// Sets the minor text.
    /// </summary>
    /// <param name="line">Line.</param>
	public void SetMinorText(string line)
    {
		minorText.text = line;
    }

	public Timer ShowMessage(string line, string speaker = "") {
		SetMainText(line);
		if (speaker != null && speaker.Length > 0)
		{
			speakerText.Speaker = speaker;
			speakerText.Show();
		}
		return mainText.StartReveal();
	}

	public Timer ShowMessage(List<string> line, string speaker = "")
    {
        SetMainText(line);
		Debug.Log(line);
		if (speaker != null && speaker.Length > 0)
        {
            speakerText.Speaker = speaker;
            speakerText.Show();
        }
        return mainText.StartReveal();
    }

    public void SetInteractionText(string line)
    {
        if (interaction.IsHidden)
        {
            interaction.IsHidden = false;
        }
        interaction.Text = line;
    }

    void ToggleLoadingScreen()
    {
        loadingScreen.color = loadingScreen.color.SetAlpha(1 - loadingScreen.color.a);
    }

    public bool IsLoadingScreenUp {
        get {
            return loadingScreen.color.a <= 0.0f;
        }
    }

    public void UpdateKeys() {
        itemOneBox.KeyText = Controller.Instance.GetKey("ItemOne");
        itemTwoBox.KeyText = Controller.Instance.GetKey("ItemTwo");
        interaction.KeyText = Controller.Instance.GetKey("Interact");
    }
}
