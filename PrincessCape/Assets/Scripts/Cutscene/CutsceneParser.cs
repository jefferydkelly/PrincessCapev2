using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneParser
{
    static int elementsStart = 1;
    public static List<CutsceneStep> ParseTextFile(string text)
    {
        List<CutsceneStep> steps = new List<CutsceneStep>();
        string[] lines = text.Split('\n');

        for (int i = elementsStart; i < lines.Length; i++)
        {


            CutsceneStep step = new CutsceneStep();
            step.elements = new List<CutsceneElement>();
            bool seq = false;

            do
            {
                string line = lines[i].Trim();
                seq = line.Substring(line.Length - 3) == "and";

                if (seq)
                {
                    line = line.Substring(0, line.Length - 4);
                }

                CutsceneElement e = ParseFromText(line);


                if (e != null)
                {
                    step.AddElement(e);
                    if (seq)
                    {
                        i++;
                    }
                }

                //seq = false;
            } while (seq && i < lines.Length);


            if (step.NumElements > 0)
            {
                steps.Add(step);
            }
        }

        return steps;
    }
    public static CutsceneElement ParseFromText(string line)
    {
        string[] parts = line.Split(' ');
        string p = parts[0].ToLower();

        if (p == "show")
        {
            if (parts.Length == 2)
            {
                return new CutsceneShow(parts[1], true);
            }
            else
            {
                return new CutsceneShow(parts[1], true, new Vector3(float.Parse(parts[2]), float.Parse(parts[3])));
            }
        }
        else if (p == "hide")
        {
            return new CutsceneShow(parts[1], false);
        }
        else if (p == "fade-in")
        {
            return new CutsceneFade(parts[1], 1.0f, float.Parse(parts[2]));
        }
        else if (p == "fade-out")
        {
            return new CutsceneFade(parts[1], 0.0f, float.Parse(parts[2]));
        }
        else if (p == "fade")
        {
            return new CutsceneFade(parts[1], float.Parse(parts[2]), float.Parse(parts[3]));
        }
        else if (p == "alpha")
        {
            return new CutsceneFade(parts[1], float.Parse(parts[2]), 0);
        }
        else if (p == "flip-x")
        {
            return new CutsceneFlip(parts[1], true);
        }
        else if (p == "flip-y")
        {
            return new CutsceneFlip(parts[1], false);
        }
        else if (p == "scale")
        {
            return new CutsceneScale(ScaleType.All, parts[1], float.Parse(parts[2]), parts.Length == 4 ? float.Parse(parts[3]) : 0);
        }
        else if (p == "scalex")
        {
            return new CutsceneScale(ScaleType.X, parts[1], float.Parse(parts[2]), float.Parse(parts[3]));
        }
        else if (p == "scaley")
        {
            return new CutsceneScale(ScaleType.Y, parts[1], float.Parse(parts[2]), float.Parse(parts[3]));
        }
        else if (p == "scalexy")
        {
            return new CutsceneScale(parts[1], float.Parse(parts[2]), float.Parse(parts[3]), float.Parse(parts[4]));
        }
        else if (p == "rotate")
        {
            return new CutsceneRotation(parts[1], float.Parse(parts[2]), parts.Length == 4 ? float.Parse(parts[3]) : 0);
        }
        else if (p == "move")
        {
            return new CutsceneMovement(parts[1], float.Parse(parts[2]), float.Parse(parts[3]), parts.Length == 5 ? float.Parse(parts[4]) : 0);
        }
        else if (p == "move-x")
        {
            return new CutsceneMovement(parts[1], float.Parse(parts[2]), float.PositiveInfinity, parts.Length == 4 ? float.Parse(parts[3]) : 0);
        }
        else if (p == "move-y")
        {
            return new CutsceneMovement(parts[1], float.PositiveInfinity, float.Parse(parts[2]), parts.Length == 4 ? float.Parse(parts[3]) : 0);
        }
        else if (p == "pan")
        {
            if (parts[1] == "to")
            {
                if (parts.Length == 4)
                {
                    float panTime = float.Parse(parts[3]);

                    return new CutscenePan(parts[2], panTime);
                }
                else
                {
                    float panTime = float.Parse(parts[4]);

                    return new CutscenePan(new Vector3(float.Parse(parts[2]), float.Parse(parts[3]), Camera.main.transform.position.z), panTime);
                }
            }
            else
            {
                Vector2 dif = new Vector2(float.Parse(parts[1]), float.Parse(parts[2]));
                float time = float.Parse(parts[3]);
                Debug.Log(string.Format("Panning {0} over {1}", dif, time));
                return new CutscenePan(dif,time);
            }
        }
        else if (p == "wait")
        {
            return new CutsceneWait(float.Parse(parts[1]));
        }
        else if (p == "create")
        {
            return new CutsceneCreation(parts[1], parts[2], parts[3], parts.Length == 5 ? parts[4] : "0");
        }
        else if (p == "destroy")
        {
            return new CutsceneCreation(parts[1].Trim());
        }
        else if (p == "add")
        {
            return new CutsceneAdd(parts[1].Trim());
        }
        else if (p == "disable")
        {

            string objectName = parts[1].Trim();

            GameObject go = Cutscene.Instance.FindGameObject(objectName);

            if (go)
            {
                return new CutsceneEnable(go, false);
            }
            else
            {
                return new CutsceneEnable(objectName, false);
            }
        }
        else if (p == "enable")
        {
            string objectName = parts[1].Trim();

            GameObject go = Cutscene.Instance.FindGameObject(objectName);

            if (go)
            {
                return new CutsceneEnable(go, true);
            }
            else
            {
                return new CutsceneEnable(objectName, true);
            }
        }
        else if (p == "activate")
        {
            GameObject go = GameObject.Find(parts[1].Trim());
            if (go != null)
            {
                ActivatedObject ao = go.GetComponent<ActivatedObject>();
                if (ao != null)
                {
                    return new CutsceneActivate(ao, parts[2].Trim() == "true");
                }
            }

            return new CutsceneActivate(parts[1].Trim(), parts[2].Trim() == "true");
        }
        else if (p == "align")
        {
            return new CutsceneAlign(parts[1].Trim() == "left");
        }
        else if (p == "play")
        {
            return new CutscenePlay(parts[1].Trim());
        }
        else if (p == "follow")
        {
            return new CameraFollow(parts[1].Trim());
        }
        else if (p == "goto")
        {
            return new SceneChange(parts[1].Trim());
        }
        else if (p == "animate")
        {
            return new CutsceneAnimation(parts[1].Trim(), parts[2].Trim());
        }
        else if (p == "character")
        {
            for (int i = 1; i < parts.Length; i++)
            {
                Cutscene.Instance.CreateCharacter(parts[i].Trim());
            }

        }
        else
        {
            parts = line.Split(':');

            if (parts.Length == 2)
            {
                return new CutsceneDialog(parts[0], parts[1].Trim());
            }
            else
            {
                return new CutsceneDialog(parts[0].Trim());
            }
        }

        return null;

    }

   

#if UNITY_EDITOR
    /// <summary>
    /// Parses the element tpye and creates the corresponding cutscene element.
    /// </summary>
    /// <returns>A new Cutscene Element Editor of the given type.</returns>
    /// <param name="ce">The type of CutsceneElement.</param>
    public static CutsceneElementEditor ParseElement(CutsceneElements ce)
    {
        switch (ce)
        {
            case CutsceneElements.Activate:
                return new ActivateEditor();
            case CutsceneElements.Add:
                return new AddEditor();
            case CutsceneElements.Align:
                return new AlignmentEditor();
            case CutsceneElements.Animation:
                return new AnimationEditor();
            case CutsceneElements.Creation:
                return new CreationEditor();
            case CutsceneElements.Destroy:
                return new DestructionEditor();
            case CutsceneElements.Dialog:
                return new DialogEditor();
            case CutsceneElements.Fade:
                return new FadeEditor();
            case CutsceneElements.Flip:
                return new FlipEditor();
            case CutsceneElements.Follow:
                return new FollowEditor();
            case CutsceneElements.Hide:
                return new HideEditor();
            case CutsceneElements.Movement:
                return new MovementEditor();
            case CutsceneElements.Pan:
                return new PanEditor();
            case CutsceneElements.Play:
                return new PlayEditor();
            case CutsceneElements.Rotate:
                return new RotationEditor();
            case CutsceneElements.Scale:
                return new ScaleEditor();
            case CutsceneElements.Show:
                return new ScaleEditor();
            case CutsceneElements.Wait:
                return new WaitEditor();
            default:
                break;

        }

        return null;
    }

    public static CutsceneElementEditor ParseElement(string line)
    {
        string[] parts = line.Split(' ');

        string p = parts[0];
        CutsceneElementEditor editor;

        if (p == "show")
        {
            editor = new ShowEditor();
        }
        else if (p == "hide")
        {
            editor = new HideEditor();
        }
        else if (p.Contains("fade") || p == "alpha")
        {
            editor = new FadeEditor();
        }
        else if (p.Contains("flip"))
        {
            editor = new FlipEditor();
        }
        else if (p.Contains("scale"))
        {
            editor = new ScaleEditor();
        }
        else if (p == "rotate")
        {
            editor = new RotationEditor();
        }
        else if (p.Contains("move"))
        {
            editor = new MovementEditor();
        }
        else if (p == "pan")
        {
            editor = new PanEditor();
        }
        else if (p == "wait")
        {
            editor = new WaitEditor();
        }
        else if (p == "create")
        {
            editor = new CreationEditor();
        }
        else if (p == "destroy")
        {
            editor = new DestructionEditor();
        }
        else if (p == "add")
        {
            editor = new AddEditor();
        }
        else if (p.Contains("able"))
        {
            editor = new EnableEditor();
        }
        else if (p == "activate")
        {
            editor = new ActivateEditor();
        }
        else if (p == "align")
        {
            editor = new AlignmentEditor();
        }
        else if (p == "play")
        {
            editor = new PlayEditor();
        }
        else if (p == "follow")
        {
            editor = new FollowEditor();
        }
        else if (p == "animate")
        {
            editor = new AnimationEditor();
        }
        else
        {
            parts = line.Split(':');
            editor = new DialogEditor();
        }

        editor.GenerateFromText(parts);
        return editor;
    }

#endif
}
