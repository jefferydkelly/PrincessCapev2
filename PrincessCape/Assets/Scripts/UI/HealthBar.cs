using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {
    [SerializeField]
    GameObject heartPrefab;
	// Use this for initialization
	void Awake () {
        EventManager.StartListening("TakeDamage", UpdateHealth);
        EventManager.StartListening("PlayerRespawned", UpdateHealth);
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
        } else {
			for (int i = transform.childCount; i > Game.Instance.Player.CurrentHealth; i--)
			{
                Destroy(transform.GetChild(i).gameObject);
			}
        }
    }
}
