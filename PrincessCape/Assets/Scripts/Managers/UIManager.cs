using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class UIManager : MonoBehaviour
{
    static UIManager instance;
    //Dialog Boxes
    [SerializeField]
    MessageBox mainText;
	[SerializeField]
	SpeakerBox speakerText;
    [SerializeField]
	Text minorText;

    //Player UI Boxes
    [SerializeField]
    ItemBox itemOneBox;
	[SerializeField]
	ItemBox itemTwoBox;
    [SerializeField]
    InteractionBox interaction;
	[SerializeField]
	HealthBar healthBar;

    [SerializeField]
    Image loadingScreen;
    Timer loadFadeoutTimer;
    bool isHidden = false;

    Timer showTimer;

    private void Awake()
    {
        instance = this;
        showTimer = new Timer(2.0f);
        showTimer.OnTick.AddListener(() =>
        {
            minorText.gameObject.SetActive(true);
            SetMinorText("Press Any Key To Continue");
        });
		OnLineEnd.AddListener(showTimer.Start);

		OnMessageStart.AddListener(itemOneBox.StopListening);
		OnMessageStart.AddListener(itemTwoBox.StopListening);

		OnMessageEnd.AddListener(() =>
		{

		});
		OnMessageEnd.AddListener(itemOneBox.StartListening);
		OnMessageEnd.AddListener(itemTwoBox.StartListening);
        
        Controller.Instance.AnyKey.AddListener(HideText);
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

		Map.Instance.OnLevelLoaded.AddListener(loadFadeoutTimer.Start);

        EventManager.StartListening("LevelOver", ToggleLoadingScreen);
        UpdateKeys();

    }

    /// <summary>
    /// Hides the text.
    /// </summary>
    void HideText() {
        showTimer.Stop();
        minorText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        OnLineEnd.RemoveAllListeners();
        OnMessageStart.RemoveAllListeners();
        OnMessageEnd.RemoveAllListeners();
        Controller.Instance.AnyKey.RemoveListener(HideText);
    }

    private void Start()
    {
        IsHidden = Game.Instance.IsInLevelEditor;
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

    /// <summary>
    /// Shows the message.
    /// </summary>
    /// <returns>The message.</returns>
    /// <param name="line">Line.</param>
    /// <param name="speaker">Speaker.</param>
	public Timer ShowMessage(string line, string speaker = "") {
		SetMainText(line);
		if (speaker != null && speaker.Length > 0)
		{
			speakerText.Speaker = speaker;
			speakerText.Show();
		}
		return mainText.StartReveal();
	}

    /// <summary>
    /// Shows the message.
    /// </summary>
    /// <returns>The message.</returns>
    /// <param name="line">Line.</param>
    /// <param name="speaker">Speaker.</param>
	public Timer ShowMessage(List<string> line, string speaker = "")
    {
        SetMainText(line);
        
		if (speaker != null && speaker.Length > 0)
        {
            speakerText.Speaker = speaker;
            speakerText.Show();
        }
        return mainText.StartReveal();
    }

    /// <summary>
    /// Sets the interaction text.
    /// </summary>
    /// <param name="line">Line.</param>
    public void SetInteractionText(string line)
    {
        if (interaction.IsHidden)
        {
            interaction.IsHidden = false;
        }
        interaction.Text = line;
    }

    /// <summary>
    /// Toggles the loading screen.
    /// </summary>
    void ToggleLoadingScreen()
    {
        loadingScreen.color = loadingScreen.color.SetAlpha(1 - loadingScreen.color.a);
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:UIManager"/> is loading screen up.
    /// </summary>
    /// <value><c>true</c> if is loading screen up; otherwise, <c>false</c>.</value>
    public bool IsLoadingScreenUp {
        get {
            return loadingScreen.color.a <= 0.0f;
        }
    }

    /// <summary>
    /// Updates the key text.
    /// </summary>
    public void UpdateKeys() {
        itemOneBox.KeyText = Controller.Instance.GetKey("ItemOne");
        itemTwoBox.KeyText = Controller.Instance.GetKey("ItemTwo");
        interaction.KeyText = Controller.Instance.GetKey("Interact");
    }

    /// <summary>
    /// Gets or sets the alignment of text in the main text.
    /// </summary>
    /// <value>The alignment.</value>
	public TextAnchor Alignment {
		get {
			return mainText.Alignment;
		}

		set {
			mainText.Alignment = value;
		}
	}

    /// <summary>
    /// Gets the health bar.
    /// </summary>
    /// <value>The health bar.</value>
	public HealthBar HealthBar {
		get {
			return healthBar;
		}
	}

    /// <summary>
    /// Gets the event for when the message starts.
    /// </summary>
    /// <value>The onMessageStart event.</value>
	public UnityEvent OnMessageStart {
		get {
			return mainText.OnMessageStart;
		}
	}

    /// <summary>
    /// Gets the event for when the message ends.
    /// </summary>
    /// <value>The onMessageEnd event.</value>
	public UnityEvent OnMessageEnd
    {
        get
        {
			return mainText.OnMessageEnd;
        }
    }

    /// <summary>
    /// Gets the event for when a line in the message ends.
    /// </summary>
    /// <value>The onLineEnd event.</value>
	public UnityEvent OnLineEnd {
		get {
			return mainText.OnLineEnd;
		}
	}

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:UIManager"/> is revealing message.
    /// </summary>
    /// <value><c>true</c> if is revealing message; otherwise, <c>false</c>.</value>
	public bool IsRevealingMessage {
		get {
			return mainText.IsRevealing;
		}
	}

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:UIManager"/> is hidden.
    /// </summary>
    /// <value><c>true</c> if is hidden; otherwise, <c>false</c>.</value>
    public bool IsHidden {
        get {
            return isHidden;
        }

        set {
            isHidden = value;
            HealthBar.gameObject.SetActive(!value);
            itemOneBox.IsHidden = value;
            itemTwoBox.IsHidden = value;
            interaction.IsHidden = value;
        }
    }
}
