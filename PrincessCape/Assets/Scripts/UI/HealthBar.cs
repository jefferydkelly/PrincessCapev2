using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {
    [SerializeField]
    GameObject heartPrefab;
	// Use this for initialization
	void Awake () {
        EventManager.StartListening("TakeDamage", TakeDamage);
        EventManager.StartListening("RestoreHealth", RestoreHealth);
        EventManager.StartListening("PlayerRespawned", FullRestore);
        EventManager.StartListening("IncreaseHealth", AddNewHeart);
		Game.Instance.OnReady.AddListener(() =>
		{
			for (int i = 0; i < 3; i++) {
				AddNewHeart();
			}
		});
	}

    private void UpdateHealth()
    {
        if (Game.Instance.Player.CurrentHealth > transform.childCount) {
            Debug.Log(Game.Instance.Player.CurrentHealth);
            for (int i = transform.childCount; i < Game.Instance.Player.CurrentHealth; i++)
            {
                GameObject heart = Instantiate(heartPrefab);
                heart.transform.SetParent(transform);
                heart.transform.localPosition = Vector3.right * 66 * i;
            }
        }
    }

    void RestoreHealth() {
        for (int i = 0; i < Game.Instance.Player.CurrentHealth; i++) {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
    void TakeDamage() {
		for (int i = transform.childCount; i > Game.Instance.Player.CurrentHealth; i--)
		{
            transform.GetChild(i-1).gameObject.SetActive(false);
		}
    }
    void AddNewHeart() {
        FullRestore();
		GameObject heart = Instantiate(heartPrefab);
        heart.transform.SetParent(transform);
		heart.transform.localPosition = Vector3.right * 66 * transform.childCount;
    }

    void FullRestore() {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
