using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour {

	[SerializeField]
	EditorLayer layer = EditorLayer.Foreground;

	/// <summary>
	/// Dehighlights this instance on awake
	/// </summary>
	private void Awake()
	{
		Highlighted = false;
	}
#if UNITY_EDITOR
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
			transform.localScale += Vector3.up;
			transform.position += Vector3.up / 2.0f;
		}
		else if (transform.localScale.y > 1)
		{
			transform.localScale += Vector3.down;
			transform.position += Vector3.down / 2.0f;
		}
	}

	/// <summary>
	/// Delete this instance.
	/// </summary>
	public virtual void Delete()
	{
		DestroyImmediate(gameObject);
	}

	/// <summary>
	/// Gets the bounds of this instance.
	/// </summary>
	/// <value>The bounds.</value>
	public Vector3 Bounds
	{
		get
		{
			return GetComponent<SpriteRenderer>().bounds.size;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="T:EditorTile"/> is highlighted.
	/// </summary>
	/// <value><c>true</c> if highlighted; otherwise, <c>false</c>.</value>
	public bool Highlighted
	{
		set
		{
			GetComponent<SpriteRenderer>().color = value ? Color.blue : Color.white;
		}

		get
		{
			return GetComponent<SpriteRenderer>().color == Color.blue;
		}
	}

    public MapHighlightState HighlightState {
        get {
            Color myCol = GetComponent<SpriteRenderer>().color;
            if (myCol == Color.white) {
                return MapHighlightState.Normal;
            } else if (myCol == Color.blue) {
                return MapHighlightState.Primary;
            } else {
                return MapHighlightState.Secondary;
            }
        }

        set {
            if (value == MapHighlightState.Normal) {
                GetComponent<SpriteRenderer>().color = Color.white;
            } else if (value == MapHighlightState.Primary) {
                GetComponent<SpriteRenderer>().color = Color.blue;
            } else {
                GetComponent<SpriteRenderer>().color = Color.red;
            }
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
	/// Renders the in editor.
	/// </summary>
	public virtual void RenderInEditor()
	{

	}
#endif

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

