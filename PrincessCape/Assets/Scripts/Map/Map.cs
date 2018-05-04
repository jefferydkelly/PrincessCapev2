using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class Map : MonoBehaviour
{
    [SerializeField]
    string levelName;
    [SerializeField]
    string fileName;
    [SerializeField]
    ItemLevel items = ItemLevel.None;
    [SerializeField]
    bool showInLevelSelect = true;

    List<MapTile> tiles;
    Dictionary<string, GameObject> prefabs;

    UnityEvent onLevelLoaded;
    static Map instance;
    public void Awake()
    {
        Init();
    }

    /// <summary>
    /// Loads the Tile prefabs from the appropriate folder
    /// </summary>
    void LoadPrefabs() {
		Object[] obj = Resources.LoadAll("Tiles", typeof(GameObject));

		prefabs = new Dictionary<string, GameObject>(obj.Length);

		for (int i = 0; i < obj.Length; i++)
		{
			GameObject go = (GameObject)obj[i];
			prefabs.Add(go.name, go);
		}
	}

    /// <summary>
    /// Assigns unique IDs to every tile.
    /// </summary>
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

    /// <summary>
    /// Clear the map of all tiles.
    /// </summary>
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
                if (tile)
                {
                    DestroyImmediate(tile.gameObject, false);
                }
            }
        }

        tiles = new List<MapTile>();

    }
    /// <summary>
    /// Sets the highlight state of each tile to normal.
    /// </summary>
    public void ClearHighlights()
    {
       
        foreach (MapTile tile in tiles)
        {
            tile.HighlightState = MapHighlightState.Normal;
        }
    }

    /// <summary>
    /// Adds a tile to the map and assigns it an ID.
    /// </summary>
    /// <param name="tile">Tile.</param>
    public void AddTile(MapTile tile)
    {
        tile.transform.SetParent(transform);

        tiles.Add(tile);

        if (tile.ID < 0)
        {
            tile.ID = NumberOfTiles;
        }
    }

    /// <summary>
    /// Removes the tile from the map.
    /// </summary>
    /// <param name="tile">Tile.</param>
    public void RemoveTile(MapTile tile)
    {
        tiles.Remove(tile);
#if UNITY_EDITOR
        tile.Delete();
#else
        Destroy(tile.gameObject);
#endif
    }

    /// <summary>
    /// Gets the number of tiles in the map.
    /// </summary>
    /// <value>The number of tiles in the map.</value>
    public int NumberOfTiles
    {
        get
        {
            return tiles.Count;
        }
    }

    /// <summary>
    /// Gets the tile according to its place in the array.
    /// </summary>
    /// <returns>The tile located at tileNum or null if tileNum is greater than the number of tiles in the array.</returns>
    /// <param name="tileNum">Tile number.</param>
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
    /// <summary>
    /// Renders the in editor.
    /// </summary>
    public void RenderInEditor() {
		foreach (MapTile tile in tiles)
		{
			tile.RenderInEditor();
		}
    }
#endif
	/// <summary>
	/// Converts the map into a json-ish file string for saving to file
	/// </summary>
	/// <returns> A json-ish file string.</returns>
	public string SaveToFile() {
        string info = "{\n";
        info += PCLParser.CreateAttribute("MapName", levelName);
        info += PCLParser.CreateAttribute("Show In Level Select", showInLevelSelect);
        info += PCLParser.CreateAttribute("Items", items);
        info += PCLParser.CreateArray("Tiles");
        foreach(MapTile tile in tiles) {
            info += tile.SaveData();
        }
        info += "]\n}";
        return info;
    }

	/// <summary>
	/// Creates a List of Tile Struct information based on the information in the string passed in
	/// </summary>
	/// <returns> List of Tile Struct information based on the json passed in.</returns>
	/// <param name="json">Json.</param>
	public List<TileStruct> LoadFromFile(string json) {
        string[] lines = json.Split('\n');

        levelName = PCLParser.ParseLine(lines[1]);
        showInLevelSelect = PCLParser.ParseBool(lines[2]);
        items = PCLParser.ParseEnum<ItemLevel>(lines[3]);
        int ind = json.IndexOf(',');
        string tileData = json.Substring(ind);
        return PCLParser.ParseTiles(tileData);
    }

    /// <summary>
    /// Gets the tile by its ID number.
    /// </summary>
    /// <returns>The tile with the given ID if there is one.</returns>
    /// <param name="id">Identifier.</param>
    public MapTile GetTileByID(int id) {
        foreach(MapTile tile in tiles) {
            if (tile.ID == id) {
                return tile;
            }
        }

        return null;
    }

    /// <summary>
    /// Whether or not a tile has the given identifier.
    /// </summary>
    /// <returns><c>true</c>, if has a tile has the ID, <c>false</c> otherwise.</returns>
    /// <param name="id">Identifier.</param>
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

    /// <summary>
    /// Load the specified file.
    /// </summary>
    /// <returns>The load.</returns>
    /// <param name="file">File.</param>
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
                        tile.name = tile.name.Replace("(Clone)", "");
                        tile.FromData(t);
                        tile.Init();
                        AddTile(tile);
                    }

                    foreach (ActivatorObject ao in GetComponentsInChildren<ActivatorObject>())
                    {
                        ao.Reconnect();
                    }

                    if (Application.isPlaying) {                       
                        OnLevelLoaded.Invoke();
                    }
                }


            }
        }
    }

    /// <summary>
    /// Reload the level.
    /// </summary>
    public void Reload() {
        Load(fileName);
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

    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    /// <value>The name of the file.</value>
    public string FileName {
        get {
            return fileName;
        }
    }

    /// <summary>
    /// Initializes the Map and all of the tiles
    /// </summary>
    public void Init() {
       
        transform.position = Vector3.zero;
        tiles = GetComponentsInChildren<MapTile>().ToList();
		foreach(MapTile mt in tiles) {
            mt.Init();
        }

        ClearHighlights();

        AssignIDs();
        onLevelLoaded = new UnityEvent();
        instance = this;
    }

    /// <summary>
    /// Gets the on level loaded event.
    /// </summary>
    /// <value>The on level loaded event.</value>
    public UnityEvent OnLevelLoaded {
        get {
            return onLevelLoaded;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Map"/> show in level select.
    /// </summary>
    /// <value><c>true</c> if show in level select; otherwise, <c>false</c>.</value>
    public bool ShowInLevelSelect {
        get {
            return showInLevelSelect;
        }
    }

    public GameObject GetPrefabByName(string tileName) {
        GameObject prefab = null;
        prefabs.TryGetValue(tileName, out prefab);
        return prefab;
    }

    public static Map Instance {
        get {
            return instance;
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
