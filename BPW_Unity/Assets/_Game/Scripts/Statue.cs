using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour {
	public float minDist = 10f;
	PlayerController player;
	WorldManager worldManager;

	private void Start() {
		player = GameObject.FindObjectOfType<PlayerController>();
		worldManager = GameObject.FindObjectOfType<WorldManager>();
	}

	//drops the item, giving it to the statue as offering
	private void OnMouseOver() {
		if(Input.GetMouseButtonDown(1) && !Menu.gameover) {
			if(Vector3.Distance(transform.position, player.transform.position) <= minDist) {
				player.OfferItem();
			}
		}
	}

	//respawn item to a new location when the item is spawned on/in the statue
	private void OnCollisionEnter(Collision collision) {
		if(collision.transform.tag == "Item") {
			collision.transform.localPosition = worldManager.RandomPos();
		}
	}
}
