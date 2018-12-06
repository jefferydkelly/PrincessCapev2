using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[ExecuteInEditMode]
public class MessageBox : MonoBehaviour {

    List<string> message;
    Text textbox;
    int curLine = 0;
    int currentCharacter = 0;
    float revealTime = 0.05f;
    Timer revealTimer;

	UnityEvent onMessageStart;
	UnityEvent onMessageEnd;
	UnityEvent onLineEnd;

    public void Init() {
        gameObject.SetActive(false);
        textbox = GetComponentInChildren<Text>();
        revealTimer = new Timer(0);
        onLineEnd = new UnityEvent();
        onMessageEnd = new UnityEvent();
        onMessageStart = new UnityEvent();
    }

    public string Text {
        get {
            return textbox.text;
        }

        set {
            if (textbox)
            {
                textbox.text = value;
            }
        }
    }

    public void DisplayLine() {
        if (!gameObject.activeSelf)
        {
            Cutscene.Instance.OnEnd.AddListener(Hide);
            gameObject.SetActive(true);
        }


        textbox.text = ParseKeys(message[0]);
        curLine = 0;
    }

    string ParseKeys(string line) {
        while (line.Contains("[[["))
		{
			int start = line.IndexOf("[[[", System.StringComparison.Ordinal);
			int end = line.IndexOf("]]]", System.StringComparison.Ordinal);
            string sub = line.Substring(start, end - start + 3);
           
            string key = sub.Replace("[[[", "").Replace("]]]", "");
            line = line.Replace(sub, Controller.Instance.GetKey(key, true));

		}

        return line;
    }
    void Hide() {
        Cutscene.Instance.OnEnd.RemoveListener(Hide);
        gameObject.SetActive(false);
    }
    void DisplayMessage() {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
      
			Cutscene.Instance.OnEnd.AddListener(Hide);
        }
        for (int i = 0; i < message.Count; i++) {
            message[i] = ParseKeys(message[i]);
        }
        curLine = 0;
		StartReveal();
    }

    /// <summary>
    /// Starts the reveal.
    /// </summary>
    /// <returns>The reveal.</returns>
	public Timer StartReveal() {
		if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            //EventManager.StartListening("HideMessage", Hide);
            Cutscene.Instance.OnEnd.AddListener(Hide);
        }

		OnMessageStart.Invoke();

		curLine = 0;
		return RevealLine();
    }

    /// <summary>
    /// Starts revealing the line
    /// </summary>
    /// <returns>The reveal timer for the line.</returns>
	Timer RevealLine() {
		currentCharacter = 0;
		textbox.text = "";
        revealTimer = new Timer(revealTime, message[curLine].Length - 1);
        revealTimer.OnTick.AddListener(RevealCharacter);
        Controller.Instance.AnyKey.AddListener(FastReveal);
		if (message.Count > 1)
		{
			revealTimer.OnComplete.AddListener(() =>
			{
				Controller.Instance.AnyKey.RemoveListener(FastReveal);
				Controller.Instance.AnyKey.AddListener(NextLine);
			});
		}
		if (Game.Instance.IsInCutscene)
		{
			return revealTimer;
		} else {
			revealTimer.Start();
			return null;
		}
        
	}

    /// <summary>
    /// Reveals the entire line and stopes the timer.
    /// </summary>
    void FastReveal() {
		if (revealTimer.IsRunning)
		{
			revealTimer.Stop();
			revealTimer.OnComplete.Invoke();
			currentCharacter = message[curLine].Length;
			textbox.text = message[curLine].Substring(0, currentCharacter);
			OnLineEnd.Invoke();
			Controller.Instance.AnyKey.RemoveListener(FastReveal);
			if (message.Count > 1)
			{
				Controller.Instance.AnyKey.AddListener(NextLine);
			}
		}
    }

    /// <summary>
    /// Reveals the next character in the line if there is one.
    /// </summary>
    void RevealCharacter() {
		if (curLine < message.Count)
		{
			currentCharacter++;

			currentCharacter = Mathf.Min(message[curLine].Length, currentCharacter);
			textbox.text = message[curLine].Substring(0, currentCharacter);

			if (currentCharacter == message[curLine].Length)
			{
				OnLineEnd.Invoke();
			}
		}
    }
    /// <summary>
    /// Starts the next line of the message if there is one.  If there isn't, it triggers the end of the message.
    /// </summary>
    void NextLine() {
		if (!revealTimer.IsRunning)
		{
			Controller.Instance.AnyKey.RemoveListener(NextLine);
			curLine++;
           
			if (curLine < message.Count)
			{
				RevealLine();
			}
			else
			{
				Hide();
				OnMessageEnd.Invoke();
			}
		}

    }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>The message.</value>
    public List<string> Message {
        set {
            message = value;
			for (int i = 0; i < message.Count; i++) {
				message[i] = ParseKeys(message[i]);
			}
        }

        get {
            return message;
        }
    }

	public bool IsRevealing {
		get {
            return revealTimer.IsRunning || (message != null && curLine < message.Count);
		}
	}
    public bool IsComplete {
        get {
            if (curLine >= message.Count) {
                return false;
            }
            return currentCharacter == message[curLine].Length;
        }
    }

	public TextAnchor Alignment {
		get {
			return textbox.alignment;
		}

		set {
            if (textbox)
            {
                textbox.alignment = value;
            }
		}
	}

	public UnityEvent OnMessageStart {
		get {
			return onMessageStart;
		}
	}

	public UnityEvent OnMessageEnd {
		get {
			return onMessageEnd;
		}
	}

	public UnityEvent OnLineEnd {
		get {
			return onLineEnd;
		}
	}
}
