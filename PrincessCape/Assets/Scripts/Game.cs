using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Game : MonoBehaviour {

    static Game instance;
    List<Manager> managers;
    Player player;
	// Use this for initialization
	void Awake () {
        managers = new List<Manager>();


        SceneManager.sceneLoaded += OnSceneLoaded;
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
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Gets the instance of Game.
    /// </summary>
    /// <value>The instance.</value>
    public static Game Instance {
        get {
            if (instance == null) {
                GameObject go = new GameObject("GameManager");
                instance = go.AddComponent<Game>();
                DontDestroyOnLoad(go);
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
        player = FindObjectOfType<Player>();
        player.transform.position = Checkpoint.ResetPosition;
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

}
