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
    [SerializeField]
    string fileName = "level.json";
    [SerializeField]
    ItemLevel items = ItemLevel.None;
    List<MapTile> tiles;
    Dictionary<string, GameObject> prefabs;
    public void Awake()
    {
        tiles = GetComponentsInChildren<MapTile>().ToList();
        Init();
        ClearHighlights();
    }

    void LoadPrefabs() {
		Object[] obj = Resources.LoadAll("Tiles", typeof(GameObject));

		prefabs = new Dictionary<string, GameObject>(obj.Length);

		for (int i = 0; i < obj.Length; i++)
		{
			GameObject go = (GameObject)obj[i];
			prefabs.Add(go.name, go);
		}
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
#endif
	public string SaveToFile() {
        string info = "{\n";
        info += PCLParser.CreateAttribute("MapName", levelName);
        info += PCLParser.CreateAttribute("Items", items);//string.Format("\"MapName\": \"{0}\"", levelName) + ",\n";
        info += PCLParser.CreateArray("Tiles");
        foreach(MapTile tile in tiles) {
            info += tile.SaveData();
        }
        info += "]\n}";
        return info;
    }


	public List<TileStruct> LoadFromFile(string json) {
        string[] lines = json.Split('\n');

        levelName = PCLParser.ParseLine(lines[1]);

        if (lines[2].Contains("Items")) {
            items = PCLParser.ParseEnum<ItemLevel>(lines[2]);
        } else {
            items = ItemLevel.None;
        }
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

        if (prefabs == null) {
            LoadPrefabs();
        }
        if (file.Length > 0)
        {
            Clear();

            fileName = file.Split('/').Last();
            string scenePath = "Levels/" + fileName.Substring(0, fileName.Length - 5);

            TextAsset text = Resources.Load<TextAsset>(scenePath);

            if (text)
            {
                string json = text.text;
                if (json.Length > 0)
                {
                    List<TileStruct> newTiles = LoadFromFile(json);


                    foreach (TileStruct t in newTiles)
                    {
                        MapTile tile = Instantiate(prefabs[t.name]).GetComponent<MapTile>();
                        tile.FromData(t);
                        tile.Init();
                        AddTile(tile);
                    }

                    foreach (ActivatorObject ao in GetComponentsInChildren<ActivatorObject>())
                    {
                        ao.Reconnect();
                    }

                    if (Application.isPlaying) {                       
                        EventManager.TriggerEvent("LevelLoaded");
                    }
                }


            }
        }
    }

    /// <summary>
    /// Gets the name of the level.
    /// </summary>
    /// <value>The name of the level.</value>
    public string LevelName {
        get {
            return levelName;
        }
    }

    /// <summary>
    /// Gets the level of items the player should have in this level.
    /// </summary>
    /// <value>The items.</value>
    public ItemLevel Items {
        get {
            return items;
        }
    }

    public string FileName {
        get {
            return fileName;
        }
    }

    public void Init() {
        foreach(MapTile mt in tiles) {
            mt.Init();
        }
    }
}

//Represents the last item the player received
public enum ItemLevel {
    None,
    MagicCape,
    StarShield,
    PullGauntlet,
    PushGauntlet
}
