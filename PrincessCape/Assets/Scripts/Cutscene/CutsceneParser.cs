using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneParser
{
    static int elementsStart = 1;

    /// <summary>
    /// Parses the text file and creates a list of cutscene steps containing the elements of the cutscene
    /// </summary>
    /// <returns>The list of cutscene steps containing the elements of the cutscene.</returns>
    /// <param name="text">Text.</param>
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
                if (line.StartsWith("character", System.StringComparison.Ordinal)) {
                    string[] parts = line.Split(' ');
                    for (int j = 1; j < parts.Length; j++) {
                        Cutscene.Instance.CreateCharacter(parts[j]);
                    }
                    continue;
                }
                seq = line.Substring(line.Length - 3) == "and";

                if (seq)
                {
                    line = line.Substring(0, line.Length - 4);
                }

                CutsceneElement e = ParseElement(line);


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

    /// <summary>
    /// Parses the element tpye and creates the corresponding cutscene element.
    /// </summary>
    /// <returns>A new Cutscene Element Editor of the given type.</returns>
    /// <param name="ce">The type of CutsceneElement.</param>
    public static CutsceneElement ParseElement(CutsceneElements ce)
    {
        switch (ce)
        {
            case CutsceneElements.Activate:
                return new CutsceneActivate();
            case CutsceneElements.Add:
                return new CutsceneAdd();
            case CutsceneElements.Align:
                return new CutsceneAlign();
            case CutsceneElements.Animation:
                return new CutsceneAnimation();
            case CutsceneElements.Creation:
                return new CutsceneCreation(false);
            case CutsceneElements.Destroy:
                return new CutsceneCreation(true);
            case CutsceneElements.Dialog:
                return new CutsceneDialog();
            case CutsceneElements.Fade:
                return new CutsceneFade();
            case CutsceneElements.Flip:
                return new CutsceneFlip();
            case CutsceneElements.Follow:
                return new CutsceneFollow();
            case CutsceneElements.Hide:
                return new CutsceneShow(false);
            case CutsceneElements.Movement:
                return new CutsceneMovement();
            case CutsceneElements.Pan:
                return new CutscenePan();
            case CutsceneElements.Play:
                return new CutscenePlay();
            case CutsceneElements.Rotate:
                return new CutsceneRotation();
            case CutsceneElements.Scale:
                return new CutsceneScale();
            case CutsceneElements.Show:
                return new CutsceneShow(true);
            case CutsceneElements.Wait:
                return new CutsceneWait();
            default:
                break;

        }

        return null;
    }

    /// <summary>
    /// Parses a cutscene element editor from the given string
    /// </summary>
    /// <returns>The element.</returns>
    /// <param name="line">Line.</param>
    public static CutsceneElement ParseElement(string line)
    {
        string[] parts = line.Split(' ');

        string p = parts[0];
        CutsceneElement editor;

        if (p == "show")
        {
            editor = new CutsceneShow(true);
        }
        else if (p == "hide")
        {
            editor = new CutsceneShow(false);
        }
        else if (p.Contains("fade") || p == "alpha")
        {
            editor = new CutsceneFade();
        }
        else if (p.Contains("flip"))
        {
            editor = new CutsceneFlip();
        }
        else if (p.Contains("scale"))
        {
            editor = new CutsceneScale();
        }
        else if (p == "rotate")
        {
            editor = new CutsceneRotation();
        }
        else if (p.Contains("move"))
        {
            editor = new CutsceneMovement();
        }
        else if (p == "pan")
        {
            editor = new CutscenePan();
        }
        else if (p == "wait")
        {
            editor = new CutsceneWait();
        }
        else if (p == "create")
        {
            editor = new CutsceneCreation(false);
        }
        else if (p == "destroy")
        {
            editor = new CutsceneCreation(true);
        }
        else if (p == "add")
        {
            editor = new CutsceneAdd();
        }
        else if (p.Contains("able"))
        {
            editor = new CutsceneEnable();
        }
        else if (p == "activate")
        {
            editor = new CutsceneActivate();
        }
        else if (p == "align")
        {
            editor = new CutsceneAlign();
        }
        else if (p == "play")
        {
            editor = new CutscenePlay();
        }
        else if (p == "follow")
        {
            editor = new CutsceneFollow();
        }
        else if (p == "animate")
        {
            editor = new CutsceneAnimation();
        }
        else
        {
            parts = line.Split(':');
            editor = new CutsceneDialog();
        }

        editor.CreateFromText(parts);
        return editor;
    }

    /// <summary>
    /// Parses the given json into a set of structs representing each step of the cutscene
    /// </summary>
    /// <returns>The steps.</returns>
    /// <param name="json">Json.</param>
    public static List<CutsceneStepStruct> ParseSteps(string json)
    {
        List<CutsceneStepStruct> steps = new List<CutsceneStepStruct>();
        string[] lines = json.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (PCLParser.IsLine(line))
            {
                if (PCLParser.ParsePropertyName(line) == "Step Number")
                {
                    CutsceneStepStruct step = new CutsceneStepStruct();
                    string pass = "";
                    while (i < lines.Length - 1)
                    {
                        i++;
                        pass += lines[i] + "\n";
                        if (lines[i] == PCLParser.ArrayEnding)
                        {
                            break;
                        }

                    }

                    step.elements = ParseElements(pass);
                    steps.Add(step);
                }
            }
        }
        return steps;
    }

    /// <summary>
    /// Parses the cutscene file.
    /// </summary>
    /// <returns>The cutscene file.</returns>
    /// <param name="json">Json.</param>
    public static CutsceneFile ParseCutsceneFile(string json)
    {
        string[] lines = json.Split('\n');
        CutsceneFile file = new CutsceneFile();
        file.cutsceneName = PCLParser.ParseLine(lines[1]);
        file.sceneName = PCLParser.ParseLine(lines[2]);
        file.characters = PCLParser.ParseLine(lines[3]).Split(' ');
        file.steps = ParseSteps(json);
        return file;
    }

    /// <summary>
    /// Parses the individual elements
    /// </summary>
    /// <returns>The elements.</returns>
    /// <param name="json">Json.</param>
    public static List<CutsceneElementStruct> ParseElements(string json)
    {
        List<CutsceneElementStruct> elements = new List<CutsceneElementStruct>();

        int ind = json.IndexOf('[');
        int lastInd = PCLParser.FindEndOfArray(json, ind);
        ind += 2;
        json = json.Substring(ind, lastInd - ind);

        string[] tilesList = json.Split('\n');

        for (int i = 0; i < tilesList.Length; i++)
        {
            if (tilesList[i] == "{")
            {
                List<string> toParse = new List<string>();
                int j = i + 1;

                while (j < tilesList.Length && !tilesList[j].Contains("}"))
                {

                    toParse.Add(tilesList[j]);
                    j++;
                }
                elements.Add(ParseCutsceneElementStruct(toParse));
                i = j;
            }
        }

        return elements;
    }

    /// <summary>
    /// Creates a CutsceneElement from the passed in list of strings
    /// </summary>
    /// <returns>The cutscene element struct.</returns>
    /// <param name="json">Json.</param>
    public static CutsceneElementStruct ParseCutsceneElementStruct(List<string> json)
    {
        CutsceneElementStruct element = new CutsceneElementStruct();
        element.type = PCLParser.ParseEnum<CutsceneElements>(json[0]);
        json.RemoveAt(0);
        element.info = json;
        return element;
    }

}
