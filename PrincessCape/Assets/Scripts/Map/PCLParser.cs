using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCLParser {

    public static Vector3 ParseVector3(string pcl) {
        string firstSub = pcl.Split('(')[1];
        firstSub = pcl.Split(')')[0];
        string[] xyz = firstSub.Split(',');
        return new Vector3(float.Parse(xyz[0].Trim()), float.Parse(xyz[1].Trim()), float.Parse(xyz[2].Trim()));
    }

    public static List<TileStruct> ParseTiles(string pcl) {
        List<TileStruct> tiles = new List<TileStruct>();
        int ind = pcl.IndexOf('[') + 2;
        int lastInd = pcl.LastIndexOf(']');
        pcl = pcl.Substring(ind, lastInd - ind);
        string[] tilesList = pcl.Split('\n');

        for (int i = 0; i < tilesList.Length; i++)
        {
            if (tilesList[i] == "{")
            {
                List<string> toParse = new List<string>();
                int j = i + 1;
             
                while (j < tilesList.Length && !tilesList[j].Contains("}")) {
                   
                    toParse.Add(tilesList[j]);
                    j++;
                }
              
                tiles.Add(ParseTileStruct(toParse));
                i = j;
            }
        }
        return tiles;
    }

    public static TileStruct ParseTileStruct (List<string> tile) {
        TileStruct ts = new TileStruct();
        for (int i = 0; i < tile.Count; i++) {
            string s = tile[i];

			if (s.Contains("\"Name\": "))
			{
				ts.name = ParseLine(s);
			}
			else if (IsLine(s))
			{
                ts.AddInfo(s);
			}
        }
     
        return ts;
    }

    public static string ParseLine(string line) {
        string tName = line.Substring(line.IndexOf(':') + 3);
		tName = tName.Substring(0, tName.Length - 2);
        return tName;
    }

    static bool IsLine(string s) {
        return !(s.Contains("{") || s.Contains("}"));
    }
}

public struct TileStruct {
    public string name;
    public List<string> info;

    public void AddInfo(string s) {
        if (info == null) {
            info = new List<string>();
        }
        info.Add(s);
    }
}
