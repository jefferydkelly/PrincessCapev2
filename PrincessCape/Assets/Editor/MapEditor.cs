using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor {
	GameObject[] prefabs;
    MapTile selectedPrefab;
    MapTile selectedMapTile;
    MapTile secondaryMapTile;
	List<MapTile> spawnedGO = new List<MapTile>();
	bool makeDown = false;

	Vector3 spawnPosition = Vector3.zero;
	GameObject targetGameObject;

    string[] tools = { "Translate (Z)", "Rotate (X)", "Scale (C)", "Flip (V)", "Duplicate (B)" };
    MapEditMode mode = MapEditMode.Translate;

	/// <summary>
	/// Handles the initialization of the MapEditor
	/// </summary>
	private void OnEnable()
	{
		targetGameObject = (target as Map).gameObject;
		Object[] obj = Resources.LoadAll("Tiles", typeof(GameObject));
		prefabs = new GameObject[obj.Length];

		for (int i = 0; i < obj.Length; i++)
		{
			prefabs[i] = (GameObject)obj[i];
		}
		spawnedGO = targetGameObject.GetComponentsInChildren<MapTile>().ToList();
        foreach (MapTile et in spawnedGO)
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
		if (prefabs != null)
		{
			int elementsInThisRow = 0;
			for (int i = 0; i < prefabs.Length; i++)
			{
				elementsInThisRow++;
				Texture prefabTexture = prefabs[i].GetComponent<SpriteRenderer>().sprite.texture;

				if (GUILayout.Button(prefabTexture, GUILayout.MaxWidth(50), GUILayout.MaxHeight(50)))
				{
                    selectedPrefab = prefabs[i].GetComponent<MapTile>();
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
                        if (atLoc == SecondaryMapTile) {
                            SecondaryMapTile = SelectedMapTile;
                        }
                        SelectedMapTile = atLoc != selectedMapTile ? atLoc : null;
                    } else {
                        //Probably spawn something
                    }
                    Event.current.Use();
                } else if (Event.current.button == 1) {
                    if (atLoc != null) {
                        if (atLoc == SelectedMapTile) {
                            SelectedMapTile = SecondaryMapTile;
                        }
                        SecondaryMapTile = atLoc != secondaryMapTile ? atLoc : null; ;
                    }
                }
            }
		}

        if (Event.current.type == EventType.KeyDown)
		{
			if (Event.current.keyCode == KeyCode.E && !makeDown)
			{
				makeDown = true;
				Event.current.Use();
			}
			else if (Event.current.keyCode == KeyCode.Delete || Event.current.keyCode == KeyCode.Backspace)
			{
				if (SelectedMapTile)
				{
					spawnedGO.Remove(selectedMapTile);
					selectedMapTile.Delete();

					if (spawnedGO.Count > 0)
					{
						SelectedMapTile = spawnedGO[spawnedGO.Count - 1];
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
                mode = MapEditMode.Duplicate;
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
                    } else if (mode == MapEditMode.Duplicate) {
                        Duplicate(Direction.Up);
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
					else if (mode == MapEditMode.Duplicate)
					{
                        Duplicate(Direction.Down);
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
					else if (mode == MapEditMode.Duplicate)
					{
                        Duplicate(Direction.Right);
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
					else if (mode == MapEditMode.Duplicate)
					{
                        Duplicate(Direction.Left);
                        Event.current.Use();
					}
                } else if (Event.current.keyCode == KeyCode.Space) {
                    Connect();
                }
			}


		}
		else if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.E && makeDown)
		{
			makeDown = false;
			Event.current.Use();
		}
		else if (Event.current.type == EventType.MouseMove && makeDown)
		{
			Spawn(spawnPosition);
			Event.current.Use();
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

		foreach (MapTile et in spawnedGO)
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
    void Duplicate(Direction dir) {
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

	/// <summary>
	/// Spawns the selected prefab at the specified spawnPosition.
	/// </summary>
	/// <returns>The spawn.</returns>
	/// <param name="_spawnPosition">Spawn position.</param>
	void Spawn(Vector2 _spawnPosition)
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
			go.transform.SetParent(targetGameObject.transform);
			spawnedGO.Add(go.GetComponent<MapTile>());
		}
	}

	/// <summary>
	/// Determines whether or not an object can be spawned at the given position
	/// </summary>
	/// <returns><c>true</c>, if spawn position open was ised, <c>false</c> otherwise.</returns>
	/// <param name="spawnPos">Spawn position.</param>
	bool IsSpawnPositionOpen(Vector3 spawnPos)
	{
        Vector3 prefabBounds = selectedPrefab.Bounds / 2;
		foreach (MapTile goat in spawnedGO)
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

		foreach (MapTile goat in spawnedGO)
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
			if (selectedMapTile)
			{
				selectedMapTile.Highlighted = false;
			}

			selectedMapTile = value;
            if (selectedMapTile)
            {
                selectedMapTile.Highlighted = true;
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
            if (secondaryMapTile) {
                secondaryMapTile.Highlighted = false;
            }

            secondaryMapTile = value;

			if (secondaryMapTile)
			{
				secondaryMapTile.Highlighted = true;
			}
        }
    }


}

public enum MapEditMode {
    Translate,
    Rotate,
    Scale,
    Flip,
    Duplicate
}

public enum Direction {
    Up,
    Left,
    Down,
    Right
}
