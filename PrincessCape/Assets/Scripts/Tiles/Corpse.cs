using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : MapTile {

	
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
}
