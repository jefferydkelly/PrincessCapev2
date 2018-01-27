using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor {
    MapTile selectedPrefab;
    MapTile selectedMapTile;
    MapTile secondaryMapTile;
    List<MapTile> tiles = new List<MapTile>();
    Dictionary<string, GameObject> prefabs;

	Vector3 spawnPosition = Vector3.zero;
    Map map;

    string[] tools = { "Translate (Z)", "Rotate (X)", "Scale (C)", "Flip (V)", "Align (B)" };
    MapEditMode mode = MapEditMode.Translate;

	/// <summary>
	/// Handles the initialization of the MapEditor
	/// </summary>
	private void OnEnable()
	{
        map = target as Map;
		
		Object[] obj = Resources.LoadAll("Tiles", typeof(GameObject));
		
        prefabs = new Dictionary<string, GameObject>(obj.Length);

		for (int i = 0; i < obj.Length; i++)
		{
            GameObject go = (GameObject)obj[i];
            prefabs.Add(go.name, go);
		}
        tiles = map.GetComponentsInChildren<MapTile>().ToList();
        foreach (MapTile et in tiles)
		{
			et.Highlighted = false;
		}
	}

	/// <summary>
	/// Handles the disabling of the MapEditor
	/// </summary>
	private void OnDisable()
	{
		if (SelectedMapTile)
		{
			SelectedMapTile.Highlighted = false;
		}
	}

	/// <summary>
	/// Handles the destruction of the MapEditor
	/// </summary>
	private void OnDestroy()
	{
		if (SelectedMapTile)
		{
			SelectedMapTile.Highlighted = false;
		}
	}

	/// <summary>
	/// Draws the Buttons in the MapEditor and Handles the determination of the selected prefab
	/// </summary>
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
        GUILayout.BeginHorizontal();
        mode = (MapEditMode)GUILayout.Toolbar((int)mode, tools);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (SelectedMapTile && SecondaryMapTile) {
            if (GUILayout.Button("Connect")) {
                Connect();
            }
        }
        GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (prefabs != null)
		{
			int elementsInThisRow = 0;
            foreach(KeyValuePair<string, GameObject> pair in prefabs) {
				elementsInThisRow++;
                Texture prefabTexture = pair.Value.GetComponent<SpriteRenderer>().sprite.texture;

				if (GUILayout.Button(prefabTexture, GUILayout.MaxWidth(50), GUILayout.MaxHeight(50)))
				{
                    selectedPrefab = pair.Value.GetComponent<MapTile>();
					EditorWindow.FocusWindowIfItsOpen(typeof(SceneView));
				}


				if (elementsInThisRow > Screen.width / 128)
				{
					elementsInThisRow = 0;
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
				}

			}
		}

		GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
		if (GUILayout.Button("Save"))
		{
            Save();
            GUIUtility.ExitGUI();
		}

		if (GUILayout.Button("Load"))
		{
			Load();
			GUIUtility.ExitGUI();
		}
		GUILayout.EndHorizontal();
	}

	/// <summary>
	/// Handles events in the MapEditor and spawns, deletes, connects, manipulates objects according to user input
	/// </summary>
	private void OnSceneGUI()
	{
      
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
		if (Event.current.isMouse)
		{
			spawnPosition = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;

            if (Event.current.type == EventType.MouseDown) {
                MapTile atLoc = GetObjectAtLocation(spawnPosition);
                if (Event.current.button == 0)
                {
                    if (atLoc != null)
                    {
                        if (atLoc == SecondaryMapTile)
                        {
                            SwapSelectedAndSecondary();
                        }
                        else
                        {
                            SelectedMapTile = atLoc != selectedMapTile ? atLoc : null;
                        }
                        Repaint();
                    } else {
                        Spawn(spawnPosition);
                        Event.current.Use();
                    }
                    Event.current.Use();
                } else if (Event.current.button == 1) {
                    
                    if (atLoc == SelectedMapTile)
                    {
                        SwapSelectedAndSecondary();
                    }
                    else
                    {
                        SecondaryMapTile = atLoc;
                    }
                    Repaint();

                }
			}
			else if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
			{

				Spawn(spawnPosition);
				Event.current.Use();
			}
		}

        if (Event.current.type == EventType.KeyDown)
		{
            /*
			if (Event.current.keyCode == KeyCode.E && !makeDown)
			{
				makeDown = true;
				Event.current.Use();
			}
			else*/ if (Event.current.keyCode == KeyCode.Delete || Event.current.keyCode == KeyCode.Backspace)
			{
				if (SelectedMapTile)
				{
					tiles.Remove(selectedMapTile);
					selectedMapTile.Delete();

					if (tiles.Count > 0)
					{
						SelectedMapTile = tiles[tiles.Count - 1];
					}
				}

				Event.current.Use();
			}
            else if (Event.current.keyCode == KeyCode.Z) {
                
                mode = MapEditMode.Translate;
                Repaint();
                Event.current.Use();
            } else if (Event.current.keyCode == KeyCode.X) {
                mode = MapEditMode.Rotate;
                Repaint();
                Event.current.Use();
            } else if (Event.current.keyCode == KeyCode.C) {
                mode = MapEditMode.Scale;
                Repaint();
                Event.current.Use();
            } else if (Event.current.keyCode == KeyCode.V) {
                mode = MapEditMode.Flip;
                Repaint();
                Event.current.Use();
            } else if (Event.current.keyCode == KeyCode.B) {
                mode = MapEditMode.Align;
				Repaint();
				Event.current.Use();
            }
			else if (SelectedMapTile != null)
			{
                if (Event.current.keyCode == KeyCode.UpArrow) {
                    if (mode == MapEditMode.Translate) {
						SelectedMapTile.Translate(new Vector3(0, 1, 0));
						Event.current.Use();
                    } else if (mode == MapEditMode.Scale) {
						SelectedMapTile.ScaleY(true);
						Event.current.Use();
                    } else if (mode == MapEditMode.Flip) {
                        SelectedMapTile.FlipY();
                        Event.current.Use();
                    } else if (mode == MapEditMode.Align) {
                        SpawnAligned(Direction.Up);
                        Event.current.Use();
                    }
                } else if (Event.current.keyCode == KeyCode.DownArrow) {
					if (mode == MapEditMode.Translate)
					{
						SelectedMapTile.Translate(new Vector3(0, -1, 0));
						Event.current.Use();
					}
					else if (mode == MapEditMode.Scale)
					{
                        SelectedMapTile.ScaleY(false);
						Event.current.Use();
					}
					else if (mode == MapEditMode.Flip)
					{
						SelectedMapTile.FlipY();
						Event.current.Use();
					}
					else if (mode == MapEditMode.Align)
					{
                        SpawnAligned(Direction.Down);
                        Event.current.Use();
					}
				}
                else if (Event.current.keyCode == KeyCode.RightArrow)
				{
					if (mode == MapEditMode.Translate)
					{
						SelectedMapTile.Translate(new Vector3(1, 0, 0));
						Event.current.Use();
					}
					else if (mode == MapEditMode.Scale)
					{
						SelectedMapTile.ScaleX(true);
						Event.current.Use();
                    } else if (mode == MapEditMode.Rotate) {
                        SelectedMapTile.Rotate(90);
                        Event.current.Use();
					}
					else if (mode == MapEditMode.Flip)
					{
						SelectedMapTile.FlipX();
						Event.current.Use();
					}
					else if (mode == MapEditMode.Align)
					{
                        SpawnAligned(Direction.Right);
                        Event.current.Use();
					}
				}
				else if (Event.current.keyCode == KeyCode.LeftArrow)
				{
					if (mode == MapEditMode.Translate)
					{
						SelectedMapTile.Translate(new Vector3(-1, 0, 0));
						Event.current.Use();
					}
					else if (mode == MapEditMode.Scale)
					{
						SelectedMapTile.ScaleX(false);
						Event.current.Use();
					}
					else if (mode == MapEditMode.Rotate)
					{
						SelectedMapTile.Rotate(-90);
						Event.current.Use();
					}
					else if (mode == MapEditMode.Flip)
					{
						SelectedMapTile.FlipX();
						Event.current.Use();
					}
					else if (mode == MapEditMode.Align)
					{
                        SpawnAligned(Direction.Left);
                        Event.current.Use();
					}
                } else if (Event.current.keyCode == KeyCode.Space) {
                    Connect();
                }
			}


		}

		if (SelectedMapTile != null)
		{
			Handles.Label(SelectedMapTile.transform.position, "X");
		}


		Handles.BeginGUI();
		GUILayout.Box("Map Edit Mode");
		if (selectedPrefab == null)
		{
			GUILayout.Box("No prefab selected!");
		}


		Handles.EndGUI();

		foreach (MapTile et in tiles)
		{
			et.RenderInEditor();
		}
	}

    void Connect() {
        if (selectedMapTile != null && secondaryMapTile != null)
        {
            ActivatedObject connected = null;
            ActivatorObject connector = selectedMapTile.GetComponent<ActivatorObject>();
            if (connector != null)
            {
                connected = secondaryMapTile.GetComponent<ActivatedObject>();
            }
            else
            {
                connected = selectedMapTile.GetComponent<ActivatedObject>();
                connector = secondaryMapTile.GetComponent<ActivatorObject>();
            }

            if (connector != null && connected != null)
            {
                if (connector.HasConnection(connected))
                {
                    connector.RemoveConnection(connected);
                } else {
                    connector.AddConnection(connected);
                }
                Event.current.Use();
            }
        }

    }
    void SpawnAligned(Direction dir) {
        if (selectedPrefab != null && selectedMapTile != null)
		{
			SpriteRenderer spSpr = selectedPrefab.GetComponent<SpriteRenderer>();
			SpriteRenderer sgoSpr = SelectedMapTile.GetComponent<SpriteRenderer>();
			float selectedGameObjectWidth = sgoSpr.bounds.size.x;
			float selectedGameObjectHeight = sgoSpr.bounds.size.y;
			float selectedPrefabWidth = spSpr.bounds.size.x;
			float selectedPrefabHeight = spSpr.bounds.size.y;

            switch (dir) {
                case Direction.Up:
					spawnPosition = new Vector3(SelectedMapTile.transform.position.x,
												SelectedMapTile.transform.position.y + (selectedGameObjectHeight + selectedPrefabHeight) / 2.0f, 0);
                    break;
                case Direction.Down:
					spawnPosition = new Vector3(SelectedMapTile.transform.position.x,
												SelectedMapTile.transform.position.y - (selectedGameObjectHeight + selectedPrefabHeight) / 2.0f, 0);
                    break;
                case Direction.Left:
					spawnPosition = new Vector3(SelectedMapTile.transform.position.x - (selectedGameObjectWidth + selectedPrefabWidth) / 2.0f,
											SelectedMapTile.transform.position.y, 0);
                    break;
                case Direction.Right:
					spawnPosition = new Vector3(SelectedMapTile.transform.position.x + (selectedGameObjectWidth + selectedPrefabWidth) / 2.0f,
												SelectedMapTile.transform.position.y, 0);
                    break;
            }

            Spawn(spawnPosition);
		}
    }

    void SwapSelectedAndSecondary() {
        MapTile temp = SecondaryMapTile;
        SecondaryMapTile = SelectedMapTile;
        SelectedMapTile = temp;
    }

	/// <summary>
	/// Spawns the selected prefab at the specified spawnPosition.
	/// </summary>
	/// <returns>The spawn.</returns>
	/// <param name="_spawnPosition">Spawn position.</param>
	void Spawn(Vector2 _spawnPosition)
	{
        if (selectedPrefab != null)
        {
            Vector3 pos = new Vector3(Mathf.Round(_spawnPosition.x), Mathf.Round(_spawnPosition.y), 0);


            if ((int)selectedPrefab.Bounds.x % 2 == 0)
            {
                pos.x += 0.5f;

                if (!IsSpawnPositionOpen(pos))
                {
                    pos.x -= 1.0f;
                }
            }

            if ((int)selectedPrefab.Bounds.y % 2 == 0)
            {
                pos.y += 0.5f;

                if (!IsSpawnPositionOpen(pos))
                {
                    pos.y -= 1.0f;
                }
            }

            if (IsSpawnPositionOpen(pos))
            {
                GameObject go = (GameObject)Instantiate(selectedPrefab.gameObject);
                SelectedMapTile = go.GetComponent<MapTile>();


                pos.z = SelectedMapTile.ZPos;
                go.transform.position = pos;
                go.name = selectedPrefab.name;
                go.transform.SetParent(map.transform);
                tiles.Add(go.GetComponent<MapTile>());
            }
        }
	}

	/// <summary>
	/// Determines whether or not an object can be spawned at the given position
	/// </summary>
	/// <returns><c>true</c>, if spawn position open was ised, <c>false</c> otherwise.</returns>
	/// <param name="spawnPos">Spawn position.</param>
	bool IsSpawnPositionOpen(Vector3 spawnPos)
	{
        if (selectedPrefab == null) {
            return false;
        }
        Vector3 prefabBounds = selectedPrefab.Bounds / 2;
		foreach (MapTile goat in tiles)
		{
			Vector3 dif = spawnPos - goat.transform.position;
			Vector3 bounds = prefabBounds + (goat.Bounds / 2);
			if (dif.x.BetweenEx(-bounds.x, bounds.x) && dif.y.BetweenEx(-bounds.y, bounds.y))
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Gets the object (if any) at location.
	/// </summary>
	/// <returns>The object at location.</returns>
	/// <param name="spawnPos">Spawn position.</param>
	MapTile GetObjectAtLocation(Vector3 spawnPos)
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

	/// <summary>
	/// Gets or sets the selected game object.
	/// </summary>
	/// <value>The selected game object.</value>
    MapTile SelectedMapTile
	{
		set
		{
            if (selectedMapTile && selectedMapTile.HighlightState == MapHighlightState.Primary)
			{
                selectedMapTile.HighlightState = MapHighlightState.Normal;
			}

			selectedMapTile = value;
            if (selectedMapTile)
            {
                selectedMapTile.HighlightState = MapHighlightState.Primary;
            }
		}

		get
		{
			return selectedMapTile;
		}
	}

    MapTile SecondaryMapTile {
        get {
            return secondaryMapTile;
        }

        set {
            if (secondaryMapTile && selectedMapTile.HighlightState == MapHighlightState.Secondary) {
                secondaryMapTile.HighlightState = MapHighlightState.Normal;
            }

            secondaryMapTile = value;

			if (secondaryMapTile)
			{
                secondaryMapTile.HighlightState = MapHighlightState.Secondary;
			}
        }
    }

    void Save() {
        string path = EditorUtility.SaveFilePanel("Save Level To File", "", "level.pcl", "pcl");

        if (path.Length > 0) {
            string json = map.SaveToFile();

            File.WriteAllText(path, json);
            Debug.Log("Saved");
		}
    }

    void Load() {
        string path = EditorUtility.OpenFilePanel("Open A Level File", "", "pcl");
        if (path.Length > 0)
        {
            string json = File.ReadAllText(path);
            if (json.Length > 0) {
                List<TileStruct> newTiles = map.LoadFromFile(json);

                if (newTiles.Count > 0) {
                    while(map.transform.childCount > 0) {
                        DestroyImmediate(map.transform.GetChild(0).gameObject, false);
                    }

                    foreach(TileStruct t in newTiles) {
                        MapTile tile = Instantiate(prefabs[t.name]).GetComponent<MapTile>();
                        tile.transform.SetParent(map.transform);
                        tile.FromData(t);

                    }
                }
            }
        }
    }

}

public enum MapEditMode {
    Translate,
    Rotate,
    Scale,
    Flip,
    Align
}

public enum Direction {
    Up,
    Left,
    Down,
    Right
}
