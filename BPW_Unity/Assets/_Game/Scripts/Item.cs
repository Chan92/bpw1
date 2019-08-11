using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
	public float minDist = 10f;
	PlayerController player;

	private void Start() {
		player = GameObject.FindObjectOfType<PlayerController>();
	}

	//picksup the item
	private void OnMouseOver() {
		if(Input.GetMouseButtonDown(1) && !Menu.gameover) {
			if(Vector3.Distance(transform.position, player.transform.position) <= minDist) {
				player.Pickup(transform);
			}
		}
	}
}
