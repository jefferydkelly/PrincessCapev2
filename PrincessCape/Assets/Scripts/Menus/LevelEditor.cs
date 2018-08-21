using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using UnityEngine.Events;

public class LevelEditor : MonoBehaviour {
    
    [SerializeField]
    GameObject levelBrowser;
    [SerializeField]
    ToolsUI toolsUI;
    [SerializeField]
    GameObject playbackUI;
    [SerializeField]
    TileBrowser tileBrowser;
    [SerializeField]
    LevelSaver saveUI;

    [SerializeField]
    Button activateButton;
    [SerializeField]
    Button connectButton;
    [SerializeField]
    Button invertButton;

    [SerializeField]
    Button saveButton;
    [SerializeField]
    Button loadButton;
    [SerializeField]
    GameObject connectionPrefab;

    List<ConnectionLine> connectionLines;
    List<MapTile> selectedObjects;
    MapTile secondaryMapTile;
    MapEditMode mode = MapEditMode.None;



    float cameraSpeed = 2.0f;

    //MapTile Events
    MapTileEvent onTileMoved = new MapTileEvent();
    MapTileEvent onTileDestroyed = new MapTileEvent();
    ConnectionEvent onConnectionRemoved = new ConnectionEvent();
    ConnectionEvent onConnectionInverted = new ConnectionEvent();
    static LevelEditor instance;
	// Use this for initialization
	void Start () {
        instance = this;
        connectionLines = new List<ConnectionLine>();

        Game.Instance.OnGameStateChanged.AddListener((GameState state) => {
            HideConnections = state != GameState.Paused;
        });
       

        Map.Instance.Init();

       
        activateButton.gameObject.SetActive(false);
        connectButton.gameObject.SetActive(false);
        invertButton.gameObject.SetActive(false);


        levelBrowser.SetActive(false);
        saveUI.gameObject.SetActive(false);

        selectedObjects = new List<MapTile>();

	}
	
	// Update is called once per frame
	void Update () {
        if (!Game.Instance.IsPlaying && mode != MapEditMode.Save && mode != MapEditMode.Load)
        {
            List<ConnectionLine> toBeDeleted = new List<ConnectionLine>();
            foreach(ConnectionLine cl in connectionLines) {
                if (cl.ToBeDeleted) {
                    toBeDeleted.Add(cl);
                }
            }

            foreach (ConnectionLine cl in toBeDeleted)
            {
                connectionLines.Remove(cl);
                Destroy(cl.gameObject);
            }

            ProcessInput();
            Vector3 mousePos = Controller.Instance.MousePosition;
            Vector3 pos = new Vector3(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y));

            MapTile atLoc = Map.Instance.GetObjectAtLocation(pos);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
            if (Input.GetKeyDown(KeyCode.Mouse0) && screenPos.y < tileBrowser.transform.position.y)
            {
                if (atLoc != null)
                {
                    if (atLoc == SecondaryMapTile)
                    {
                        if (!IsShiftDown)
                        {
                            SwapPrimaryAndSecondary();
                            foreach (MapTile mt in selectedObjects)
                            {
                                mt.HighlightState = MapHighlightState.Normal;
                            }
                            selectedObjects.Clear();
                        }
                        else
                        {
                            secondaryMapTile.HighlightState = MapHighlightState.Primary;
                            selectedObjects.Add(secondaryMapTile);
                            secondaryMapTile = null;
                        }
                    }
                    else
                    {
                        if (!IsShiftDown)
                        {
                            PrimaryMapTile = atLoc;

                        }
                        else if (atLoc)
                        {
                            if (!selectedObjects.Contains(atLoc))
                            {
                                atLoc.HighlightState = MapHighlightState.Backup;
                                selectedObjects.Add(atLoc);
                            }
                            else
                            {
                                atLoc.HighlightState = MapHighlightState.Normal;
                                selectedObjects.Remove(atLoc);
                                if (selectedObjects.Count > 0)
                                {
                                    PrimaryMapTile = selectedObjects[0];
                                    for (int i = 1; i < selectedObjects.Count; i++)
                                    {
                                        selectedObjects[i].HighlightState = MapHighlightState.Backup;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (tileBrowser.IsTileSelected)
                {
                    Spawn(pos);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1) && screenPos.y < tileBrowser.transform.position.y)
            {
                if (atLoc == PrimaryMapTile)
                {
                    SwapPrimaryAndSecondary();
                }
                else
                {
                    SecondaryMapTile = atLoc != secondaryMapTile ? atLoc : null;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
            {
                DeleteTile();
            }
        }


	}

    /// <summary>
    /// Processes the input and determines the action to be taken in the editor.
    /// </summary>
    void ProcessInput() {

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Mode = MapEditMode.Translate;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            Mode = MapEditMode.Rotate;
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            Mode = MapEditMode.Scale;
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            Mode = MapEditMode.Flip;
        }
        else if (Input.GetKey(KeyCode.B))
        {
            Mode = MapEditMode.Align;
        } else if (Input.GetKeyDown(KeyCode.Space) && ShowConnectButton) {
            ToggleConnection();
        }


        if (mode == MapEditMode.None)
        {
            Camera.main.transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * cameraSpeed * Time.deltaTime;
        } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
            switch(mode) {
                case MapEditMode.Translate:
                    PrimaryMapTile.Translate(Vector3.left);
                    OnTileMoved.Invoke(PrimaryMapTile);
                    break;
                case MapEditMode.Rotate:
                    PrimaryMapTile.Rotate(90);
                    break;
                case MapEditMode.Scale:
                    PrimaryMapTile.ScaleX(false);
                    break;
                case MapEditMode.Flip:
                    PrimaryMapTile.FlipX();
                    break;
                case MapEditMode.Align:
                    SpawnAligned(Direction.Left);
                    break;
            }
        } else if  (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)){
            switch (mode)
            {
                case MapEditMode.Translate:
                    PrimaryMapTile.Translate(Vector3.right);
                    OnTileMoved.Invoke(PrimaryMapTile);
                    break;
                case MapEditMode.Rotate:
                    PrimaryMapTile.Rotate(-90);
                    break;
                case MapEditMode.Scale:
                    PrimaryMapTile.ScaleX(true);
                    break;
                case MapEditMode.Flip:
                    PrimaryMapTile.FlipX();
                    break;
                case MapEditMode.Align:
                    SpawnAligned(Direction.Right);
                    break;
            }
        } else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            switch (mode)
            {
                case MapEditMode.Translate:
                    PrimaryMapTile.Translate(Vector3.up);
                    OnTileMoved.Invoke(PrimaryMapTile);
                    break;
                case MapEditMode.Scale:
                    PrimaryMapTile.ScaleY(true);
                    break;
                case MapEditMode.Flip:
                    PrimaryMapTile.FlipY();
                    break;
                case MapEditMode.Align:
                    SpawnAligned(Direction.Up);
                    break;
            }
        } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            switch (mode)
            {
                case MapEditMode.Translate:
                    PrimaryMapTile.Translate(Vector3.down);
                    OnTileMoved.Invoke(PrimaryMapTile);
                    break;
                case MapEditMode.Scale:
                    PrimaryMapTile.ScaleY(false);
                    break;
                case MapEditMode.Flip:
                    PrimaryMapTile.FlipY();
                    break;
                case MapEditMode.Align:
                    SpawnAligned(Direction.Down);
                    break;
            }
        } 
    }

    /// <summary>
    /// Spawn the selected prefab the given position.
    /// </summary>
    /// <param name="pos">Position.</param>
    void Spawn(Vector3 pos) {
        
        if (IsSpawnPositionOpen(pos))
        {
            GameObject go = Instantiate(tileBrowser.SelectedPrefab.gameObject);
            PrimaryMapTile = go.GetComponent<MapTile>();


            //pos.z = SelectedMapTile.ZPos;
            go.transform.position = pos;
            go.name = tileBrowser.SelectedPrefab.name;
            Map.Instance.AddTile(PrimaryMapTile);
        }
    }

    /// <summary>
    /// Spawns an instance of the selected prefab aligned with the selected object.
    /// </summary>
    /// <param name="dir">Dir.</param>
    void SpawnAligned(Direction dir)
    {
        if (tileBrowser.IsTileSelected && PrimaryMapTile != null)
        {
            SpriteRenderer spSpr = tileBrowser.SelectedPrefab.GetComponent<SpriteRenderer>();
            SpriteRenderer sgoSpr = PrimaryMapTile.GetComponent<SpriteRenderer>();
            float selectedGameObjectWidth = sgoSpr.bounds.size.x;
            float selectedGameObjectHeight = sgoSpr.bounds.size.y;
            float selectedPrefabWidth = spSpr.bounds.size.x;
            float selectedPrefabHeight = spSpr.bounds.size.y;
            Vector3 spawnPosition = Vector3.zero;
            switch (dir)
            {
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
    /// Deletes the selected tile.
    /// </summary>
    void DeleteTile() {
        if (PrimaryMapTile)
        {
            OnTileDestroyed.Invoke(PrimaryMapTile);
            Map.Instance.RemoveTile(PrimaryMapTile);
            selectedObjects.Remove(PrimaryMapTile);
            if (Map.Instance.NumberOfTiles > 0)
            {
                PrimaryMapTile = Map.Instance.GetTile(Map.Instance.NumberOfTiles - 1);
            } else {
                PrimaryMapTile = null;
            }
        }
    }

    /// <summary>
    /// Determines whether or not an object can be spawned at the given position
    /// </summary>
    /// <returns><c>true</c>, if spawn position open was ised, <c>false</c> otherwise.</returns>
    /// <param name="spawnPos">Spawn position.</param>
    public bool IsSpawnPositionOpen(Vector3 spawnPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(spawnPos);
       
        if (!tileBrowser.IsTileSelected)
        {
            return false;
        }

      
        if (screenPos.y > tileBrowser.transform.position.y) {
            return false;
        }

        for (int i = 0; i < Map.Instance.NumberOfTiles; i++)
        {
            MapTile tile = Map.Instance.GetTile(i);

            if (tile.Overlaps(tileBrowser.SelectedPrefab,spawnPos))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Opens the level select menu.
    /// </summary>
    public void OpenLevelSelect() {
        
        if (Application.isEditor) {
            #if UNITY_EDITOR
            string path  = EditorUtility.OpenFilePanel("Open a Level File", Application.absoluteURL + "/Assets/Resources/Levels", "json");

            if (path.Length > 0)
            {
                Map.Instance.Load(path);
                AddConnectionLines();

            }
            #endif
        } else {
            ShowLevelBrowser = true;
            mode = MapEditMode.Load;
        }
    }

    /// <summary>
    /// Loads the level at the given path.
    /// </summary>
    /// <param name="path">Path.</param>
    public void LoadLevel(string path) {
        Map.Instance.Load(path);
        AddConnectionLines();
        ShowLevelBrowser = false;
        mode = MapEditMode.None;
    }

    /// <summary>
    /// Goes back to the editor from the loading screen
    /// </summary>
    public void CancelLoad() {
        ShowLevelBrowser = false;
        mode = MapEditMode.None;
    }

    /// <summary>
    /// Adds the lines that show the connections between activated objects and activators in the editor.
    /// </summary>
    void AddConnectionLines() {
        foreach (ConnectionLine cl in connectionLines) {
            Destroy(cl.gameObject);
        }

        connectionLines.Clear();
        foreach (ActivatorConnection c in Map.Instance.Connections)
        {
            CreateConnectionLine(c);
        }
    }

    void CreateConnectionLine(ActivatorConnection c) {
        ConnectionLine connectionLine = Instantiate(connectionPrefab).GetComponent<ConnectionLine>();
        connectionLine.Connection = c;
        connectionLines.Add(connectionLine);
    }
    /// <summary>
    /// Opens the menu for saving the level.
    /// </summary>
    public void SaveLevel() {

        if (Application.isEditor)
        {
            #if UNITY_EDITOR
            string path = EditorUtility.SaveFilePanel("Save the Level", Application.absoluteURL + "/Assets/Resources/Levels", Map.Instance.FileName, "json");

            if (path.Length > 0)
            {
                File.WriteAllText(path, Map.Instance.SaveToFile());
            }
            #endif
        }
        else
        {
            mode = MapEditMode.Save;
            ShowSaveUI = true;
        }


    }

    /// <summary>
    /// Takes the menu out of the save menu
    /// </summary>
    public void EndSave() {
        mode = MapEditMode.None;
        ShowSaveUI = false;
    }

    /// <summary>
    /// Sets a value indicating whether or not to <see cref="T:LevelEditor"/> show the save user interface.
    /// </summary>
    /// <value><c>true</c> if show save user interface; otherwise, <c>false</c>.</value>
    bool ShowSaveUI {
        set {
            saveUI.gameObject.SetActive(value);
            toolsUI.gameObject.SetActive(!value);
            playbackUI.gameObject.SetActive(!value);
            tileBrowser.gameObject.SetActive(!value);
            saveButton.gameObject.SetActive(!value);
            loadButton.gameObject.SetActive(!value);
        }
    }



    /// <summary>
    /// Gets or sets the selected game object.
    /// </summary>
    /// <value>The selected game object.</value>
    MapTile PrimaryMapTile
    {

        set
        {

            foreach (MapTile mt in selectedObjects)
            {
                mt.HighlightState = MapHighlightState.Normal;
            }
            selectedObjects.Clear();
            if (value)
            {
                value.HighlightState = MapHighlightState.Primary;
                selectedObjects.Add(value);

                activateButton.gameObject.SetActive(value.GetComponent<ActivatedObject>());
                if (activateButton.gameObject.activeSelf) {
                    activateButton.GetComponentInChildren<Text>().text = value.GetComponent<ActivatedObject>().StartsActive ? "Deactivate" : "Activate";
                }
                connectButton.gameObject.SetActive(ShowConnectButton);

                if (ShowConnectButton) {
                    UpdateConnectButtonText();
                } else {
                    invertButton.gameObject.SetActive(false);
                }

            } else {
                activateButton.gameObject.SetActive(false);
                connectButton.gameObject.SetActive(false);
                invertButton.gameObject.SetActive(false);
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
    MapTile SecondaryMapTile
    {
        get
        {
            return secondaryMapTile;
        }

        set
        {
            if (secondaryMapTile && secondaryMapTile.HighlightState == MapHighlightState.Secondary)
            {
                secondaryMapTile.HighlightState = MapHighlightState.Normal;
            }

            secondaryMapTile = value;

            if (secondaryMapTile)
            {
                secondaryMapTile.HighlightState = MapHighlightState.Secondary;

                connectButton.gameObject.SetActive(ShowConnectButton);

                if (ShowConnectButton)
                {
                    UpdateConnectButtonText();
                }
            } else {
                connectButton.gameObject.SetActive(false);
            }
        }
    }


    /// <summary>
    /// Swaps the Primary and Secondary MapTiles.
    /// </summary>
    void SwapPrimaryAndSecondary()
    {
        MapTile temp = SecondaryMapTile;
        SecondaryMapTile = PrimaryMapTile;
        PrimaryMapTile = temp;
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:LevelEditor"/> shift key is held down.
    /// </summary>
    /// <value><c>true</c> if is shift down; otherwise, <c>false</c>.</value>
    bool IsShiftDown {
        get {
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }
    }

    /// <summary>
    /// Gets the instance on the Level Editor.
    /// </summary>
    /// <value>The instance.</value>
    public static LevelEditor Instance {
        get {
            return instance;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:LevelEditor"/> is hidden.
    /// </summary>
    /// <value><c>true</c> if is hidden; otherwise, <c>false</c>.</value>
    public bool IsHidden {
        get {
            return gameObject.activeSelf;
        }

        set {
            gameObject.SetActive(!value);
        }
    }


    /// <summary>
    /// Gets or sets the current mode of the editor.
    /// </summary>
    /// <value>The mode.</value>
    public MapEditMode Mode {
        set {
            mode = value;
            toolsUI.Mode = mode;
        }

        get {
            return mode;
        }
    }

    /// <summary>
    /// Switches the mode to or from Translate
    /// </summary>
    public void ToggleTranslate() {
        ToggleMode(MapEditMode.Translate);
    }

    /// <summary>
    /// Switches the mode to or from Rotate
    /// </summary>
    public void ToggleRotate()
    {
        ToggleMode(MapEditMode.Rotate);
    }

    /// <summary>
    /// Switches the mode to or from Scale
    /// </summary>
    public void ToggleScale()
    {
        ToggleMode(MapEditMode.Scale);
    }

    /// <summary>
    /// Switches the mode to or from Align
    /// </summary>
    public void ToggleAlign()
    {
        ToggleMode(MapEditMode.Align);
    }

    /// <summary>
    /// Switches the mode to or from Flip
    /// </summary>
    public void ToggleFlip()
    {
        ToggleMode(MapEditMode.Flip);
    }

    /// <summary>
    /// Toggles the mode on/off.
    /// </summary>
    /// <param name="mem">Mem.</param>
    void ToggleMode(MapEditMode mem) {
        if (mode == mem) {
            Mode = MapEditMode.None;
        } else {
            Mode = mem;
        }


    }

    /// <summary>
    /// Toggles the activation state of the primary map tile if possible.
    /// </summary>
    public void TogglePrimaryActivation() {
        ActivatedObject activatedObject = PrimaryMapTile.GetComponent<ActivatedObject>();
        activatedObject.StartsActive = !activatedObject.StartsActive;
        activateButton.GetComponentInChildren<Text>().text = activatedObject.StartsActive ? "Deactivate" : "Activate";
    }

    /// <summary>
    /// Creates or destroys a connection between the Primary Map Tile to the Secondary Tile if possible
    /// </summary>
    public void ToggleConnection() {
        
        ActivatorObject activator = null;
        ActivatedObject activated = null;
        if (PrimaryMapTile.HasCompnent<ActivatorObject>()) {
            activator = PrimaryMapTile.GetComponent<ActivatorObject>();
            activated = SecondaryMapTile.GetComponent<ActivatedObject>();
        } else {
            activator = SecondaryMapTile.GetComponent<ActivatorObject>();
            activated = PrimaryMapTile.GetComponent<ActivatedObject>();
        }

        if (activator.HasConnection(activated))
        {
            activator.RemoveConnection(activated);
        }
        else
        {
            ActivatorConnection connection = new ActivatorConnection(activator.ID, activated.ID, false);
            CreateConnectionLine(connection);
            Map.Instance.Connections.Add(connection);
            activator.AddConection(connection);
        }

        UpdateConnectButtonText();
    }

    /// <summary>
    /// Toggles the inverted status of the connection between the select objects.
    /// </summary>
    public void ToggleInverted() {
        ActivatorObject activator = null;
        ActivatedObject activated = null;
        if (PrimaryMapTile.HasCompnent<ActivatorObject>())
        {
            activator = PrimaryMapTile.GetComponent<ActivatorObject>();
            activated = SecondaryMapTile.GetComponent<ActivatedObject>();
        }
        else
        {
            activator = SecondaryMapTile.GetComponent<ActivatorObject>();
            activated = PrimaryMapTile.GetComponent<ActivatedObject>();
        }

        ActivatorConnection connection = Map.Instance.GetConnection(activator, activated);
        connection.IsInverted = !connection.IsInverted;
        onConnectionInverted.Invoke(connection);
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the <see cref="T:LevelEditor"/> level browser is shown.
    /// </summary>
    /// <value><c>true</c> if show level browser; otherwise, <c>false</c>.</value>
    bool ShowLevelBrowser {
        get {
            return levelBrowser.activeSelf;
        }

        set {
            playbackUI.SetActive(!value);
            toolsUI.gameObject.SetActive(!value);
            tileBrowser.gameObject.SetActive(!value);
            levelBrowser.SetActive(value);
        }
    }

    /// <summary>
    /// Gets a value indicating whether or not <see cref="T:LevelEditor"/> the connect button is shown.
    /// </summary>
    /// <value><c>true</c> if show connect button; otherwise, <c>false</c>.</value>
    bool ShowConnectButton {
        get {
            if (PrimaryMapTile && SecondaryMapTile)
            {
                if (PrimaryMapTile.HasCompnent<ActivatorObject>())
                {
                    return SecondaryMapTile.HasCompnent<ActivatedObject>();
                  
                }
                else if (SecondaryMapTile.HasCompnent<ActivatorObject>())
                {
                    return PrimaryMapTile.HasCompnent<ActivatedObject>();
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Updates the connect button text.
    /// </summary>
    void UpdateConnectButtonText() {
        ActivatorObject activator;
        ActivatedObject activated;
        if (PrimaryMapTile.HasCompnent<ActivatorObject>()) {
            activator = PrimaryMapTile.GetComponent<ActivatorObject>();
            activated = SecondaryMapTile.GetComponent<ActivatedObject>();
        }  else {
            activator = SecondaryMapTile.GetComponent<ActivatorObject>();
            activated = PrimaryMapTile.GetComponent<ActivatedObject>();
        }

        connectButton.GetComponentInChildren<Text>().text = (activator.HasConnection(activated) ? "Disconnect" : "Connect") + " (Space)";
        invertButton.gameObject.SetActive(activator.HasConnection(activated));

    }

    /// <summary>
    /// Gets the on tile moved event.
    /// </summary>
    /// <value>The on tile moved.</value>
    public MapTileEvent OnTileMoved {
        get {
            return onTileMoved;
        }
    }

    /// <summary>
    /// Gets the on tile destroyed event.
    /// </summary>
    /// <value>The on tile destroyed.</value>
    public MapTileEvent OnTileDestroyed
    {
        get
        {
            return onTileDestroyed;
        }
    }

    /// <summary>
    /// Gets the on connection removed event.
    /// </summary>
    /// <value>The on connection removed event.</value>
    public ConnectionEvent OnConnectionRemoved {
        get {
            return onConnectionRemoved;
        }
    }

    /// <summary>
    /// Gets the event that is triggered whenever a connection is inverted.
    /// </summary>
    /// <value>The on connection inverted.</value>
    public ConnectionEvent OnConnectionInverted {
        get {
            return onConnectionInverted;
        }
    }

    bool HideConnections {
        set {
            foreach(ConnectionLine cl in connectionLines) {
                cl.IsHidden = value;
            }
        }
    }
}

/// <summary>
/// Map edit mode.
/// </summary>
public enum MapEditMode
{
    Translate,
    Rotate,
    Scale,
    Flip,
    Align,
    Save,
    Load,
    None
}

[SerializeField]
public class MapTileEvent: UnityEvent<MapTile> {
}

[SerializeField]
public class ConnectionEvent: UnityEvent<ActivatorConnection> {
}