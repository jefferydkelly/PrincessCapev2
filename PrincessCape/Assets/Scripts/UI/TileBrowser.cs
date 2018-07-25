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

    private void Update()
    {
        if (selectedPrefab) {
            selectedPrefab.transform.position = Controller.Instance.MousePosition;
        }
    }

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

            if (selectedPrefab) {
                Destroy(selectedPrefab.gameObject);
            }

            selectedPrefab = Instantiate(selected.Tile);
            selectedPrefab.GetComponent<SpriteRenderer>().color = selectedPrefab.GetComponent<SpriteRenderer>().color.SetAlpha(0.25f);
        }
        else
        {
            selected.IsSelected = false;
            selected = null;

            if (selectedPrefab)
            {
                Destroy(selectedPrefab.gameObject);
            }

            selectedPrefab = null;
        }
    }

    public bool IsTileSelected {
        get {
            return selected != null;
        }
    }

    public MapTile SelectedPrefab {
        get {
            return selectedPrefab;
        }
    }
}
