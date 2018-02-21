using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Game : MonoBehaviour {

    static Game instance;
    List<Manager> managers;
    Player player;
    bool paused = false;
    Map map;
    string levelToLoad = "levelOne.json";
	// Use this for initialization
	void Awake () {
        if (!instance)
        {
            managers = new List<Manager>();
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            EventManager.StartListening("Pause", () => { paused = true; });
            EventManager.StartListening("Unpause", () => { paused = false; });
        } else {
            Destroy(gameObject);
        }

    }

    public void StartGame()
    {
        SceneManager.LoadScene("Test");
        levelToLoad = "levelOne.json";
    }

    // Update is called once per frame
    void Update () {
        foreach(Manager m in managers) {
            m.Update(Time.deltaTime);
        }
	}

    /// <summary>
    /// Adds a manager.
    /// </summary>
    /// <param name="m">M.</param>
    public void AddManager(Manager m) {
        managers.Add(m);
    }

    /// <summary>
    /// Reset this game.
    /// </summary>
    public void Reset() {
      
        player.Reset();
    }

    /// <summary>
    /// Loads the scene.
    /// </summary>
    /// <param name="sceneName">Scene name.</param>
    public void LoadScene(string sceneName) {
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
            }
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    /// <summary>
    /// Gets the instance of Game.
    /// </summary>
    /// <value>The instance.</value>
    public static Game Instance {
        get {
            if (instance == null) {
                GameObject go = new GameObject("GameManager");
                go.AddComponent<Game>();
            }
            return instance;
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
            LoadScene(levelToLoad);
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
            return paused;
        }
    }
}
