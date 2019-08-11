using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	[HideInInspector]
	public float strength;

	private void Start() {
		Destroy(gameObject, 5f);
	}

	private void OnTriggerEnter(Collider other) {
		if(other.tag == "Flooffy") {
			other.GetComponent<EnemyController>().GetDmg(strength);
			Destroy(gameObject);
		}
	}
}
