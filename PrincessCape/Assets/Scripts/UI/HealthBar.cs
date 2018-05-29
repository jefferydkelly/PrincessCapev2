using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {
    [SerializeField]
    GameObject heartPrefab;

	public int MaxHealth {
		get {
			return transform.childCount;
		}

		set {
			if (value > MaxHealth) {
				for (int i = MaxHealth; i < value; i++)
				{
					GameObject heart = Instantiate(heartPrefab);
					heart.transform.SetParent(transform);
					heart.transform.localPosition = Vector3.right * 66 * transform.childCount;
				}
			}
		}
	}

	public int CurrentHealth {
		get {
			int active = 0;
			for (int i = 0; i < transform.childCount; i++) {
				if (transform.GetChild(i).gameObject.activeSelf) {
					active++;
				}
			}
			return active;
		}

		set {
			if (value > CurrentHealth) {
				int heal = Mathf.Min(value, MaxHealth);
				for (int i = CurrentHealth; i < heal; i++) {
					transform.GetChild(i).gameObject.SetActive(true);
				}
			} else if (value < CurrentHealth) {
				int damage = Mathf.Max(0, value);
				for (int i = damage; i < MaxHealth; i++) {
					transform.GetChild(i).gameObject.SetActive(false);
				}
			}
		}
	}
}
