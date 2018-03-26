using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    static UIManager instance;
    [SerializeField]
    MessageBox mainText;
    [SerializeField]
    Text minorText;
    [SerializeField]
    InteractionBox interaction;

    private void Awake()
    {
        instance = this;
        Timer showTimer = new Timer(2.0f);
        showTimer.OnTick.AddListener(()=> {
			minorText.gameObject.SetActive(true);
			SetMinorText("Press Any Key To Continue");
        });
        EventManager.StartListening("EndOfLine", ()=> {
            showTimer.Start();
        });

        EventManager.StartListening("AnyKey", ()=> {
            showTimer.Stop();
            minorText.gameObject.SetActive(false);
        });

        interaction.Text = "";
        minorText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Gets the instance of UI Manager.
    /// </summary>
    /// <value>The instance.</value>
    public static UIManager Instance {
        get {
            return instance;
        }
    }

    /// <summary>
    /// Sets the main text.
    /// </summary>
    /// <param name="line">Line.</param>
    public void SetMainText(string line) {
        mainText.Message = new List<string>() { line };
    }

    /// <summary>
    /// Sets the main text.
    /// </summary>
    /// <param name="message">Message.</param>
    public void SetMainText(List<string> message) {
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

    public void SetInteractionText(string line) {
        if (interaction.IsHidden) {
            interaction.IsHidden = false;
        }
        interaction.Text = line;
    }

}
