using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour {
	[SerializeField]
	EditorLayer layer = EditorLayer.Foreground;

    [SerializeField]
    Rect editorTextureRect = new Rect(0, 0, 128, 128);
    int id = -1;

	/// <summary>
	/// Dehighlights this instance on awake
	/// </summary>
	private void Awake()
	{
        HighlightState = MapHighlightState.Normal;
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
	public virtual Vector3 Bounds
	{
		get
		{
			return GetComponent<SpriteRenderer>().bounds.size;
		}
	}

	public virtual bool Overlaps(Vector3 pos)
	{
		Vector3 dif = pos - transform.position;
		Vector3 bounds = Bounds / 2;
		return dif.x.BetweenEx(-bounds.x, bounds.x) && dif.y.BetweenEx(-bounds.y, bounds.y);
	}
	public virtual bool Overlaps(MapTile other, Vector3 spawnPos)
	{
		Vector3 dif = spawnPos - transform.position;
		Vector3 bounds = (other.Bounds + Bounds) / 2;
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
				GetComponent<SpriteRenderer>().color = Color.white;
			}
			else if (value == MapHighlightState.Primary)
			{
				GetComponent<SpriteRenderer>().color = Color.blue;
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
		transform.Rotate(0, 0, ang);
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
			transform.position += Vector3.right * 0.5f;
		}
		else if (transform.localScale.x > 1)
		{
			transform.localScale += Vector3.left;
			transform.position += Vector3.left * 0.5f;
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
            Debug.Log("Scale");
            transform.localScale += transform.up;
            transform.position += transform.up / 2.0f;
		}
		else if (transform.localScale.y > 1)
		{
            transform.localScale -= transform.up;
            transform.position -= transform.up / 2.0f;
		}
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
		info += PCLParser.CreateAttribute("Rotation", transform.rotation);
		info += PCLParser.CreateAttribute("Scale", transform.localScale);
        return info;
    }

    public virtual void FromData(TileStruct tile) {
        id = tile.id;
        transform.position = PCLParser.ParseVector3(PCLParser.ParseLine(tile.info[0]));
        Vector3 rot = PCLParser.ParseVector3(PCLParser.ParseLine(tile.info[1]));
		transform.rotation.Set(rot.x, rot.y, rot.z, 1);
		transform.localScale = PCLParser.ParseVector3(PCLParser.ParseLine(tile.info[2]));
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

}

public enum EditorLayer
{
	Foreground,
	Background
}

public enum MapHighlightState {
    Normal,
    Primary,
    Secondary
}

