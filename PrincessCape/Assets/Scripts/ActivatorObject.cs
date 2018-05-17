using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorObject : ActivatedObject {
    List<int> connectionIDs = new List<int>();
    /// <summary>
    /// Activate this instance.
    /// </summary>
    public override void Activate()
    {
        foreach(ActivatedObject ao in connections) {
            if (ao)
            {
                ao.Activate();
            }
        }
    }

    /// <summary>
    /// Deactivate this instance.
    /// </summary>
    public override void Deactivate()
    {
        foreach (ActivatedObject ao in connections)
		{
            if (ao)
            {
                ao.Deactivate();
			}
		}
    }

	protected override string GenerateSaveData()
	{
		string info = base.GenerateSaveData();

		
        List<int> ids = new List<int>();
		foreach (ActivatedObject ao in connections)
		{
			if (ao)
			{
                ids.Add(ao.ID);
			}

		}
        info += PCLParser.CreateArray("Connections", ids);
		return info;
	}

	public override void FromData(TileStruct tile)
	{
		base.FromData(tile);
		connectionIDs = new List<int>();

        if (tile.Peek.Contains("Connections"))
		{
            tile.TossLine();
            while (!tile.FullyRead && !tile.Peek.Contains("]")) {
                connectionIDs.Add(PCLParser.ParseInt(tile.NextLine));
            }
		}
	}

	public void Reconnect()
	{
		Map map = Map.Instance;
		foreach (int i in connectionIDs)
		{
			AddConnection(map.GetTileByID(i).GetComponent<ActivatedObject>());
		}
		connectionIDs.Clear();
	}

	protected int NumConnections
	{
		get
		{
			return Mathf.Max(connectionIDs.Count, connections.Count);
		}
	}
}
