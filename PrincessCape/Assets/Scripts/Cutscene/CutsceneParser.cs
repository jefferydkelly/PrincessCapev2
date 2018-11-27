using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneParser
{
    static int elementsStart = 2;
    /// <summary>
    /// Parses the cutscene file.
    /// </summary>
    /// <returns>The cutscene file.</returns>
    /// <param name="json">Json.</param>
    public static CutsceneFile ParseJSONFile(string json)
    {
        string[] lines = json.Split('\n');
        CutsceneFile file = new CutsceneFile();
        file.cutsceneName = PCLParser.ParseLine(lines[1]);
        file.sceneName = PCLParser.ParseLine(lines[2]);
        file.characters = PCLParser.ParseLine(lines[3]).Trim().Split(' ');
        foreach(string character in file.characters) {
            Cutscene.Instance.AddActor(character);
        }
        file.steps = ParseStepsFromJSON(json);
        return file;
    }

    public static CutsceneFile ParseTextFile(TextAsset text)
    {
        string[] lines = text.text.Split('\n');
        CutsceneFile file = new CutsceneFile();
        file.cutsceneName = text.name.SplitCamelCase();
        file.sceneName = lines[0];
        string[] characters = lines[1].Split(' ');
        file.characters = new string[characters.Length - 1];
        for (int i = 1; i < characters.Length; i++) {
            file.characters[i - 1] = characters[i];
            Cutscene.Instance.AddActor(characters[i]);
        }


        file.steps = ParseStepsFromText(text.text);
        return file;
    }

    public static CutsceneFile ParseTextFile(string path) {
        return ParseTextFile(Resources.Load<TextAsset>(path));
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
            case CutsceneElements.Enable:
                return new CutsceneEnable();
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
    public static List<CutsceneStep> ParseStepsFromJSON(string json)
    {
        List<CutsceneStep> steps = new List<CutsceneStep>();
        string[] lines = json.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (PCLParser.IsLine(line))
            {
                if (PCLParser.ParsePropertyName(line) == "Step Number")
                {
                    CutsceneStep step = new CutsceneStep(PCLParser.ParseInt(line));
                    string pass = "";
                    while (i < lines.Length - 1)
                    {
                        i++;
                        line = lines[i].Trim() + "\n";;
                        pass += line;
                        if (line.Contains(PCLParser.ArrayEnding)) {
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

    public static List<CutsceneStep> ParseStepsFromText(string text) {
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
    /// Parses the individual elements
    /// </summary>
    /// <returns>The elements.</returns>
    /// <param name="json">Json.</param>
    public static List<CutsceneElement> ParseElements(string json)
    {
        List<CutsceneElement> elements = new List<CutsceneElement>();

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

                int j = i + 2;
                CutsceneElements type = PCLParser.ParseEnum<CutsceneElements>(tilesList[i + 1]);

                CutsceneElement element = ParseElement(type);

                while (j < tilesList.Length && !tilesList[j].Contains("}"))
                {

                    toParse.Add(tilesList[j]);
                    j++;
                }

                element.CreateFromJSON(toParse.ToArray());
                elements.Add(element);

                i = j;
            }
        }

        return elements;
    }
}
