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
    GameObject actor;
    Animator animator;
    List<string> triggers;
    int selectedTrigger = 0;

    public CutsceneAnimation() {
        canSkip = true;
        autoAdvance = true;
        type = CutsceneElements.Animation;
    }

    public override string SaveData
    {
        get
        {
            string data = "";
            data += PCLParser.CreateAttribute("Character", actor.name);
            data += PCLParser.CreateAttribute("Trigger", triggers[selectedTrigger]);
            return data;
        }
    }

    public override string ToText {
        get {
            return string.Format("animate {0} {1}", actor.name, triggers[selectedTrigger]);
        }
    }

    public override void CreateFromText(string[] data)
    {
        //actor = GameObject.Find(data[1]);
        actorName = data[1];
        if (actor == null)
        {
            actor = FindActor(actorName);
        }

        if (actor)
        {
            animator = actor.GetComponent<Animator>();
            if (animator != null)
            {
                CreateListOfTriggers();
                selectedTrigger = triggers.IndexOf(data[2]);
            }
        }
        triggerName = data[2];
    }

    public override void CreateFromJSON(string[] data)
    {
        actor = GameObject.Find(PCLParser.ParseLine(data[0]));
        animator = actor.GetComponent<Animator>();
        CreateListOfTriggers();
        selectedTrigger = triggers.IndexOf(PCLParser.ParseLine(data[1]));
    }
#if UNITY_EDITOR
    public override void RenderEditor()
    {
        GameObject oldObject =  actor;
        actor = EditorGUILayout.ObjectField("Game Object", actor, typeof(GameObject), true) as GameObject;
        if (actor && actor.activeInHierarchy && oldObject != actor)
        {
            animator = actor.GetComponent<Animator>();
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
#endif
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

    public override Timer Run()
    {
        if (actor == null) {
            actor = FindActor(actorName);
        }

        if (actor)
        {
            animator = actor.GetComponent<Animator>();
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