using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class LevelEditor : MonoBehaviour {
    [SerializeField]
    GameObject tileButtonPrefab;
    MapTile selectedPrefab;

    List<MapTile> selectedObjects;
    MapTile secondaryMapTile;
    MapEditMode mode = MapEditMode.None;

    Dictionary<string, GameObject> prefabs;

    int currentIndex = 0;
    int numButtons = 8;
    Vector3 buttonStart = new Vector3(-1050, 600);
    List<TileSelectButton> tileButtons;
    TileSelectButton selected;

    float cameraSpeed = 2.0f;

    static LevelEditor instance;
	// Use this for initialization
	void Start () {
        instance = this;
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
            button.transform.SetParent(transform);
            button.transform.localScale = Vector3.one;
            button.transform.localPosition = buttonStart + Vector3.right * 300 * i;
            button.editor = this;
            tileButtons.Add(button);
        }

        UpdateButtons();

        selectedObjects = new List<MapTile>();

	}
	
	// Update is called once per frame
	void Update () {
        if (!Game.Instance.IsPlaying)
        {
            if (mode == MapEditMode.None)
            {
                Camera.main.transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * cameraSpeed * Time.deltaTime;
            }
            Vector3 mousePos = Controller.Instance.MousePosition;
            Vector3 pos = new Vector3(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y));

            MapTile atLoc = Map.Instance.GetObjectAtLocation(pos);
            if (selected)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
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
                            if (IsShiftDown)
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
                    else
                    {
                        Spawn(pos);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Mouse1))
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

        if (screenPos.y > buttonStart.y) {
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
            string path  = EditorUtility.OpenFilePanel("Open a Level File", Application.absoluteURL + "/Assets/Resources/Levels", "json");

            if (path.Length > 0) {
                Map.Instance.Load(path);
            }
        }
    }

    public void SaveLevel() {
        if (Application.isEditor) {
            string path = EditorUtility.SaveFilePanel("Save the Level", Application.absoluteURL + "/Assets/Resources/Levels", Map.Instance.FileName, "json");

            if (path.Length > 0) {
                File.WriteAllText(path, Map.Instance.SaveToFile());
            }
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
            mode = MapEditMode.None;
        } else {
            mode = mem;
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