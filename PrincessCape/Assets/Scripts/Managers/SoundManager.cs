using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	static SoundManager instance;
    AudioSource fxSource;
    AudioSource bgmSource;
    [SerializeField]
    List<AudioClip> soundEffects;
    Dictionary<string, AudioClip> clips;
	// Use this for initialization
	void Awake () {
        instance = this;
        AudioSource[] sources = GetComponents<AudioSource>();
        fxSource = sources[0];
        bgmSource = sources[1];
        Game.Instance.OnGameStateChanged.AddListener(OnGameStateChanged);
        clips = new Dictionary<string, AudioClip>();
        foreach(AudioClip ac in soundEffects) {
            clips.Add(ac.name, ac);
        }
        soundEffects.Clear();
	}

    /// <summary>
    /// Plays the sound.
    /// </summary>
    /// <param name="clip">Clip.</param>
    public void PlaySound(AudioClip clip) {
        if (Game.Instance.IsPlaying || Game.Instance.IsInCutscene)
        {
            fxSource.clip = clip;
            fxSource.Play();
        }
    }

    public float PlaySound(string clipName) {
        if (clips.ContainsKey(clipName)) {
            AudioClip clip = clips[clipName];
            PlaySound(clip);
            return clip.length;
        }

        return 0.0f;
    }

    /// <summary>
    /// Plays the music.
    /// </summary>
    /// <param name="clip">Clip.</param>
    public void PlayMusic(AudioClip clip) {
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public static SoundManager Instance {
        get {
            return instance;
        }
    }

    /// <summary>
    /// Handles the changing of game state
    /// </summary>
    /// <param name="state">State.</param>
    void OnGameStateChanged(GameState state) {
        if (state == GameState.Paused) {
            bgmSource.Pause();
            fxSource.Pause();
        } else if (state == GameState.Playing) {
            bgmSource.UnPause();
            fxSource.UnPause();
        }
    }
}
