using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using UnityEngine.Events;

public class LevelEditor : MonoBehaviour {
    [SerializeField]
    GameObject tileButtonPrefab;
    [SerializeField]
    GameObject levelBrowser;
    [SerializeField]
    ToolsUI toolsUI;
    [SerializeField]
    GameObject playbackUI;
    [SerializeField]
    GameObject tileBrowser;
    MapTile selectedPrefab;
    [SerializeField]
    Button activateButton;
    [SerializeField]
    Button connectButton;
    [SerializeField]
    GameObject connectionPrefab;

    List<ConnectionLine> connectionLines;
    List<MapTile> selectedObjects;
    MapTile secondaryMapTile;
    MapEditMode mode = MapEditMode.None;

    Dictionary<string, GameObject> prefabs;

    int currentIndex = 0;
    int numButtons = 8;
    Vector3 buttonStart = new Vector3(-1050, 0);
    List<TileSelectButton> tileButtons;
    TileSelectButton selected;

    float cameraSpeed = 2.0f;

    //MapTile Events
    MapTileEvent onTileMoved = new MapTileEvent();

    static LevelEditor instance;
	// Use this for initialization
	void Start () {
        instance = this;
        connectionLines = new List<ConnectionLine>();
        Object[] obj = Resources.LoadAll("Tiles", typeof(GameObject));

        prefabs = new Dictionary<string, GameObject>(obj.Length);

        for (int i = 0; i < obj.Length; i++)
        {
            GameObject go = (GameObject)obj[i];
            prefabs.Add(go.name, go);
        }

        Map.Instance.Init();

        tileButtons = new List<TileSelectButton>();
        for (int i = 0; i < numButtons; i++)
        {
            TileSelectButton button = Instantiate(tileButtonPrefab).GetComponent<TileSelectButton>();
            button.transform.SetParent(tileBrowser.transform);
            button.transform.localScale = Vector3.one;
            button.transform.localPosition = buttonStart + Vector3.right * 300 * i;
            button.editor = this;
            tileButtons.Add(button);
        }
        activateButton.gameObject.SetActive(false);
        connectButton.gameObject.SetActive(false);
        UpdateButtons();

        levelBrowser.SetActive(false);

        selectedObjects = new List<MapTile>();

	}
	
	// Update is called once per frame
	void Update () {
        if (!Game.Instance.IsPlaying)
        {
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
                else if (selected)
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

    void Spawn(Vector3 pos) {
        if (IsSpawnPositionOpen(pos))
        {
            GameObject go = Instantiate(selectedPrefab.gameObject);
            PrimaryMapTile = go.GetComponent<MapTile>();


            //pos.z = SelectedMapTile.ZPos;
            go.transform.position = pos;
            go.name = selectedPrefab.name;
            Map.Instance.AddTile(PrimaryMapTile);
        }
    }

    /// <summary>
    /// Spawns an instance of the selected prefab aligned with the selected object.
    /// </summary>
    /// <param name="dir">Dir.</param>
    void SpawnAligned(Direction dir)
    {
        if (selectedPrefab != null && PrimaryMapTile != null)
        {
            SpriteRenderer spSpr = selectedPrefab.GetComponent<SpriteRenderer>();
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

    void DeleteTile() {
        if (PrimaryMapTile)
        {
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
    bool IsSpawnPositionOpen(Vector3 spawnPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(spawnPos);
       
        if (selectedPrefab == null)
        {
            return false;
        }

      
        if (screenPos.y > tileBrowser.transform.position.y) {
            return false;
        }

        for (int i = 0; i < Map.Instance.NumberOfTiles; i++)
        {
            MapTile tile = Map.Instance.GetTile(i);

            if (tile.Overlaps(selectedPrefab,spawnPos))
            {
                return false;
            }
        }

        return true;
    }

    public void StartLoadingLevel() {
        if (Application.isEditor) {
            #if UNITY_EDITOR
            string path  = EditorUtility.OpenFilePanel("Open a Level File", Application.absoluteURL + "/Assets/Resources/Levels", "json");

            if (path.Length > 0)
            {
                Map.Instance.Load(path);
                foreach (ConnectionStruct c in Map.Instance.Connections)
                {
                    ConnectionLine connectionLine = Instantiate(connectionPrefab).GetComponent<ConnectionLine>();
                    connectionLine.Connection = c;
                }

            }
            #endif
        } else {
            ShowLevelBrowser = true;
        }
    }

    public void LoadLevel(string path) {
        Map.Instance.Load(path);

        foreach(ConnectionStruct c in Map.Instance.Connections) {
            ConnectionLine connectionLine = Instantiate(connectionPrefab).GetComponent<ConnectionLine>();
            connectionLine.Connection = c;
        }
        ShowLevelBrowser = false;
    }
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
    }

    public void Increment() {
        currentIndex = Mathf.Min(currentIndex + 1, prefabs.Count - numButtons - 1);
        UpdateButtons();
    }

    public void Decrement() {
        currentIndex = Mathf.Max(currentIndex - 1, 0);
        UpdateButtons();
    }

    void UpdateButtons() {
        int i = 0;

        foreach (KeyValuePair<string, GameObject> kvp in prefabs)
        {
            

            if (i >= currentIndex)
            {
                tileButtons[i - currentIndex].Tile = kvp.Value.GetComponent<MapTile>();


                if (i >= currentIndex + numButtons - 1)
                {
                    break;
                }

            }

            i++;
        }    
    }

    public void SelectButton(TileSelectButton button) {
        if (selected != button)
        {
            if (selected)
            {
                selected.IsSelected = false;
            }
            selected = button;
            selected.IsSelected = true;
            selectedPrefab = selected.Tile;
        } else {
            selected.IsSelected = false;
            selected = null;
            selectedPrefab = null;
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
            } else {
                activateButton.gameObject.SetActive(false);
                connectButton.gameObject.SetActive(false);
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

    bool IsShiftDown {
        get {
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }
    }

    public static LevelEditor Instance {
        get {
            return instance;
        }
    }

    public bool IsHidden {
        get {
            return gameObject.activeSelf;
        }

        set {
            gameObject.SetActive(!value);
        }
    }

    public MapEditMode Mode {
        set {
            mode = value;
            toolsUI.Mode = mode;
        }

        get {
            return mode;
        }
    }

    public void ToggleTranslate() {
        ToggleMode(MapEditMode.Translate);
    }

    public void ToggleRotate()
    {
        ToggleMode(MapEditMode.Rotate);
    }

    public void ToggleScale()
    {
        ToggleMode(MapEditMode.Scale);
    }

    public void ToggleAlign()
    {
        ToggleMode(MapEditMode.Align);
    }

    public void ToggleFlip()
    {
        ToggleMode(MapEditMode.Flip);
    }


    void ToggleMode(MapEditMode mem) {
        if (mode == mem) {
            Mode = MapEditMode.None;
        } else {
            Mode = mem;
        }


    }

    public void TogglePrimaryActivation() {
        ActivatedObject activatedObject = PrimaryMapTile.GetComponent<ActivatedObject>();
        activatedObject.StartsActive = !activatedObject.StartsActive;
        activateButton.GetComponentInChildren<Text>().text = activatedObject.StartsActive ? "Deactivate" : "Activate";
    }

    public void ToggleConnection() {
        ActivatorObject activator = PrimaryMapTile.GetComponent<ActivatorObject>();
        ActivatedObject activated = SecondaryMapTile.GetComponent<ActivatedObject>();
        if (activator == null) {
            activator = SecondaryMapTile.GetComponent<ActivatorObject>();
            activated = PrimaryMapTile.GetComponent<ActivatedObject>();
        }

        if (activator.HasConnection(activated))
        {
            activator.RemoveConnection(activated);
        }
        else
        {
            activator.AddConnection(activated);
        }
    }
    bool ShowLevelBrowser {
        get {
            return levelBrowser.activeSelf;
        }

        set {
            playbackUI.SetActive(!value);
            toolsUI.gameObject.SetActive(!value);
            tileBrowser.SetActive(!value);
            levelBrowser.SetActive(value);
        }
    }

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

    public MapTileEvent OnTileMoved {
        get {
            return onTileMoved;
        }
    }

}

public enum MapEditMode
{
    Translate,
    Rotate,
    Scale,
    Flip,
    Align,
    None
}

[SerializeField]
public class MapTileEvent: UnityEvent<MapTile> {
}