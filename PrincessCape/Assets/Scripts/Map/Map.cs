using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class Map : MonoBehaviour {
    [SerializeField]
    string levelName = "Level";
    List<MapTile> tiles;

    public void Awake()
    {
        tiles = GetComponentsInChildren<MapTile>().ToList();
        Clear();
    }

    public void Clear() {
		foreach (MapTile tile in tiles)
		{
            tile.HighlightState = MapHighlightState.Normal;
		}
    }
    public void AddTile(MapTile tile) {
        tile.transform.SetParent(transform);
        tiles.Add(tile);
    }

    public void RemoveTile(MapTile tile) {
        tiles.Remove(tile);
        tile.Delete();
    }

    public int NumberOfTiles {
        get {
            return tiles.Count;
        }
    }

    public MapTile GetTile(int tileNum) {
        if (tileNum < NumberOfTiles) {
            return tiles[tileNum];
        }
        return null;
    }

	/// <summary>
	/// Gets the object (if any) at location.
	/// </summary>
	/// <returns>The object at location.</returns>
	/// <param name="spawnPos">Spawn position.</param>
	public MapTile GetObjectAtLocation(Vector3 spawnPos)
	{

		foreach (MapTile goat in tiles)
		{
			Vector3 dif = spawnPos - goat.transform.position;
			Vector3 bounds = goat.Bounds / 2;
			if (dif.x.BetweenEx(-bounds.x, bounds.x) && dif.y.BetweenEx(-bounds.y, bounds.y))
			{
				return goat;
			}

		}

		return null;
	}

    public void RenderInEditor() {
		foreach (MapTile tile in tiles)
		{
			tile.RenderInEditor();
		}
    }
    public string SaveToFile() {
        string info = "{\n";
        info += string.Format("\"MapName\": \"{0}\"", levelName) + ",\n";
        info +="\"Tiles\": [\n";
        foreach(MapTile tile in tiles) {
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
        string tileData = json.Substring(ind);
        return PCLParser.ParseTiles(tileData);
  
    }
}
