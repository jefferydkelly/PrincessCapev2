using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public abstract class ActivatedObject : MapTile
{

    //[SerializeField]
    //protected List<ActivatedObject> connections = new List<ActivatedObject>();
    [SerializeField]
    protected bool startActive = false;
    protected bool isActivated = false;
	[SerializeField]
	protected bool isConnected = false;
	[SerializeField]
	protected int requiredActivators = 1;
	protected int currentActivators = 0;

    private void Awake()
    {
        Init();

    }
    
	public override void Init()
	{
		base.Init();

		if (startActive && Application.isPlaying)
        {
            Activate();
            //EventManager.StartListening("LevelLoaded", Activate);
        }
	}
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="T:ActivatedObject"/> is activated.
	/// </summary>
	/// <value><c>true</c> if is activated; otherwise, <c>false</c>.</value>
	public virtual bool IsActivated
    {
        get
        {
            return isActivated;
        }

        protected set
        {
            isActivated = value;
        }
    }

    /// <summary>
    /// Activate this instance.
    /// </summary>
    public abstract void Activate();

    /// <summary>
    /// Deactivate this instance.
    /// </summary>
    public abstract void Deactivate();

    protected override string GenerateSaveData()
    {
        string data = base.GenerateSaveData();
        data += PCLParser.CreateAttribute("Starts Active", startActive);
		data += PCLParser.CreateAttribute("Required Activators", requiredActivators);
        return data;
    }

    public override void FromData(TileStruct tile)
    {
		base.FromData(tile);
		StartsActive = PCLParser.ParseBool(tile.NextLine);
		requiredActivators = PCLParser.ParseInt(tile.NextLine);
    }
#if UNITY_EDITOR
    /// <summary>
    /// Draws indicators of the connections between the Activated Object and its connections as well as the activation status of the object
    /// </summary>
    public override void RenderInEditor()
    {
        Handles.color = Color.black;
        Handles.DrawSolidArc(Center, -Vector3.forward, Vector3.up, 360, 0.4f);

        if (startActive)
        {
            Handles.color = Color.green;

        }
        else
        {
            Handles.color = Color.red;
        }
        Handles.DrawSolidArc(Center, -Vector3.forward, Vector3.up, 360, 0.33f);
        Handles.color = Color.white;
    }

    #endif
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:ActivatedObject"/> starts active.
    /// </summary>
    /// <value><c>true</c> if starts active; otherwise, <c>false</c>.</value>
    public virtual bool StartsActive {
        get {
            return startActive;
        }

        set {
            startActive = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:ActivatedObject"/> is connected.
    /// </summary>
    /// <value><c>true</c> if is connected; otherwise, <c>false</c>.</value>
	public bool IsConnected {
		get {
			return isConnected;
		}

		set {
			isConnected = value;
		}
	}

	public void IncrementActivator() {
		currentActivators++;
		if (currentActivators >= requiredActivators && !isActivated) {
			IsActivated = true;
			Activate();
		}
	}

	public void DecrementActivator() {
		currentActivators--;
		if (isActivated && currentActivators < requiredActivators) {
			IsActivated = false;
			Deactivate();
		}
	}
}
