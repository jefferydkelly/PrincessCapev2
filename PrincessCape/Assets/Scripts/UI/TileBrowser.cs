using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBrowser : MonoBehaviour {
    [SerializeField]
    GameObject tileButtonPrefab;

    int currentIndex = 0;
    int numButtons = 8;
    Vector3 buttonStart = new Vector3(-1050, 0);
    List<TileSelectButton> tileButtons;
    Dictionary<string, GameObject> prefabs;
  
    TileSelectButton selected;
    MapTile selectedPrefab;

    [SerializeField]
    PrefabCursor cursor;

	// Use this for initialization
	void Awake () {

        Object[] obj = Resources.LoadAll("Tiles", typeof(GameObject));

        prefabs = new Dictionary<string, GameObject>(obj.Length);

        for (int i = 0; i < obj.Length; i++)
        {
            GameObject go = (GameObject)obj[i];
            prefabs.Add(go.name, go);
        }

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
	}

    private void OnEnable()
    {
        cursor.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        cursor.gameObject.SetActive(false);
    }

    /// <summary>
    /// Increment the index of the current tiles 
    /// </summary>
    public void Increment()
    {
        currentIndex = Mathf.Min(currentIndex + 1, prefabs.Count - numButtons - 1);
        UpdateButtons();
    }

    public void Decrement()
    {
        currentIndex = Mathf.Max(currentIndex - 1, 0);
        UpdateButtons();
    }

    /// <summary>
    /// Updates the button sprites and backgrounds.
    /// </summary>
    void UpdateButtons()
    {
        int i = 0;

        foreach (KeyValuePair<string, GameObject> kvp in prefabs)
        {


            if (i >= currentIndex)
            {
                TileSelectButton tile = tileButtons[i - currentIndex];
                tile.Tile = kvp.Value.GetComponent<MapTile>();
                tile.IsSelected = tile.Tile == selectedPrefab;

                if (i >= currentIndex + numButtons - 1)
                {
                    break;
                }

            }

            i++;
        }
    }

    /// <summary>
    /// Selects the button.
    /// </summary>
    /// <param name="button">Button.</param>
    public void SelectButton(TileSelectButton button)
    {
        if (selected != button)
        {
            if (selected)
            {
                selected.IsSelected = false;
            }
         
            selected = button;
            selected.IsSelected = true;

           

            selectedPrefab = button.Tile;
            cursor.Sprite = selectedPrefab.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            selected.IsSelected = false;
            selected = null;

            selectedPrefab = null;
            cursor.Sprite = null;
        }
    }

    /// <summary>
    /// Gets a value indicating whether a tile is selected.
    /// </summary>
    /// <value><c>true</c> if is tile selected; otherwise, <c>false</c>.</value>
    public bool IsTileSelected {
        get {
            return selected != null;
        }
    }

    /// <summary>
    /// Gets the selected prefab.
    /// </summary>
    /// <value>The selected prefab.</value>
    public MapTile SelectedPrefab {
        get {
            return selectedPrefab;
        }
    }


}
