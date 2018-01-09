using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor {
	GameObject[] prefabs;
	GameObject selectedPrefab;
    MapTile selectedGameObject;
	List<MapTile> spawnedGO = new List<MapTile>();
	bool makeDown = false;

	Vector3 spawnPosition = Vector3.zero;
	GameObject targetGameObject;

    string[] tools = { "Translate (Z)", "Rotate (X)", "Scale (C)", "Flip (V)", "Connect (B)" };
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
		if (SelectedGameObject)
		{
			SelectedGameObject.Highlighted = false;
		}
	}

	/// <summary>
	/// Handles the destruction of the MapEditor
	/// </summary>
	private void OnDestroy()
	{
		if (SelectedGameObject)
		{
			SelectedGameObject.Highlighted = false;
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
					selectedPrefab = prefabs[i];
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
		if (Event.current.isMouse)
		{
			spawnPosition = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
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
				if (SelectedGameObject)
				{
					//int ind = spawnedGO.IndexOf(selectedGameObject);
					spawnedGO.Remove(selectedGameObject);
					selectedGameObject.Delete();

					if (spawnedGO.Count > 0)
					{
						SelectedGameObject = spawnedGO[spawnedGO.Count - 1];
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
                mode = MapEditMode.Connect;
				Repaint();
				Event.current.Use();
            }
			else if (Event.current.keyCode == KeyCode.R)
			{
				SelectedGameObject = GetObjectAtLocation(spawnPosition);
				Event.current.Use();
			}
			else if (SelectedGameObject != null)
			{
                if (Event.current.keyCode == KeyCode.UpArrow) {
                    if (mode == MapEditMode.Translate) {
						SelectedGameObject.Translate(new Vector3(0, 1, 0));
						Event.current.Use();
                    } else if (mode == MapEditMode.Scale) {
						SelectedGameObject.ScaleY(true);
						Event.current.Use();
                    } else if (mode == MapEditMode.Flip) {
                        SelectedGameObject.FlipY();
                        Event.current.Use();
                    }
                } else if (Event.current.keyCode == KeyCode.DownArrow) {
					if (mode == MapEditMode.Translate)
					{
						SelectedGameObject.Translate(new Vector3(0, -1, 0));
						Event.current.Use();
					}
					else if (mode == MapEditMode.Scale)
					{
                        SelectedGameObject.ScaleY(false);
						Event.current.Use();
					}
					else if (mode == MapEditMode.Flip)
					{
						SelectedGameObject.FlipY();
						Event.current.Use();
					}
				}
                else if (Event.current.keyCode == KeyCode.RightArrow)
				{
					if (mode == MapEditMode.Translate)
					{
						SelectedGameObject.Translate(new Vector3(1, 0, 0));
						Event.current.Use();
					}
					else if (mode == MapEditMode.Scale)
					{
						SelectedGameObject.ScaleX(true);
						Event.current.Use();
                    } else if (mode == MapEditMode.Rotate) {
                        SelectedGameObject.Rotate(90);
                        Event.current.Use();
					}
					else if (mode == MapEditMode.Flip)
					{
						SelectedGameObject.FlipX();
						Event.current.Use();
					}
				}
				else if (Event.current.keyCode == KeyCode.LeftArrow)
				{
					if (mode == MapEditMode.Translate)
					{
						SelectedGameObject.Translate(new Vector3(-1, 0, 0));
						Event.current.Use();
					}
					else if (mode == MapEditMode.Scale)
					{
						SelectedGameObject.ScaleX(false);
						Event.current.Use();
					}
					else if (mode == MapEditMode.Rotate)
					{
						SelectedGameObject.Rotate(-90);
						Event.current.Use();
					}
					else if (mode == MapEditMode.Flip)
					{
						SelectedGameObject.FlipX();
						Event.current.Use();
					}
				}
				else if (Event.current.keyCode == KeyCode.T)
				{
					MapTile newsie = GetObjectAtLocation(spawnPosition);

					if (newsie)
					{
						ActivatedObject connected = null;
						ActivatorObject connector = selectedGameObject.GetComponent<ActivatorObject>();
						if (connector != null)
						{
							connected = newsie.GetComponent<ActivatedObject>();
						}
						else
						{
							connected = selectedGameObject.GetComponent<ActivatedObject>();
							connector = newsie.GetComponent<ActivatorObject>();
						}

						if (connector != null && connected != null)
						{
							connector.AddConnection(connected);
							Event.current.Use();
						}

					}
				}
				else if (Event.current.keyCode == KeyCode.Y)
				{
					MapTile newsie = GetObjectAtLocation(spawnPosition);

					if (newsie)
					{
						ActivatedObject connected = null;
						ActivatorObject connector = selectedGameObject.GetComponent<ActivatorObject>();
						if (connector != null)
						{
							connected = newsie.GetComponent<ActivatedObject>();
						}
						else
						{
							connected = selectedGameObject.GetComponent<ActivatedObject>();
							connector = newsie.GetComponent<ActivatorObject>();
						}

						if (connector != null && connected != null)
						{
							connector.RemoveConnection(connected);
							Event.current.Use();
						}

					}
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

		if (SelectedGameObject != null)
		{
			Handles.Label(SelectedGameObject.transform.position, "X");

			if (selectedPrefab != null)
			{
				SpriteRenderer spSpr = selectedPrefab.GetComponent<SpriteRenderer>();
				SpriteRenderer sgoSpr = SelectedGameObject.GetComponent<SpriteRenderer>();
				float selectedGameObjectWidth = sgoSpr.bounds.size.x;
				float selectedGameObjectHeight = sgoSpr.bounds.size.y;
				float selectedPrefabWidth = spSpr.bounds.size.x;
				float selectedPrefabHeight = spSpr.bounds.size.y;

				if (Event.current.type == EventType.KeyDown)
				{
					if (Event.current.keyCode == KeyCode.W)
					{
						spawnPosition = new Vector3(SelectedGameObject.transform.position.x,
													SelectedGameObject.transform.position.y + (selectedGameObjectHeight + selectedPrefabHeight) / 2.0f, 0);
						Spawn(spawnPosition);
					}
					else if (Event.current.keyCode == KeyCode.S)
					{
						spawnPosition = new Vector3(SelectedGameObject.transform.position.x,
													SelectedGameObject.transform.position.y - (selectedGameObjectHeight + selectedPrefabHeight) / 2.0f, 0);
						Spawn(spawnPosition);
					}
					else if (Event.current.keyCode == KeyCode.A)
					{
						spawnPosition = new Vector3(SelectedGameObject.transform.position.x - (selectedGameObjectWidth + selectedPrefabWidth) / 2.0f,
												SelectedGameObject.transform.position.y, 0);
						Spawn(spawnPosition);
					}
					else if (Event.current.keyCode == KeyCode.D)
					{
						spawnPosition = new Vector3(SelectedGameObject.transform.position.x + (selectedGameObjectWidth + selectedPrefabWidth) / 2.0f,
													SelectedGameObject.transform.position.y, 0);
						Spawn(spawnPosition);
					}
					Event.current.Use();
				}

			}
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

	/// <summary>
	/// Spawns the selected prefab at the specified spawnPosition.
	/// </summary>
	/// <returns>The spawn.</returns>
	/// <param name="_spawnPosition">Spawn position.</param>
	void Spawn(Vector2 _spawnPosition)
	{
		Vector3 pos = new Vector3(Mathf.Round(_spawnPosition.x), Mathf.Round(_spawnPosition.y), 0);
		if (IsSpawnPositionOpen(pos))
		{
			GameObject go = (GameObject)Instantiate(selectedPrefab);
			SelectedGameObject = go.GetComponent<MapTile>();
			pos.z = SelectedGameObject.ZPos;
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
		foreach (MapTile goat in spawnedGO)
		{
			Vector3 dif = spawnPos - goat.transform.position;
			Vector3 bounds = goat.Bounds / 2;
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
	MapTile SelectedGameObject
	{
		set
		{
			if (selectedGameObject)
			{
				selectedGameObject.Highlighted = false;
			}

			selectedGameObject = value;
            if (selectedGameObject)
            {
                selectedGameObject.Highlighted = true;
            }
		}

		get
		{
			return selectedGameObject;
		}
	}


}

public enum MapEditMode {
    Translate,
    Rotate,
    Scale,
    Flip,
    Connect
}
