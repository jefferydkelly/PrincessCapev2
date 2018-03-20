using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightRemains : InteractiveObject
{

    [SerializeField]
    ItemLevel itemOnRemains = ItemLevel.MagicCape;
    [SerializeField]
    string knightName = "Sir Matthew";
    [SerializeField]
    List<string> message;
    [SerializeField]
    TextAsset messageFile;
    bool itemGiven = false;
    public override void Interact()
    {
        if (!itemGiven)
        {
            //Add item to the player's inventory
            MessageBox.SetMessage(message);
            SpeakerBox.SetSpeaker(knightName);
            EventManager.StartListening("EndOfMessage", GiveItem);
            EventManager.TriggerEvent("ShowDialog");

        }
    }

    void GiveItem()
    {
        itemGiven = true;
        IsHighlighted = false;
        EventManager.StopListening("EndOfMessage", GiveItem);
        Game.Instance.Player.AddItem(ScriptableObject.CreateInstance(itemOnRemains.ToString()) as MagicItem, true);
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

    protected override string GenerateSaveData()
    {
        string data = base.GenerateSaveData();
        data += PCLParser.CreateAttribute("Knight", knightName);
        data += PCLParser.CreateAttribute("Item", itemOnRemains);
        //data += PCLParser.CreateArray("Message", message);
        string fileName = "None";
        if (messageFile != null)
        {
            fileName = messageFile.name;
        }
        data += PCLParser.CreateAttribute<string>("File", fileName);
        return data;
    }

    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        knightName = PCLParser.ParseLine(tile.NextLine);
        itemOnRemains = PCLParser.ParseEnum<ItemLevel>(tile.NextLine);

        message = new List<string>();

        string fileName = PCLParser.ParseLine(tile.NextLine);
        if (fileName != "None") {
            
            messageFile = Resources.Load<TextAsset>("Cutscenes/" + fileName);
            foreach(string s in messageFile.text.Split('\n')) {
                message.Add(s);
            }
        }
        /*
        tile.TossLine();
        while (!tile.FullyRead && !tile.Peek.Contains("]")) {
            message.Add(PCLParser.ParseLine(tile.NextLine));
        }*/
    }
}