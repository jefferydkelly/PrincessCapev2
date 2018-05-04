using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ContainerTile : MapTile
{
    [SerializeField]
    GameObject insideTheContainer;

    // Use this for initialization
    protected override string GenerateSaveData()
    {
        string data = base.GenerateSaveData();
        data += PCLParser.CreateAttribute("Contains", insideTheContainer ? insideTheContainer.name : "Null");
        return data;
    }

    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        string inside = PCLParser.ParseLine(tile.NextLine);
        if (inside != "Null")
        {
            insideTheContainer = Map.Instance.GetPrefabByName(inside);

            if (insideTheContainer)
            {
                SpawnContents();
            }
        }
    }

    private void OnValidate()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(CheckContents());
        }
    }

    IEnumerator CheckContents()
    {
        if (insideTheContainer == null && transform.childCount > 0)
        {
            yield return null;
            DestroyImmediate(transform.GetChild(0).gameObject, false);
        }
        else if (transform.childCount == 0 && insideTheContainer != null)
        {
            SpawnContents();
        }
        else if (insideTheContainer != null && transform.GetChild(0).name != insideTheContainer.name)
        {

            yield return null;
            DestroyImmediate(transform.GetChild(0).gameObject, false);
            SpawnContents();
        }

        MakeContentsClear();
        yield return null;
    }

    void SpawnContents()
    {
        GameObject contents = Instantiate(insideTheContainer);
        contents.transform.SetParent(transform);
        contents.transform.localPosition = Vector3.up;
        contents.name = insideTheContainer.name;

        MakeContentsClear();
    }

    void MakeContentsClear() 
    {
        if (transform.childCount > 0)
        {
            SpriteRenderer spr = transform.GetChild(0).GetComponent<SpriteRenderer>();
            spr.color = spr.color.SetAlpha(0.25f);

        }    
    }

    public override void Init()
    {
        base.Init();

        if (Application.isPlaying)
        {
            if (transform.childCount > 0)
            {
                transform.GetChild(0).localPosition = Vector3.zero;
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
#if UNITY_EDITOR
    public override void RenderInEditor()
    {
        if (transform.childCount > 0)
        {
            Handles.DrawDottedLine(transform.position, transform.position + Vector3.up, 0.25f);
        }
    }
#endif
}
