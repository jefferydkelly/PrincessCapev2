using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor {
    MapTile selectedPrefab;

    List<MapTile> selectedObjects;
    MapTile secondaryMapTile;

    Dictionary<string, GameObject> prefabs;

    Vector3 spawnPosition = Vector3.zero;
    Map map;
    SerializedObject serialMap;

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

        map.Init();
        serialMap = new SerializedObject(map);
        selectedObjects = new List<MapTile>();
    }

    /// <summary>
    /// Handles the disabling of the MapEditor
    /// </summary>
    private void OnDisable()
    {
        ClearMap();
    }

    /// <summary>
    /// Handles the destruction of the MapEditor
    /// </summary>
    private void OnDestroy()
    {
        
        ClearMap();
    }

    void ClearMap() {
        if (PrimaryMapTile)
        {
            PrimaryMapTile.HighlightState = MapHighlightState.Normal;
        }

        foreach (MapTile mt in selectedObjects)
        {
            mt.HighlightState = MapHighlightState.Normal;
        }
        if (SecondaryMapTile)
        {
            SecondaryMapTile.HighlightState = MapHighlightState.Normal;
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
        if (PrimaryMapTile) {
			ActivatedObject ao = PrimaryMapTile.GetComponent<ActivatedObject>();
            if (SecondaryMapTile)
            {
				ActivatedObject bo = SecondaryMapTile.GetComponent<ActivatedObject>();
				if (ao && bo && (ao is ActivatorObject || bo is ActivatorObject))
				{
					ActivatorObject activator = (ao is ActivatorObject ? ao : bo) as ActivatorObject;
					ActivatedObject activated = ao is ActivatorObject ? bo : ao;
                    string buttonMessage = (activator.HasConnection(activated) ? "Disconnect" : "Connect") + " (Space)";
					if (GUILayout.Button(buttonMessage))
					{
						Connect(activator, activated);
					}
				}
            }

           
            if (ao) {
                string butText = ao.StartsActive ? "Deactivate" : "Activate";
                if (GUILayout.Button(butText)) {
                    ao.StartsActive = !ao.StartsActive;
                }
            }
        }
        GUILayout.EndHorizontal();
		if (PrimaryMapTile && PrimaryMapTile is ActivatorObject) {
			ActivatorObject activatorObject = PrimaryMapTile as ActivatorObject;
			if (activatorObject.Connections.Count > 0)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Connections:");
				GUILayout.EndHorizontal();
				for (int i = 0; i < activatorObject.Connections.Count; i++)
				{
					ActivatorConnection akon = activatorObject.Connections[i];
					GUILayout.BeginHorizontal();
					GUILayout.Label(string.Format("{0}: {1}", akon.Activated.name, akon.Activated.ID));
					if (GUILayout.Button("Invert")) {
						akon.IsInverted = !akon.IsInverted;
					}
                    GUILayout.EndHorizontal();
				}

			}		
		}
        GUILayout.BeginHorizontal();
        if (prefabs != null)
        {
            int elementsInThisRow = 0;
            foreach(KeyValuePair<string, GameObject> pair in prefabs) {
                elementsInThisRow++;
                Texture prefabTexture = pair.Value.GetComponent<MapTile>().EditorTexture;

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

        if (GUILayout.Button("Reload")) {
            Reload();
            GUIUtility.ExitGUI();
        }
        if (GUILayout.Button("Load"))
        {
            Load();
            GUIUtility.ExitGUI();
        }

        if (GUILayout.Button("Clear")) {
            selectedObjects.Clear();
            SecondaryMapTile = null;
            map.Clear();
        }
        GUILayout.EndHorizontal();
        map.RenderInEditor();
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
                
                MapTile atLoc = map.GetObjectAtLocation(spawnPosition);
                if (Event.current.button == 0)
                {
                    if (atLoc != null)
                    {
                        if (atLoc == SecondaryMapTile)
                        {
                            if (!Event.current.shift)
                            {
                                SwapPrimaryAndSecondary();
                                foreach (MapTile mt in selectedObjects)
                                {
                                    mt.HighlightState = MapHighlightState.Normal;
                                }
                                selectedObjects.Clear();
                            } else {
                                secondaryMapTile.HighlightState = MapHighlightState.Primary;
                                selectedObjects.Add(secondaryMapTile);
                                secondaryMapTile = null;
                            }
                        }
                        else
                        {
                            if (!Event.current.shift)
                            {
                                PrimaryMapTile = atLoc;

                            } else if (atLoc) {
                                if (!selectedObjects.Contains(atLoc))
                                {
                                    atLoc.HighlightState = MapHighlightState.Backup;
                                    selectedObjects.Add(atLoc);
                                } else {
                                    atLoc.HighlightState = MapHighlightState.Normal;
                                    selectedObjects.Remove(atLoc);
                                    if (selectedObjects.Count > 0) {
                                        PrimaryMapTile = selectedObjects[0];
                                        for (int i = 1; i < selectedObjects.Count; i++) {
                                            selectedObjects[i].HighlightState = MapHighlightState.Backup;
                                        }
                                    }
                                }
                            }
                        }
                        Repaint();
                    } else {
                        Spawn(spawnPosition);
                        Event.current.Use();
                    }
                    Event.current.Use();
                } else if (Event.current.button == 1) {
                    
                    if (atLoc == PrimaryMapTile)
                    {
                        SwapPrimaryAndSecondary();
                    }
                    else
                    {
                        SecondaryMapTile = atLoc != secondaryMapTile ? atLoc : null;
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
            if (Event.current.keyCode == KeyCode.Delete || Event.current.keyCode == KeyCode.Backspace)
            {
                if (PrimaryMapTile)
                {
					
					map.RemoveTile(PrimaryMapTile);
					selectedObjects.Remove(PrimaryMapTile);
                    if (map.NumberOfTiles > 0)
                    {
                        PrimaryMapTile = map.GetTile(map.NumberOfTiles - 1);
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
            else if (PrimaryMapTile != null)
            {
                if (Event.current.keyCode == KeyCode.UpArrow) {
                    if (mode == MapEditMode.Translate) {
                        //PrimaryMapTile.Translate(new Vector3(0, 1, 0));
                        foreach(MapTile mt in selectedObjects) {
                            mt.Translate(new Vector3(0, 1, 0));
                        }
                        Event.current.Use();
                    } else if (mode == MapEditMode.Scale) {
                        //PrimaryMapTile.ScaleY(true);
                        foreach (MapTile mt in selectedObjects)
                        {
                            mt.ScaleY(true);
                        }
                        Event.current.Use();
                    } else if (mode == MapEditMode.Flip) {
                        //PrimaryMapTile.FlipY();

                        foreach (MapTile mt in selectedObjects)
                        {
                            mt.FlipY();
                        }
                        Event.current.Use();
                    } else if (mode == MapEditMode.Align) {
                        SpawnAligned(Direction.Up);
                        Event.current.Use();
                    }
                } else if (Event.current.keyCode == KeyCode.DownArrow) {
                    if (mode == MapEditMode.Translate)
                    {
                        //PrimaryMapTile.Translate(new Vector3(0, -1, 0));

                        foreach (MapTile mt in selectedObjects)
                        {
                            mt.Translate(new Vector3(0, -1, 0));
                        }
                        Event.current.Use();
                    }
                    else if (mode == MapEditMode.Scale)
                    {
                        //PrimaryMapTile.ScaleY(false);

                        foreach (MapTile mt in selectedObjects)
                        {
                            mt.ScaleY(false);
                        }
                        Event.current.Use();
                    }
                    else if (mode == MapEditMode.Flip)
                    {
                        //PrimaryMapTile.FlipY();

                        foreach (MapTile mt in selectedObjects)
                        {
                            mt.FlipY();
                        }

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
                        //PrimaryMapTile.Translate(new Vector3(1, 0, 0));

                        foreach (MapTile mt in selectedObjects)
                        {
                            mt.Translate(new Vector3(1, 0, 0));
                        }
                        Event.current.Use();
                    }
                    else if (mode == MapEditMode.Scale)
                    {
                        //PrimaryMapTile.ScaleX(true);

                        foreach (MapTile mt in selectedObjects)
                        {
                            mt.ScaleX(true);
                        }

                        Event.current.Use();
                    } else if (mode == MapEditMode.Rotate) {
                        //PrimaryMapTile.Rotate(-90);

                        foreach (MapTile mt in selectedObjects)
                        {
                            mt.Rotate(-90);
                        }
                        Event.current.Use();
                    }
                    else if (mode == MapEditMode.Flip)
                    {
                        //PrimaryMapTile.FlipX();

                        foreach (MapTile mt in selectedObjects)
                        {
                            mt.FlipX();
                        }
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
                        //PrimaryMapTile.Translate(new Vector3(-1, 0, 0));

                        foreach (MapTile mt in selectedObjects)
                        {
                            mt.Translate(new Vector3(-1, 0, 0));
                        }
                        Event.current.Use();
                    }
                    else if (mode == MapEditMode.Scale)
                    {
                        //PrimaryMapTile.ScaleX(false);

                        foreach (MapTile mt in selectedObjects)
                        {
                            mt.ScaleX(false);
                        }
                        Event.current.Use();
                    }
                    else if (mode == MapEditMode.Rotate)
                    {
                        //PrimaryMapTile.Rotate(90);

                        foreach (MapTile mt in selectedObjects)
                        {
                            mt.Rotate(90);
                        }

                        Event.current.Use();
                    }
                    else if (mode == MapEditMode.Flip)
                    {
                        //PrimaryMapTile.FlipX();

                        foreach (MapTile mt in selectedObjects)
                        {
                            mt.FlipX();
                        }
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

        if (PrimaryMapTile != null)
        {
            Handles.Label(PrimaryMapTile.transform.position, "X");
        }


        Handles.BeginGUI();
        GUILayout.Box("Map Edit Mode");
        if (selectedPrefab == null)
        {
            GUILayout.Box("No prefab selected!");
        }


        Handles.EndGUI();

        map.RenderInEditor();
    }

    /// <summary>
    /// Connect the selected map tile and the secondary map tile if they are an Activator and Activated object.
    /// </summary>
    void Connect() {
        if (PrimaryMapTile != null && secondaryMapTile != null)
        {
            ActivatedObject connected = null;
            ActivatorObject connector = PrimaryMapTile.GetComponent<ActivatorObject>();
            if (connector != null)
            {
                connected = secondaryMapTile.GetComponent<ActivatedObject>();
            }
            else
            {
                connected = PrimaryMapTile.GetComponent<ActivatedObject>();
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

    /// <summary>
    /// Connect the specified activator and activated.
    /// </summary>
    /// <param name="activator">Activator.</param>
    /// <param name="activated">Activated.</param>
	void Connect(ActivatorObject activator, ActivatedObject activated) {
		if (activator.HasConnection(activated)) {
			activator.RemoveConnection(activated);
		} else {
			activator.AddConnection(activated);
		}
	}

    /// <summary>
    /// Spawns an instance of the selected prefab aligned with the selected object.
    /// </summary>
    /// <param name="dir">Dir.</param>
    void SpawnAligned(Direction dir) {
        if (selectedPrefab != null && PrimaryMapTile != null)
        {
            SpriteRenderer spSpr = selectedPrefab.GetComponent<SpriteRenderer>();
            SpriteRenderer sgoSpr = PrimaryMapTile.GetComponent<SpriteRenderer>();
            float selectedGameObjectWidth = sgoSpr.bounds.size.x;
            float selectedGameObjectHeight = sgoSpr.bounds.size.y;
            float selectedPrefabWidth = spSpr.bounds.size.x;
            float selectedPrefabHeight = spSpr.bounds.size.y;

            switch (dir) {
                case Direction.Up:
                    spawnPosition = new Vector3(PrimaryMapTile.transform.position.x,
                                                PrimaryMapTile.transform.position.y + (selectedGameObjectHeight + selectedPrefabHeight) / 2.0f, 0);
                    break;
                case Direction.Down:
                    spawnPosition = new Vector3(PrimaryMapTile.transform.position.x,
                                                PrimaryMapTile.transform.position.y - (selectedGameObjectHeight + selectedPrefabHeight) / 2.0f, 0);
                    break;
                case Direction.Left:
                    spawnPosition = new Vector3(PrimaryMapTile.transform.position.x - (selectedGameObjectWidth + selectedPrefabWidth) / 2.0f,
                                            PrimaryMapTile.transform.position.y, 0);
                    break;
                case Direction.Right:
                    spawnPosition = new Vector3(PrimaryMapTile.transform.position.x + (selectedGameObjectWidth + selectedPrefabWidth) / 2.0f,
                                                PrimaryMapTile.transform.position.y, 0);
                    break;
            }

            Spawn(spawnPosition);
        }
    }

    /// <summary>
    /// Swaps the Primary and Secondary MapTiles.
    /// </summary>
	void SwapPrimaryAndSecondary() {
        MapTile temp = SecondaryMapTile;
        SecondaryMapTile = PrimaryMapTile;
        PrimaryMapTile = temp;
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
            Vector3 pos = new Vector3(Mathf.Round(_spawnPosition.x), Mathf.Round(_spawnPosition.y), selectedPrefab.ZPos);


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
                PrimaryMapTile = go.GetComponent<MapTile>();


                //pos.z = SelectedMapTile.ZPos;
                go.transform.position = pos;
                go.name = selectedPrefab.name;
                map.AddTile(PrimaryMapTile);
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
     
        for (int i = 0; i < map.NumberOfTiles; i++)
        {
            MapTile tile = map.GetTile(i);

            if (tile.Overlaps(selectedPrefab, spawnPosition)) {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Gets or sets the selected game object.
    /// </summary>
    /// <value>The selected game object.</value>
    MapTile PrimaryMapTile
    {

        set {

			foreach (MapTile mt in selectedObjects)
			{
				mt.HighlightState = MapHighlightState.Normal;
			}
			selectedObjects.Clear();
            if (value)
            {
                value.HighlightState = MapHighlightState.Primary;
                selectedObjects.Add(value);
            }
        }
        get
        {
            if (selectedObjects.Count > 0)
            {
                return selectedObjects[0];
            }

            return null;
        }
    }

    /// <summary>
    /// Gets or sets the secondary map tile.
    /// </summary>
    /// <value>The secondary map tile.</value>
    MapTile SecondaryMapTile {
        get {
            return secondaryMapTile;
        }

        set {
            if (secondaryMapTile && secondaryMapTile.HighlightState == MapHighlightState.Secondary) {
                secondaryMapTile.HighlightState = MapHighlightState.Normal;
            }

            secondaryMapTile = value;

            if (secondaryMapTile)
            {
                secondaryMapTile.HighlightState = MapHighlightState.Secondary;
            }
        }
    }

    /// <summary>
    /// Saves the current map to the given file location
    /// </summary>
    void Save() {
        string path = EditorUtility.SaveFilePanel("Save Level To File", "Assets/Resources/Levels", map.FileName, "json");

        if (path.Length > 0) {
            string json = map.SaveData;

            File.WriteAllText(path, json);
        }
    }

    /// <summary>
    /// Load the level at the given file location.
    /// </summary>
    void Load() {
        string path = EditorUtility.OpenFilePanel("Open A Level File", "Assets/Resources/Levels", "json");
        if (path.Length > 0)
        {
            
            map.Load(path);
            serialMap.FindProperty("levelName").stringValue = map.LevelName;
            serialMap.FindProperty("fileName").stringValue = map.FileName;
            serialMap.FindProperty("items").enumValueIndex = (int)map.Items;
			serialMap.FindProperty("mapID").intValue = map.MapID;
            serialMap.ApplyModifiedProperties();
            serialMap.Update();
        }
    }

    /// <summary>
    /// Reloads the map.
    /// </summary>
    void Reload() {
		selectedObjects.Clear();
		secondaryMapTile = null;
        map.Reload();
        serialMap.FindProperty("levelName").stringValue = map.LevelName;
        serialMap.FindProperty("fileName").stringValue = map.FileName;
        serialMap.FindProperty("items").enumValueIndex = (int)map.Items;
		serialMap.FindProperty("mapID").intValue = map.MapID;
        serialMap.ApplyModifiedProperties();
        serialMap.Update();
    }
}


