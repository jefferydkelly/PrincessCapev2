using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : MapTile {

	/// <summary>
	/// Generates the save data.
	/// </summary>
	/// <returns>The save data.</returns>
	protected override string GenerateSaveData()
   {
        KnightRemains remains = GetComponent<KnightRemains>();
	    string data = base.GenerateSaveData();
        data += PCLParser.CreateAttribute("Knight", remains.KnightName);
        data += PCLParser.CreateAttribute("Item", remains.Item);

	    string fileName = "None";
        if (remains.Message != null)
	    {
            fileName = remains.Message.name;
	    }
	    data += PCLParser.CreateAttribute<string>("File", fileName);
	    return data;
   }

   /// <summary>
   /// Froms the data.
   /// </summary>
   /// <param name="tile">Tile.</param>
   public override void FromData(TileStruct tile)
   {
        KnightRemains remains = GetComponent<KnightRemains>();
	    base.FromData(tile);
        remains.KnightName = PCLParser.ParseLine(tile.NextLine);
        remains.Item = PCLParser.ParseEnum<ItemLevel>(tile.NextLine);

	    string fileName = PCLParser.ParseLine(tile.NextLine);
	    if (fileName != "None") {
            remains.Message = Resources.Load<TextAsset>("Cutscenes/" + fileName);
	    }

   }

    /*
    private void OnEnable()
    {
    	Debug.Log("Enabled");
    }

    private void OnDisable()
    {
    	Debug.Log("Disabled");
    }*/
}

