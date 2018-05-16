﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour {
	[SerializeField]
	EditorLayer layer = EditorLayer.Foreground;

    [SerializeField]
    Rect editorTextureRect = new Rect(0, 0, 128, 128);
    int id = -1;

    protected bool initialized = false;

	/// <summary>
	/// Dehighlights this instance on awake
	/// </summary>
	private void Awake()
	{
        if (!initialized)
		{
			Init();
            
        }
	}

    public virtual void Init() {
		HighlightState = MapHighlightState.Normal;
		initialized = true;

    }
    /// <summary>
    /// Gets the ZPos of this <see cref="T:EditorTile"/> .
    /// </summary>
    /// <value>The ZP os.</value>
    public int ZPos
	{
		get
		{
			return (int)layer;
		}
	}

	public int ID
	{
		get
		{
			return id;
		}

		set
		{
			id = value;
		}
	}

	/// <summary>
	/// Gets the bounds of this instance.
	/// </summary>
	/// <value>The bounds.</value>
	public virtual Vector2 Bounds
	{
		get
		{
			Vector2 bounds = GetComponent<SpriteRenderer>().bounds.size;
			return bounds.RotateDeg(transform.eulerAngles.z);
		}
	}

    /// <summary>
	/// Determines whether or not this tile overlaps the given position.
    /// </summary>
	/// <returns>Whether it would overlap the position (<see langword="true"/>), <see langword="false"/> otherwise.</returns>
    /// <param name="pos">The position to be checked against.</param>
	public virtual bool Overlaps(Vector3 pos)
	{
		Vector3 dif = pos - Center;
		Vector3 bounds = Bounds / 2;
		return dif.x.BetweenEx(-bounds.x, bounds.x) && dif.y.BetweenEx(-bounds.y, bounds.y);
	}
    /// <summary>
    /// Determines whether or not this tile would overlap a tile at the given position.
    /// </summary>
	/// <returns>Whether it would overlap the other tile (<see langword="true"/>), <see langword="false"/> otherwise.</returns>
    /// <param name="other">The MapTile to be checked against.</param>
    /// <param name="spawnPos">The position where the tile will be spawned.</param>
	public virtual bool Overlaps(MapTile other, Vector3 spawnPos)
	{
		Vector3 dif = spawnPos - Center;
		Vector2 bounds = (other.Bounds + Bounds) / 2;
		return dif.x.BetweenEx(-bounds.x, bounds.x) && dif.y.BetweenEx(-bounds.y, bounds.y);
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="T:EditorTile"/> is highlighted.
	/// </summary>
	/// <value><c>true</c> if highlighted; otherwise, <c>false</c>.</value>
	public bool Highlighted
	{
		get
		{
			return GetComponent<SpriteRenderer>().color != Color.white;
		}
	}

	public virtual MapHighlightState HighlightState
	{
		get
		{
			Color myCol = GetComponent<SpriteRenderer>().color;

			if (myCol == Color.white)
			{
				return MapHighlightState.Normal;
			}
			else if (myCol == Color.blue)
			{
				return MapHighlightState.Primary;
            } else if (myCol == Color.cyan) {
                return MapHighlightState.Backup;
            }
			else
			{
				return MapHighlightState.Secondary;
			}
		}

		set
		{
			if (value == MapHighlightState.Normal)
			{
                float oldA = GetComponent<SpriteRenderer>().color.a;
                GetComponent<SpriteRenderer>().color = Color.white.SetAlpha(Application.isPlaying ? 1 : oldA);
			}
			else if (value == MapHighlightState.Primary)
			{
				GetComponent<SpriteRenderer>().color = Color.blue;
			}
            else if (value == MapHighlightState.Backup) {
                GetComponent<SpriteRenderer>().color = Color.cyan;
            }
			else
			{
				GetComponent<SpriteRenderer>().color = Color.red;
			}
		}
	}

	/// <summary>
	/// Horizontally flips the tile
	/// </summary>
	public virtual void FlipX()
	{
		SpriteRenderer spr = GetComponent<SpriteRenderer>();
		spr.flipX = !spr.flipX;
	}

	/// <summary>
	/// Vertically flips the tile
	/// </summary>
	public virtual void FlipY()
	{
		SpriteRenderer spr = GetComponent<SpriteRenderer>();
		spr.flipY = !spr.flipY;
	}

	/// <summary>
	/// Translate the tile by the specified vector.
	/// </summary>
	/// <param name="vec">Vec.</param>
	public virtual void Translate(Vector3 vec)
	{
		transform.position += vec;
	}

	/// <summary>
	/// Rotate the tile by specified angle.
	/// </summary>
	/// <param name="ang">Ang.</param>
	public virtual void Rotate(float ang)
	{
        transform.rotation *= Quaternion.AngleAxis(ang, Vector3.forward);
		//transform.Rotate(0, 0, ang);
	}

	/// <summary>
	/// Scales the tile horizontally
	/// </summary>
	/// <param name="right">If set to <c>true</c> increases the size, <c>false</c>, decreases.</param>
	public virtual void ScaleX(bool right)
	{
		if (right)
		{
			transform.localScale += Vector3.right;
            transform.position += transform.right * 0.5f;
		}
		else if (transform.localScale.x > 1)
		{
			transform.localScale += Vector3.left;
            transform.position += transform.right * -0.5f;
		}
	}

	/// <summary>
	/// Scales the tile vertically
	/// </summary>
	/// <param name="up">If set to <c>true</c> increases the size, <c>false</c>, decreases.</param>
	public virtual void ScaleY(bool up)
	{
		if (up)
		{
            transform.localScale += Vector3.up;
            transform.localPosition += Vector3.up / 2.0f;
		}
		else if (transform.localScale.y > 1)
		{
            transform.localScale -= Vector3.up;
            transform.localPosition -= Vector3.up / 2.0f;
		}
	}

    public virtual void ScaleY(float yscale) {
        float scaleDif = yscale - transform.localScale.y;
        transform.localScale = transform.localScale.SetY(yscale);
        transform.localPosition += Vector3.up * scaleDif / 2;

    }

	/// <summary>
	/// Delete this instance.
	/// </summary>
	public virtual void Delete()
	{
        if (gameObject)
        {
            DestroyImmediate(gameObject);
        }
	}

	

	/// <summary>
	/// Renders the in editor.
	/// </summary>
	public virtual void RenderInEditor()
	{

	}

    /// <summary>
    /// Creates a string of Save Data for the Tile.
    /// </summary>
    /// <returns>A string representing the object.</returns>
    public string SaveData() {
        if (transform.parent.name  != "Map") {
            return "";
        }
        string info = PCLParser.StructStart;
        info += GenerateSaveData();
        info += PCLParser.StructEnd;
        return info;
    }

    protected virtual string GenerateSaveData() {
        string info = "";
		info += PCLParser.CreateAttribute("Name", name.Split('(')[0]);
		info += PCLParser.CreateAttribute("ID", ID);
		info += PCLParser.CreateAttribute("Position", transform.position);
		info += PCLParser.CreateAttribute("Rotation", new Vector3(0, 0, transform.eulerAngles.z));
		info += PCLParser.CreateAttribute("Scale", transform.localScale);
        return info;
    }

    public virtual void FromData(TileStruct tile) {
        id = tile.id;
        transform.position = PCLParser.ParseVector3(tile.NextLine).SetZ((float)layer);
        Vector3 rot = PCLParser.ParseVector3(tile.NextLine);

        transform.localScale = PCLParser.ParseVector3(tile.NextLine);
        transform.rotation *= Quaternion.AngleAxis(rot.z, Vector3.forward);
    }

    public Texture EditorTexture {
        get {
            int wid = (int)editorTextureRect.width;
            int hite = (int)editorTextureRect.height;
            Color[] pix = GetComponent<SpriteRenderer>().sprite.texture.GetPixels((int)editorTextureRect.x, (int)editorTextureRect.y, wid, hite);
            Texture2D editorTexture = new Texture2D(wid, hite);
            editorTexture.SetPixels(pix);
            editorTexture.Apply();
            return editorTexture;
        }
    }

    /// <summary>
    /// Gets the layer the object is on.
    /// </summary>
    /// <value>The layer.</value>
    protected EditorLayer Layer {
        get {
            return layer;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:MapTile"/> is initialized.
    /// </summary>
    /// <value><c>true</c> if is initialized; otherwise, <c>false</c>.</value>
	public bool IsInitialized {
		get {
			return initialized;
		}
	}

	public virtual Vector3 Center {
		get {
			return transform.position;
		}
	}

}

public enum EditorLayer
{
	Foreground,
	Background
}

public enum MapHighlightState {
    Normal,
    Primary,
    Secondary,
    Backup
}

