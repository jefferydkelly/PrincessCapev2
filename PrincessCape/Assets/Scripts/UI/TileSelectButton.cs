using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileSelectButton : Button
{
    bool isSelected = false;
    MapTile tilePrefab;
    Image tileSprite;
    Text tileText;
    public LevelEditor editor;
    protected override void Awake()
    {
        base.Awake();
        tileSprite = GetComponentsInChildren<Image>()[1];
        tileText = GetComponentInChildren<Text>();

    }

    public bool IsSelected
    {
        get
        {
            return isSelected;
        }

        set {
            isSelected = value;
        }
    }

    public MapTile Tile
    {
        get
        {
            return tilePrefab;
        }

        set
        {
            tilePrefab = value;
            tileSprite.sprite = tilePrefab.ButtonSprite;
            tileText.text = tilePrefab.name.SplitCamelCase();
        }
    }

    public void SelectButton()
    {
        editor.SelectButton(this);
    }
}
