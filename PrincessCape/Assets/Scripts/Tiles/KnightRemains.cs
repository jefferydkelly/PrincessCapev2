using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightRemains : InteractiveObject
{

    [SerializeField]
    MagicItemType itemOnRemains = MagicItemType.Cape;
    [SerializeField]
    string knightName = "Sir Matthew";
    [SerializeField]
    List<string> message;
    bool itemGiven = false;
    public override void Activate()
    {
        if (!itemGiven)
        {
            //Add item to the player's inventory
            MessageBox.SetMessage(message);
            SpeakerBox.SetSpeaker(knightName);
            EventManager.StartListening("EndOfMessage", GiveItem);
            EventManager.TriggerEvent("Pause");
            EventManager.TriggerEvent("ShowDialog");

        }
    }

    void GiveItem()
    {
        itemGiven = true;
        IsHighlighted = false;
        EventManager.StopListening("EndOfMessage", GiveItem);
        Game.Instance.Player.AddItem(ScriptableObject.CreateInstance(itemOnRemains.ToString()) as MagicItem);
        Timer fadeOutTimer = new Timer(0.05f, 20);
        fadeOutTimer.OnTick.AddListener(()=>{
            myRenderer.color = myRenderer.color.SetAlpha(myRenderer.color.a - 0.05f);
        });

        fadeOutTimer.OnComplete.AddListener(()=> {
            Destroy(gameObject);
        });

        fadeOutTimer.Start();
    }

    /// <summary>
    /// Highlight the object when it touches the player
    /// </summary>
    /// <param name="collision">Collision.</param>
    public new void OnTriggerEnter2D(Collider2D collision)
    {
        if (!itemGiven && collision.CompareTag("Player"))
        {
            IsHighlighted = true;
        }
    }

    public override string SaveData()
    {
		if (transform.parent.name != "Map")
		{
			return "";
		}
		string info = "{\n";
		info += string.Format("\"Name\": \"{0}\"", name.Split('(')[0]) + lineEnding;
		info += string.Format("\"ID\": \"{0}\"", ID) + lineEnding;
		info += string.Format("\"Position\": \"{0}\"", transform.position) + lineEnding;
		info += string.Format("\"Rotation\": \"{0}\"", transform.rotation) + lineEnding;
		info += string.Format("\"Scale\": \"{0}\"", transform.localScale) + lineEnding;
        info += string.Format("\"Knight\": \"{0}\"", knightName) + lineEnding;
        info += "\"Lines\": [\n";
        foreach(string s in message) {
            info += s + lineEnding;
        }
        info += "]" + lineEnding;

		info += "}" + lineEnding;
		return info;
    }

    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        knightName = PCLParser.ParseLine(tile.info[3]);
        message = new List<string>();
        for (int i = 5; i < tile.info.Count - 1; i++) {
            message.Add(tile.info[i].Substring(0, tile.info[i].Length - lineEnding.Length));
        }
    }
}

public enum MagicItemType
{
    Cape,
    PullGlove,
    PushGlove
}