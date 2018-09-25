using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour {
	[SerializeField]
	EditorLayer layer = EditorLayer.Foreground;

    [SerializeField]
    Rect editorTextureRect = new Rect(0, 0, 128, 128);
    int id = -1;

    protected bool initialized = false;
    [SerializeField]
	protected string instanceName = "";
    protected MapHighlightState highlightState = MapHighlightState.Normal;

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

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public virtual void Init() {
        if (!initialized)
        {
            HighlightState = MapHighlightState.Normal;
            if (Game.Instance)
            {
                Game.Instance.OnGameStateChanged.AddListener(OnGameStateChanged);
            }
            //initialized = true;
        }

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

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
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
			bounds = bounds.RotateDeg(transform.eulerAngles.z);
			bounds = new Vector2(Mathf.Abs(bounds.x), Mathf.Abs(bounds.y));
			return bounds;
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

    /// <summary>
    /// Gets or sets the highlighted state of the MapTile.
    /// </summary>
    /// <value>The highlighted state of the MapTile..</value>
	public virtual MapHighlightState HighlightState
	{
		get
		{
            return highlightState;
		}

		set
		{
            highlightState = value;
            SpriteRenderer myRenderer = GetComponent<SpriteRenderer>();
			if (value == MapHighlightState.Normal)
			{
                float oldA = myRenderer.color.a;
                if (oldA < 1.0f) {
                    print(name);
                }
                myRenderer.color = Color.white.SetAlpha(Application.isPlaying ? 1 : oldA);
			}
			else if (value == MapHighlightState.Primary)
			{
                myRenderer.color = Color.blue;
			}
            else if (value == MapHighlightState.Backup) {
                myRenderer.color = Color.cyan;
            }
			else
			{
                myRenderer.color = Color.red;
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

    public virtual void Scale(Vector3 vec) {
        Vector3 newScale = transform.localScale + vec;
        if (newScale.x < 0) {
            vec = vec.SetX(0);
        }

        if (newScale.y < 0) {
            vec = vec.SetY(0);
        }

        transform.localScale += vec;
        transform.position += vec / 2.0f;
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
    /// An event for when the game state changes
    /// </summary>
    /// <param name="state">State.</param>
    protected virtual void OnGameStateChanged(GameState state) {
        
    }

    private void OnDestroy()
    {
        if (!Game.isClosing && Game.Instance) {
            Game.Instance.OnGameStateChanged.RemoveListener(OnGameStateChanged);
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

    /// <summary>
    /// Generates the save data for the tile.
    /// </summary>
    /// <returns>The save data for the tile.</returns>
    protected virtual string GenerateSaveData() {
        string info = "";
        info += PCLParser.CreateAttribute("Prefab", name.Split('(')[0]);
        info += PCLParser.CreateAttribute("Instance Name", instanceName);
		info += PCLParser.CreateAttribute("ID", ID);
		info += PCLParser.CreateAttribute("Position", transform.position);
		info += PCLParser.CreateAttribute("Rotation", new Vector3(0, 0, transform.eulerAngles.z));
		info += PCLParser.CreateAttribute("Scale", transform.localScale);
        return info;
    }

    /// <summary>
    /// Sets the values of the MapTile based on the data in the TileStruct
    /// </summary>
    /// <param name="tile">Tile struct containing information about the tile.</param>
    public virtual void FromData(TileStruct tile) {
        id = tile.id;
        instanceName = tile.instanceName;
        transform.position = PCLParser.ParseVector3(tile.NextLine).SetZ((float)layer);

        Vector3 rot = PCLParser.ParseVector3(tile.NextLine);

        transform.localScale = PCLParser.ParseVector3(tile.NextLine);
        transform.rotation *= Quaternion.AngleAxis(rot.z, Vector3.forward);
    }

    /// <summary>
    /// Gets the texture that is draw in the MapEditor.
    /// </summary>
    /// <value>The editor texture.</value>
    public Texture EditorTexture {
        get {
            
            return CreateTexture();
        }
    }

    /// <summary>
    /// Gets the Sprite to be used for buttons in the Unity Editor.
    /// </summary>
    /// <value>The button sprite.</value>
    public Sprite ButtonSprite {
        get {
            return Sprite.Create(CreateTexture(), new Rect(Vector2.zero, editorTextureRect.size), Vector2.zero);
        }
    }

    /// <summary>
    /// Creates the texture.
    /// </summary>
    /// <returns>The texture.</returns>
    Texture2D CreateTexture() {
        int wid = (int)editorTextureRect.width;
        int hite = (int)editorTextureRect.height;
        Color[] pix = GetComponent<SpriteRenderer>().sprite.texture.GetPixels((int)editorTextureRect.x, (int)editorTextureRect.y, wid, hite);
        Texture2D editorTexture = new Texture2D(wid, hite);
        editorTexture.SetPixels(pix);
        editorTexture.Apply();
        return editorTexture;
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

    /// <summary>
    /// Gets the center of the obhect.
    /// </summary>
    /// <value>The center.</value>
	public virtual Vector3 Center {
		get {
			return transform.position;
		}
	}

    /// <summary>
    /// Gets or sets the name of the instance.
    /// </summary>
    /// <value>The name of the instance.</value>
	public string InstanceName
	{
		get
		{
            return instanceName;
		}
        
		set {
			if (!Application.isPlaying || !initialized) {
				instanceName = value;
			}
		}
	}

}

public enum EditorLayer
{
	Foreground = 0,
	Background = 1,
    OnWall = 2,
    Wall = 3
}

public enum MapHighlightState {
    Normal,
    Primary,
    Secondary,
    Backup
}

