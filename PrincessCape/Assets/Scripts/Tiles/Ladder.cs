using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Ladder : ActivatedObject
{
    [SerializeField]
    GameObject plainLadder;
    BoxCollider2D myCollider;
    Timer revealTimer;
    float revealTime = 0.25f;
    [SerializeField]
    SpriteRenderer activationCircle;
    public void Awake()
    {
        Init();
    }

    /// <summary>
    /// Initializes this instance setting the collider, reveal timer and determining whether or not it appears at the start of the game.
    /// </summary>
    public override void Init() {
        int numLinks = transform.childCount - 2;
        revealTimer = new Timer(revealTime, numLinks);
		revealTimer.OnTick.AddListener(RevealLadderSection);
        activationCircle = GetComponentsInChildren<SpriteRenderer>()[1];
        myCollider = GetComponent<BoxCollider2D>();
        myCollider.size = new Vector2(1, numLinks);


        if (Application.isPlaying)
		{
            if (Game.Instance.IsInLevelEditor)
            {
                RevealEverything();
                activationCircle.color = startActive ? Color.green : Color.red;
                Game.Instance.OnGameStateChanged.AddListener(OnGameStateChanged);
            }
            else
            {
                if (!startActive)
                {
                    gameObject.SetActive(false);
                    Deactivate();
                }
                else
                {
                    RevealEverything();
                }
            }
		}
    }

    void OnGameStateChanged(GameState state) {
        activationCircle.gameObject.SetActive(state != GameState.Playing);
    }

    private void OnDestroy()
    {
        if (!Game.isClosing)
        {
            Game.Instance.OnGameStateChanged.RemoveListener(OnGameStateChanged);
        }
    }
    /// <summary>
    /// Reveals every component of the ladder at once.
    /// </summary>
    void RevealEverything() {
        gameObject.SetActive(true);
		for (int i = 1; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(true);
		}

        AdjustSize(transform.childCount - 1);
    }

    void AdjustSize(int numLinks) {
        myCollider.size = myCollider.size.SetY(numLinks);

        myCollider.offset = new Vector2(0, -(numLinks - 1) / 2.0f);
    }
    /// <summary>
    /// Scales the Ladder vertically.
    /// </summary>
    /// <param name="up">If set to <c>true</c> increases the scale.  Otherwisem decreases it.</param>
    public override void ScaleY(bool up)
    {
        if (up)
        {

            transform.position += Vector3.up;
            SpawnLadderSection();
        }
        else if (transform.childCount > 2)
        {
            DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
            transform.position += Vector3.down;

        }

        AdjustSize(transform.childCount - 1);


    }

    /// <summary>
    /// Gets or sets the state of the highlight.
    /// </summary>
    /// <value>The state of the highlight.</value>
    public override MapHighlightState HighlightState
    {
        get
        {
            return base.HighlightState;
        }
        set
        {
            Color nextColor = Color.white;
            if (value == MapHighlightState.Primary)
            {
                nextColor = Color.blue;
            }
            else if (value == MapHighlightState.Secondary)
            {
                nextColor = Color.red;
			} else if (value == MapHighlightState.Backup) {
				nextColor = Color.cyan;
			}

            foreach (SpriteRenderer spr in GetComponentsInChildren<SpriteRenderer>())
            {
                if (spr != activationCircle) {
                    spr.color = nextColor;
                }
            }
        }
    }

    /// <summary>
    /// Gets the center of the ladder.
    /// </summary>
    /// <value>The center.</value>
	public override Vector3 Center
	{
		get
		{
			return transform.position + (Vector3)myCollider.offset;
		}
	}

    /// <summary>
    /// Gets the bounds of the ladder.
    /// </summary>
    /// <value>The bounds.</value>
	public override Vector2 Bounds
	{
		get
		{
            return new Vector2(1, transform.childCount - 1);
		}
	}

    /// <summary>
    /// Generates the save data for the ladder.
    /// </summary>
    /// <returns>The save data.</returns>
	protected override string GenerateSaveData()
    {
        string data = base.GenerateSaveData();
        data += PCLParser.CreateAttribute("Lines", transform.childCount - 1);
        return data;
    }

    /// <summary>
    /// Creates a ladder from the data given.
    /// </summary>
    /// <param name="tile">A struct containing the information for the ladder.</param>
    public override void FromData(TileStruct tile)
    {
        initialized = false;
        base.FromData(tile);
       
        int numLinks = PCLParser.ParseInt(tile.NextLine);
        for (int i = 0; i < numLinks; i++)
        {
            SpawnLadderSection();
        }

        if (!startActive && Application.isPlaying) {
            Deactivate();
        }

		myCollider = GetComponent<BoxCollider2D>();
        AdjustSize(transform.childCount - 1);
        revealTimer = new Timer(revealTime, numLinks);
		revealTimer.OnTick.AddListener(RevealLadderSection);
        initialized = true;
		
    }

    /// <summary>
    /// Starts the reveal of the ladder sections
    /// </summary>
    public override void Activate()
    {
        if (initialized)
        {
            gameObject.SetActive(true);
            revealTimer.Start();
        } else {
            RevealEverything();
        }
    }

    /// <summary>
    /// Makes all sections of the ladder disappear
    /// </summary>
    public override void Deactivate()
    {
        gameObject.SetActive(false);
        revealTimer.Stop();

		for (int i = 1; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}
        AdjustSize(1);
    }

    /// <summary>
    /// Spawns a new section of the ladder.
    /// </summary>
	void SpawnLadderSection()
	{
        GameObject tile = Instantiate(plainLadder);
		tile.transform.SetParent(transform);
        tile.transform.localPosition = (transform.childCount - 2) * Vector3.down;
        tile.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
        AdjustSize(transform.childCount - 1);
	}

    /// <summary>
    /// Reveals a section of the ladder.
    /// </summary>
	void RevealLadderSection()
	{
        if (revealTimer.IsRunning)
        {
            transform.GetChild(revealTimer.TicksCompleted).gameObject.SetActive(true);
            AdjustSize(revealTimer.TicksCompleted);
        }
	}

    public override bool StartsActive
    {
        set
        {
            base.StartsActive = value;
            if (Game.Instance.IsInLevelEditor) {
                activationCircle.color = startActive ? Color.green : Color.red;
            }
        }
    }
}
