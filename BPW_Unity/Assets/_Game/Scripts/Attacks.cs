using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacks : MonoBehaviour {
	public GameObject bullet;
	public Vector2 attackStrength = new Vector2(3.5f, 8f);
	public float attackSpeed = 5f;
	public float attackRange = 10f;
	public float cooldowntime = 1f;
	

	public Transform crosshair;
	private bool cooldown = false;

	Ray mouseRay;
	RaycastHit hit;
	Vector3 dir;
	Quaternion rot;

	//fixes to attack upon starting game
	private void Start() {
		StartCoroutine(AttackCoolDown());
	}

	private void Update() {
		if(Menu.gameover) {
			return;
		}

		if(Input.GetMouseButtonDown(0)) {
			Attack();
		}
	
		mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

		if(Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit, attackRange)) {
			dir = hit.point - transform.position;
		} else {
			dir = mouseRay.GetPoint(attackRange) - transform.position;
		}
	}

	//attacks
	void Attack() {
		if(!cooldown) {
			rot = Quaternion.LookRotation(dir);

			GameObject newBullet = Instantiate(bullet, crosshair.position, rot);
			newBullet.GetComponent<Bullet>().strength = Random.Range(attackStrength.x, attackStrength.y);
			newBullet.GetComponent<Rigidbody>().velocity = -(transform.forward - dir).normalized * attackSpeed;

			StartCoroutine(AttackCoolDown());
		}
	}

	//cooldown for attack
	IEnumerator AttackCoolDown() {
		cooldown = true;
		yield return new WaitForSeconds(cooldowntime);
		cooldown = false;
	}
}
