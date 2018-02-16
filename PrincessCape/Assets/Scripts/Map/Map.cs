using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

[ExecuteInEditMode]
public class Map : MonoBehaviour
{
    [SerializeField]
    string levelName = "Level";
    List<MapTile> tiles;
    Dictionary<string, GameObject> prefabs;
    public void Awake()
    {
		Object[] obj = Resources.LoadAll("Tiles", typeof(GameObject));

		prefabs = new Dictionary<string, GameObject>(obj.Length);

		for (int i = 0; i < obj.Length; i++)
		{
			GameObject go = (GameObject)obj[i];
			prefabs.Add(go.name, go);
		}

		tiles = GetComponentsInChildren<MapTile>().ToList();
        ClearHighlights();
    }

    public void AssignIDs()
    {
        int id = 0;

        foreach (MapTile tile in tiles)
        {
            if (tile.ID <= 0)
            {
                while (TileHasID(id))
                {
                    id++;
                }
                tile.ID = id;
            }
        }
    }

    public void Clear()
    {
        if (Application.isPlaying)
        {
			foreach (MapTile tile in tiles)
			{
                if (tile)
                {
                    Destroy(tile.gameObject);
                }
			}
        }
        else
        {
            foreach (MapTile tile in tiles)
            {
                DestroyImmediate(tile.gameObject, false);
            }
        }

        tiles = new List<MapTile>();

    }
    public void ClearHighlights()
    {
        foreach (MapTile tile in tiles)
        {
            tile.HighlightState = MapHighlightState.Normal;
        }
    }
    public void AddTile(MapTile tile)
    {
        tile.transform.SetParent(transform);

        tiles.Add(tile);

        if (tile.ID < 0)
        {
            tile.ID = NumberOfTiles;
        }
    }

    public void RemoveTile(MapTile tile)
    {
        tiles.Remove(tile);
#if UNITY_EDITOR
        tile.Delete();
#else
        Destroy(tile.gameObject);
#endif
    }

    public int NumberOfTiles
    {
        get
        {
            return tiles.Count;
        }
    }

    public MapTile GetTile(int tileNum)
    {
        if (tileNum < NumberOfTiles)
        {
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

        foreach (MapTile tile in tiles)
        {
            if (tile.Overlaps(spawnPos))
            {
                return tile;
            }

        }

        return null;
    }

#if UNITY_EDITOR
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
#endif

	public List<TileStruct> LoadFromFile(string json) {
        string[] lines = json.Split('\n');
        string mapName = lines[1].Split(':')[1];

        levelName = mapName.Substring(2, mapName.Length - 4);
        int ind = json.IndexOf(',');
        string tileData = json.Substring(ind);
        return PCLParser.ParseTiles(tileData);
    }

    public MapTile GetTileByID(int id) {
        foreach(MapTile tile in tiles) {
            if (tile.ID == id) {
                return tile;
            }
        }

        return null;
    }

    bool TileHasID(int id) {
		foreach (MapTile tile in tiles)
		{
			if (tile.ID == id)
			{
                return true;
			}
		}

        return false;
    }

    public void Load(string file) {
		if (file.Length > 0)
		{
            string json = File.ReadAllText(file);
            if (json.Length > 0)
            {
                List<TileStruct> newTiles = LoadFromFile(json);


                foreach (TileStruct t in newTiles)
                {
                    MapTile tile = Instantiate(prefabs[t.name]).GetComponent<MapTile>();
                    tile.FromData(t);
                    AddTile(tile);
                }

                foreach (ActivatorObject ao in GetComponentsInChildren<ActivatorObject>())
                {
                    ao.Reconnect();
                }
            
			}

            EventManager.TriggerEvent("LevelLoaded");
		}
    }
}
