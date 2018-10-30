using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneAnimation : CutsceneElement
{
    string triggerName;
    string actorName;

    public CutsceneAnimation(string aName, string tName)
    {
        triggerName = tName;
        actorName = aName;
    }

    public override Timer Run()
    {
        GameObject gameObject = GameObject.Find(actorName);
        if (gameObject)
        {
            Animator animator = gameObject.GetComponent<Animator>();
            if (animator)
            {
                animator.SetTrigger(triggerName);
                runTimer = new Timer(0.5f);
                return runTimer;
            }
        }
        else
        {
            CutsceneActor cutsceneActor = Cutscene.Instance.FindActor(actorName);
            if (cutsceneActor)
            {
                cutsceneActor.Animate(triggerName);
                runTimer = new Timer(0.5f);
                return runTimer;
            }
        }
        return null;
    }
}

#if UNITY_EDITOR
public class AnimationEditor : CutsceneElementEditor
{
    GameObject gameObject;
    Animator animator;
    List<string> triggers;
    int selectedTrigger = 0;

    public AnimationEditor()
    {
        editorType = "Play Animation";
        type = CutsceneElements.Animation;
    }

    /// <summary>
    /// Draws the GUI for the properties of this object and handles any changes
    /// </summary>
    protected override void DrawGUI()
    {
        //objectName = EditorGUILayout.TextField("Animated Object", objectName);
        //animation = EditorGUILayout.TextField("Trigger Name", animation);

        GameObject oldObject = gameObject;
        gameObject = EditorGUILayout.ObjectField("Game Object", gameObject, typeof(GameObject), true) as GameObject;
        if (gameObject && gameObject.activeInHierarchy && oldObject != gameObject)
        {
            animator = gameObject.GetComponent<Animator>();
            triggers = new List<string>();
            selectedTrigger = 0;
            CreateListOfTriggers();
        }


        if (animator)
        {
            selectedTrigger = EditorGUILayout.Popup("Trigger", selectedTrigger, triggers.ToArray());
        }
        else
        {
            EditorGUILayout.LabelField("This does not have an animator");
        }

    }

    void CreateListOfTriggers()
    {
        if (animator)
        {
            triggers = new List<string>();
            //Make a list of the animations and list them to be selected
            foreach (AnimatorControllerParameter acp in animator.parameters)
            {
                triggers.Add(acp.name);
            }
        }
    }

    public override void GenerateFromData(string[] data)
    {
        gameObject = GameObject.Find(PCLParser.ParseLine(data[0]));
        animator = gameObject.GetComponent<Animator>();
        CreateListOfTriggers();
        selectedTrigger = triggers.IndexOf(PCLParser.ParseLine(data[1]));
        //objectName = PCLParser.ParseLine(data[0]);
        //animation = PCLParser.ParseLine(data[1]);
    }

    public override string GenerateSaveData()
    {
        string data = "";
        data += PCLParser.CreateAttribute("Character", gameObject.name);
        data += PCLParser.CreateAttribute("Trigger", triggers[selectedTrigger]);
        return data;
    }

    public override string HumanReadable
    {
        get
        {
            return string.Format("animate {0} {1}", gameObject.name, triggers[selectedTrigger]);
        }
    }

    public override void GenerateFromText(string[] data)
    {
        gameObject = GameObject.Find(data[1]);
        if (gameObject == null) {
            gameObject = FindActor(data[1]);
        }

        if (gameObject)
        {
            animator = gameObject.GetComponent<Animator>();
            if (animator != null)
            {
                CreateListOfTriggers();
                selectedTrigger = triggers.IndexOf(data[2]);
            }
        }
    }

    public override void Skip()
    {
        if (animator != null)
        {
            animator.SetTrigger(triggers[selectedTrigger]);
        }
    }
}
#endif