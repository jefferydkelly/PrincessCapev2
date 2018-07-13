using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
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
    GameState gameState = GameState.None;
    ScreenState screenState = ScreenState.Menu;

	UnityEvent onReady = new UnityEvent();
    UnityEvent onEditorPlay = new UnityEvent();
    UnityEvent onEditorPause = new UnityEvent();
    UnityEvent onEditorResume = new UnityEvent();
    UnityEvent onEditorStop = new UnityEvent();

    bool alreadyPaused = false;
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
          
            Controller.Instance.OnPause.AddListener(() =>
            {
                if (!IsInLevelEditor)
                {
                    IsPaused = !IsPaused;
                } else if (!alreadyPaused)
                {
                    alreadyPaused = true;
                    PauseInEditor();
                }
            });
            
         
            Cutscene.Instance.OnStart.AddListener(()=> {
                gameState = GameState.Cutscene;
            });

			Cutscene.Instance.OnEnd.AddListener(EndCutscene);

            EventManager.StartListening("Inventory", ()=> {
                if (gameState == GameState.Playing) {
                    gameState = GameState.Inventory;
                } else if (gameState == GameState.Inventory) {
                    gameState = GameState.Playing;
                }
            });
            if (canvas)
            {
                canvas.SetActive(true);
			}

			if (SceneManager.GetActiveScene().name == "Test")
			{
                gameState = GameState.Playing;
                map = FindObjectOfType<Map>();
				map.OnLevelLoaded.AddListener(() =>
				{

					UIManager.Instance.OnMessageStart.AddListener(() => {
                        if (!IsInCutscene)
                        {
                            gameState = GameState.Message;
                        }
                    });


                    UIManager.Instance.OnMessageEnd.AddListener(() => {
                        if (!IsInCutscene)
                        {
                            gameState = GameState.Playing;
                        }
                    });
                    gameState = GameState.Playing;
					AddItems();
				});
            }


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

    public void PlayInEditor() {
     
        if (IsInLevelEditor) {
            
            if (IsPaused) {
                PauseInEditor();
            } else if (!IsPlaying)
            {
                gameState = GameState.Playing;
                Map.Instance.PlayInEditor();
                UIManager.Instance.IsHidden = false;
                LevelEditor.Instance.IsHidden = true;
                OnEditorPlay.Invoke();
                player.Inventory.Clear();
                AddItems();

            } else {
                StopInEditor();

            }
            Reset();
        }
    }

    public void StopInEditor() {
        gameState = GameState.None;
        LevelEditor.Instance.IsHidden = false;
        UIManager.Instance.IsHidden = true;
        OnEditorStop.Invoke();
        player.transform.position = Checkpoint.ResetPosition;
        map.Reload();
    }
    public void PauseInEditor() {
        if (IsInLevelEditor) {
            if (IsPlaying)
            {
                gameState = GameState.Paused;
                LevelEditor.Instance.IsHidden = false;
                UIManager.Instance.IsHidden = true;
                OnEditorPause.Invoke();

            } else {
                gameState = GameState.Playing;
                LevelEditor.Instance.IsHidden = true;
                UIManager.Instance.IsHidden = false;
                OnEditorPlay.Invoke();
            }
            if (!alreadyPaused)
            {
                alreadyPaused = true;
                Controller.Instance.OnPause.Invoke();
            }


        }
    }
    // Update is called once per frame
    void Update () {

        if (screenState !=ScreenState.Menu)
		{
			if (managers == null)
			{
				managers = new List<Manager>();
			}
			if (toAdd == null)
			{
				toAdd = new List<Manager>();
			}
			foreach (Manager m in toAdd)
			{
				managers.Add(m);
			}
			toAdd.Clear();
			foreach (Manager m in managers)
			{
				m.Update(Time.deltaTime);
			}
		}

        alreadyPaused = false;
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
				//Clear the map and load the next scene before starting the next level
				player.IsFrozen = true;
                map.Clear();
                map.Load(sceneName);

                AddItems();

				player.IsFrozen = false;


                onReady.Invoke();
				onReady.RemoveAllListeners();
                gameState = GameState.Playing;
            }

        }
        else
        {
            screenState = sceneName == "Test" ? ScreenState.Level : ScreenState.Menu;
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
            screenState = ScreenState.Level;
            player = FindObjectOfType<Player>();
            player.Init();
            map = FindObjectOfType<Map>();
			

            if (lastScene != "Test")
            {
                /*
				map.OnLevelLoaded.AddListener(() =>
				{
				    //gameState = GameState.Playing;
					AddItems();
				});*/
				
                lastScene = SceneManager.GetActiveScene().name;
                LoadScene(levelToLoad);
                return;
            } else {
                AddItems();
            }

        } else if (newScene.name == "LevelEditor") {
            screenState = ScreenState.Editor;
            player = FindObjectOfType<Player>();
            player.Init();
            map = FindObjectOfType<Map>();

        }else {
            screenState = ScreenState.Menu;
        }

        lastScene = SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Gets the map.
    /// </summary>
    /// <value>The map.</value>
    public Map Map {
        get {
            if (map == null) {
				map = FindObjectOfType<Map>();
            }
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
            return gameState == GameState.Paused;
        }

        set {
            if (value && gameState == GameState.Playing) {
                gameState = GameState.Paused;
            } else if (!value && gameState == GameState.Paused) {
                gameState = GameState.Playing;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Game"/> is in cutscene.
    /// </summary>
    /// <value><c>true</c> if is in cutscene; otherwise, <c>false</c>.</value>
    public bool IsInCutscene {
        get {
            return gameState == GameState.Cutscene;
        }
    }

    public bool IsInInventory {
        get {
            return gameState == GameState.Inventory;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Game"/> is playing.
    /// </summary>
    /// <value><c>true</c> if is playing; otherwise, <c>false</c>.</value>
    public bool IsPlaying {
        get {
            return gameState == GameState.Playing && !player.IsDead;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Game"/> is in level editor.
    /// </summary>
    /// <value><c>true</c> if is in level editor; otherwise, <c>false</c>.</value>
    public bool IsInLevelEditor {
        get {
            return screenState == ScreenState.Editor;
        }
    }

    /// <summary>
    /// Handles the transition between cutscene and gameplay
    /// </summary>
    void EndCutscene() {
        gameState = GameState.Playing;
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

	public UnityEvent OnReady {
		get {
			return onReady;
		}
	}

    public UnityEvent OnEditorPlay {
        get {
            return onEditorPlay;
        }
    }

    public UnityEvent OnEditorPause
    {
        get
        {
            return onEditorPause;
        }
    }

    public UnityEvent OnEditorStop {
        get {
            return onEditorStop;
        }
    }

    public UnityEvent OnEditorResume
    {
        get
        {
            return onEditorResume;
        }
    }
}

public enum GameState {
    None,
    Playing,
    Paused,
    Cutscene,
    Message,
    Inventory
}

public enum ScreenState {
    Menu,
    Level,
    GameOver,
    Editor
}
