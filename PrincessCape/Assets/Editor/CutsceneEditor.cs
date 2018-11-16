using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.Events;
public class CutsceneEditor : EditorWindow {

    private static CutsceneEditor instance;
    Cutscene cutscene;
    List<CutsceneActor> actors;
    Vector2 scrollPos;

    UnityEvent onClearPreview;

    [MenuItem("My Game/Cutscene Editor")]
    public static void ShowWindow() {
        GetWindow<CutsceneEditor>(false, "Cutscene Editor", true);
       
    }

    private void OnEnable()
    {
        if (EditorSceneManager.GetActiveScene().name != "Test")
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Test.unity");
        }
        instance = this;//GetWindow<CutsceneEditor>(false, "Cutscene Editor", true);
        //onClearPreview = new UnityEvent();

        scrollPos = Vector2.zero;
        cutscene = new Cutscene();
       



    }
    private void OnDestroy()
    {
        if (instance)
        {
            UIManager.Instance.Clear();

            foreach (string character in cutscene.Characters)
            {
                cutscene.RemoveActor(character);
            }

            DestroyImmediate(cutscene.GameObject);
        }
    }

    private void OnGUI()
    {
        if (instance != null)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save as JSON")) {
                //Save the cutscene
                string path = EditorUtility.SaveFilePanel("Save Cutscene To File", "Assets/Resources/Cutscenes", cutscene.Name.JoinCamelCase(), "json");
                if (path.Length > 0)
                {
                    File.WriteAllText(path, cutscene.ToJSON);
                }

            } else if (GUILayout.Button("Save as Text"))
            {
                //Save the cutscene
                string path = EditorUtility.SaveFilePanel("Save Cutscene To File", "Assets/Resources/Cutscenes", cutscene.Name.JoinCamelCase(), "txt");

                if (path.Length > 0)
                {
                    File.WriteAllText(path, cutscene.ToText);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load JSON"))
            {
                string path = EditorUtility.OpenFilePanel("Open A Level File", "Assets/Resources/Cutscenes", "json");
                if (path.Length > 0)
                {
                    foreach(string character in cutscene.Characters) {
                        cutscene.RemoveActor(character);
                    }
                    CutsceneFile file = CutsceneParser.ParseJSONFile(File.ReadAllText(path));
                    cutscene.Info = file;

                }
            } else if (GUILayout.Button("Load Text"))
            {
                string path = EditorUtility.OpenFilePanel("Open A Level File", "Assets/Resources/Cutscenes", "txt");
                if (path.Length > 0) {
                    //cutscene.Steps.Clear();
                    foreach (string character in cutscene.Characters)
                    {
                        cutscene.RemoveActor(character);
                    }
             
                    path = path.SplitAfter("Resources/").SplitBefore(".txt");
                    CutsceneFile file = CutsceneParser.ParseTextFile(path);

                    cutscene.Info = file;
                }
            }


            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            if (GUILayout.Button("Preview Cutscene")) {
                //Preview Cutscene
            }
            cutscene.Render();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach(CutsceneStep step in cutscene.Steps) {
                step.DrawGUI();
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Add Step"))
            {
                instance.cutscene.Steps.Add(new CutsceneStep(Cutscene.Steps.Count - 1));
            }
        }
    }

    public static CutsceneEditor Instance {
        get {
            return instance;
        }
    }



    public Cutscene Cutscene {
        get {
            return Cutscene;
        }
    }

}
