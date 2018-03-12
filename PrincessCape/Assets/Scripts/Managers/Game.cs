using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Game : MonoBehaviour {

    public static bool isClosing = false;
    static Game instance = null;

    [SerializeField]
    GameObject canvas;
    List<Manager> managers;
    List<Manager> toAdd;
    Player player;
    Map map;
    string levelToLoad = "levelOne.json";
    string lastScene = "Test";
    GameState state = GameState.Menu;
	// Use this for initialization
	void Awake () {
        
        if (!instance || instance == this)
        {
            isClosing = false;
            managers = new List<Manager>();
            toAdd = new List<Manager>();
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            EventManager.StartListening("Pause", () => { IsPaused = true; });
            EventManager.StartListening("Unpause", () => { IsPaused = false; });
            EventManager.StartListening("StartCutscene", ()=>{
                state = GameState.Cutscene;
            });

			EventManager.StartListening("EndCutscene", () => {
                state = GameState.Playing;
			});
            if (canvas)
            {
                canvas.SetActive(true);
            }

			if (SceneManager.GetActiveScene().name == "Test")
			{
				state = GameState.Playing;
			}

        } else {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        EventManager.Instance.Clear();
        isClosing = true;
        instance = null;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Test");
        levelToLoad = "levelOne.json";
    }

    // Update is called once per frame
    void Update () {
        foreach(Manager m in toAdd) {
            managers.Add(m);
        }
        toAdd.Clear();
         foreach(Manager m in managers) {
            m.Update(Time.deltaTime);
        }
	}

    /// <summary>
    /// Adds a manager.
    /// </summary>
    /// <param name="m">M.</param>
    public void AddManager(Manager m) {
        if (toAdd == null)
        {
            toAdd = new List<Manager>();
        }
        toAdd.Add(m);
    }

    /// <summary>
    /// Reset this game.
    /// </summary>
    public void Reset() {
        if (player)
        {
            player.Reset();
        }
    }

    /// <summary>
    /// Loads the scene.
    /// </summary>
    /// <param name="sceneName">Scene name.</param>
    public void LoadScene(string sceneName)
    {
     
        if (sceneName.Length > 6 && sceneName.Substring(sceneName.Length - 5) == ".json")
        {
            if (SceneManager.GetActiveScene().name != "Test")
            {
                levelToLoad = sceneName;
                SceneManager.LoadScene("Test");
            }
            else
            {
                //Debug.Log("Load the next level");
                //Clear the map and load the next scene before starting the next level
                map.Clear();

                map.Load(sceneName);
                AddItems();

            }
        }
        else
        {
            state = GameState.Playing;
            SceneManager.LoadScene(sceneName);
        }

       
    }

    public void AddItems() {
		for (int i = (int)player.Items + 1; i <= (int)map.Items; i++)
		{
			string itemName = ((ItemLevel)(i)).ToString();
			player.AddItem(ScriptableObject.CreateInstance(itemName) as MagicItem);
		}
    }

    /// <summary>
    /// Gets the instance of Game.
    /// </summary>
    /// <value>The instance.</value>
    public static Game Instance {
        get {
            if (!isClosing)
            {
                if (!instance)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<Game>();

                }
                return instance;
            }

            return null;
        }
    }

    /// <summary>
    /// Event called when a new scene is loaded in.
    /// </summary>
    /// <param name="newScene">New scene.</param>
    /// <param name="lsm">Lsm.</param>
    void OnSceneLoaded(Scene newScene, LoadSceneMode lsm) {
       
        if (newScene.name == "Test")
        {
            player = FindObjectOfType<Player>();
            map = FindObjectOfType<Map>();
            //player.transform.position = Checkpoint.ResetPosition;
            if (lastScene != "Test")
            {
                LoadScene(levelToLoad);
            } else {
				EventManager.TriggerEvent("LevelLoaded");
                AddItems();
            }
        }

        lastScene = SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Gets the player.
    /// </summary>
    /// <value>The player.</value>
    public Player Player {
        get {
            return player;
        }
    }

    /// <summary>
    /// Sets a value indicating whether this <see cref="T:Game"/> is paused.
    /// </summary>
    /// <value><c>true</c> if is paused; otherwise, <c>false</c>.</value>
    public bool IsPaused {
        get {
            return state == GameState.Paused;
        }

        set {
            if (value && state == GameState.Playing) {
                state = GameState.Paused;
            } else if (!value && state == GameState.Paused) {
                state = GameState.Playing;
            }
        }
    }

    public bool IsInCutscene {
        get {
            return state == GameState.Cutscene;
        }
    }

    public void StartCutscene() {
        
    }

    void EndCutscene() {
        state = GameState.Playing;
    }
}

public enum GameState {
    Menu,
    Playing,
    Paused,
    Cutscene
}
