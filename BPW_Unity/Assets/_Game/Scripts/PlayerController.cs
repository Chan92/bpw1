using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour{
	[Header("Movements")]
	public Transform cam;
	public float walkSpeed;
	public float runSpeed;
	public float rotateSpeedH;
	public float rotateSpeedV;
	public KeyCode runKey = KeyCode.LeftShift;	

	[Header("Attacks")]
	public float attackSpeed;
	public float attackDelay;
	public float attackStrength;

	[Header("Other")]
	public WorldManager worldManager;
	public AudioSource soundObj;
	public Transform hand;	
	public AudioClip offerSound;
	private bool hasPickup = false;

	void Update() {
		if(Menu.gameover) {
			return;
		}

		Movement();
	}

	//moves the character
	private void Movement() {
		//Movements
		float hor = Input.GetAxis("Horizontal");
		float ver = Input.GetAxis("Vertical");
		Vector3 dirMove = new Vector3(hor, 0, ver);

		transform.Translate(dirMove.normalized * MoveSpeed() * Time.deltaTime, Space.Self);

		//Rotations
		float mouseHor = Input.GetAxis("Mouse X");
		float mouseVer = Input.GetAxis("Mouse Y");
		
		if(Mathf.Abs(mouseHor) >= Mathf.Abs(mouseVer)) {
			Vector3 rotHor = new Vector3(0, mouseHor, 0);
			transform.localEulerAngles += rotHor;
		} else if(Mathf.Abs(mouseHor) < Mathf.Abs(mouseVer)) {
			Vector3 rotVer = new Vector3(-mouseVer, 0, 0);
			cam.localEulerAngles += rotVer;
		} else {
			//transform.root.rotation = Quaternion.identity;
		}
	}

	//pickup items to give it as offering to the statue
	public void Pickup(Transform item) {
		Destroy(item.GetComponent<Rigidbody>());
		Destroy(item.GetComponent<Collider>());
		item.parent = hand;
		item.localPosition = Vector3.zero;
		hasPickup = true;
		WorldManager.harmony = false;
		worldManager.WorldChange();
	}

	//offers the item to expand the world
	//also heals the player
	public void OfferItem() {
		if(hand.childCount > 0) {
			Destroy(hand.GetChild(0).gameObject);
			WorldManager.harmony = true;
			worldManager.Expand(worldManager.increaseSize);
			worldManager.SpawnItem();
			worldManager.WorldChange();
			if (soundObj) soundObj.PlayOneShot(offerSound);

			Health hp = transform.GetComponent<Health>();
			hp.GetHeal(hp.maxHp * 0.5f);
		}
	}

	//toggles between running and walking speed
	private float MoveSpeed() {
		if(Input.GetKey(runKey)){
			return runSpeed;
		} else {
			return walkSpeed;
		}
	}

	//gameover
	public void Death() {
		worldManager.menu.OpenEndScreen(false);
	}
}
