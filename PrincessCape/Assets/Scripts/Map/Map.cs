using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {
    [SerializeField]
    string levelName = "Level";
    public string SaveToFile() {
        string info = "{\n";
        info += string.Format("\"MapName\": \"{0}\"", levelName) + ",\n";
        info +="\"Tiles\": [\n";
        foreach(MapTile tile in GetComponentsInChildren<MapTile>()) {
            info += tile.SaveData();
        }
        info += "]\n}";
        return info;
    }
    public List<TileStruct> LoadFromFile(string json) {
        string[] lines = json.Split('\n');
        string mapName = lines[1].Split(':')[1];
        levelName = mapName.Substring(2, mapName.Length - 4);
        int ind = json.IndexOf(',');
        string tiles = json.Substring(ind);
        return PCLParser.ParseTiles(tiles);
  
    }
}
