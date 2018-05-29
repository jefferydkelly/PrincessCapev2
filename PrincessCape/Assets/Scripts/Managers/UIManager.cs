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
    private void Awake()
    {
        instance = this;
        Timer showTimer = new Timer(2.0f);
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

		Map.Instance.OnLevelLoaded.AddListener(loadFadeoutTimer.Start);

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

	public TextAnchor Alignment {
		get {
			return mainText.Alignment;
		}

		set {
			mainText.Alignment = value;
		}
	}

	public HealthBar HealthBar {
		get {
			return healthBar;
		}
	}

	public UnityEvent OnMessageStart {
		get {
			return mainText.OnMessageStart;
		}
	}
    
	public UnityEvent OnMessageEnd
    {
        get
        {
			return mainText.OnMessageEnd;
        }
    }

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
}
