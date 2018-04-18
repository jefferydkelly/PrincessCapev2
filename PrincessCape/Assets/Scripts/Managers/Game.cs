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
    string levelToLoad = "classicLevelOne.json";
    string lastScene = "Test";
    GameState state = GameState.Menu;
	// Use this for initialization
	void Awake () {
        
        if (!instance || instance == this)
        {
            
            isClosing = false;
          
            managers = new List<Manager>();
            if(toAdd != null) {
                managers = toAdd;
            }
            toAdd = new List<Manager>();
            instance = this;
            DontDestroyOnLoad(gameObject);
         
            SceneManager.sceneLoaded += OnSceneLoaded;
            EventManager.StartListening("Pause", () => { IsPaused = !IsPaused; });
            Cutscene.Instance.OnStart.AddListener(()=> {
                state = GameState.Cutscene;
            });

            Cutscene.Instance.OnEnd.AddListener(EndCutscene);

            EventManager.StartListening("ShowDialog", ()=> {
                if (!IsInCutscene)
                {
                    state = GameState.Message;
                }
            });

			EventManager.StartListening("EndOfMessage", () => {
				if (!IsInCutscene)
				{
                    state = GameState.Playing;
				}
			});

            EventManager.StartListening("Inventory", ()=> {
                if (state == GameState.Playing) {
                    state = GameState.Inventory;
                } else if (state == GameState.Inventory) {
                    state = GameState.Playing;
                }
            });
            if (canvas)
            {
                canvas.SetActive(true);
            }

			if (SceneManager.GetActiveScene().name == "Test")
			{
				state = GameState.Playing;
            }

            EventManager.StartListening("LevelLoaded", ()=> {
                state = GameState.Playing;
                AddItems();
            });

        } else {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Clear();
        }
        isClosing = true;
        instance = null;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Test");
        levelToLoad = "classicLevelOne.json";
    }

    // Update is called once per frame
    void Update () {

        if(managers == null) {
            managers = new List<Manager>();
        }
        if (toAdd == null) {
            toAdd = new List<Manager>();
        }
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
            state = sceneName == "Test" ? GameState.Playing : GameState.Menu;
            SceneManager.LoadScene(sceneName);
            Destroy(player);
        }

       
    }

    /// <summary>
    /// Adds items to the Player's inventory to bring it up to the 
    /// </summary>
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
                if (instance == null) {
                    instance = FindObjectOfType<Game>();
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
            player.Init();
            map = FindObjectOfType<Map>();
            if (lastScene != "Test")
            {
                lastScene = SceneManager.GetActiveScene().name;
                LoadScene(levelToLoad);
                return;
            } else {
                AddItems();
            }

        } else {
            state = GameState.Menu;
        }

        lastScene = SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Gets the map.
    /// </summary>
    /// <value>The map.</value>
    public Map Map {
        get {
            return map;
        }
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

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Game"/> is in cutscene.
    /// </summary>
    /// <value><c>true</c> if is in cutscene; otherwise, <c>false</c>.</value>
    public bool IsInCutscene {
        get {
            return state == GameState.Cutscene;
        }
    }

    public bool IsInInventory {
        get {
            return state == GameState.Inventory;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Game"/> is playing.
    /// </summary>
    /// <value><c>true</c> if is playing; otherwise, <c>false</c>.</value>
    public bool IsPlaying {
        get {
            return state == GameState.Playing && !player.IsDead;
        }
    }

    /// <summary>
    /// Handles the transition between cutscene and gameplay
    /// </summary>
    void EndCutscene() {
        state = GameState.Playing;
    }

    /// <summary>
    /// Quit this instance.
    /// </summary>
    public void Quit() {
        Application.Quit();
    }

    /// <summary>
    /// Triggers the event.
    /// </summary>
    /// <param name="eventName">Event name.</param>
    public void TriggerEvent(string eventName) {
        EventManager.TriggerEvent(eventName);
    }
}

public enum GameState {
    Menu,
    Playing,
    Paused,
    Cutscene,
    Message,
    Inventory
}
