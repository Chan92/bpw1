using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour{
	enum EnemyState {
		Idle,
		Moving,
		Death,
		Attack
	};

	public float stateDelay = 5f;
	public float distanceCheck = 5f;
	Vector3 sphereOffset = new Vector3(0, 0.3f, 0);
	float sphereRad = 0.5f;
	float sphereDist = 0.5f;

	[Space(10)]
	public float harmonySpeed = 3f;
	public float aggressionSpeed = 7f;

	[Space(10)]
	public float attackStrength = 10f;
	public float attackRange = 8f;
	public float attackDelay = 0.5f;
	private bool attackCooldown = false;
	private RaycastHit hit;

	public AudioSource soundObj;
	public AudioClip attackSound;

	private Transform player;
	private WorldManager worldManager;
	private NavMeshAgent agent;
	private Animator anim;
	private Coroutine randStateRoutine;
	private Vector3 destPos;
	private EnemyState enemyState;
	private Health hp;

	void Start() {	
		hp = transform.GetComponent<Health>();
		soundObj = GameObject.Find("SoundEffect").GetComponent<AudioSource>();
		hp.soundObj = soundObj;
		player = GameObject.FindWithTag("Player").transform;
		agent = transform.GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
		worldManager = FindObjectOfType<WorldManager>();
		enemyState = EnemyState.Moving;
		destPos = worldManager.RandomPos();
		agent.SetDestination(destPos);
	}

	void Update(){
		if(hp.isDeath) {
			return;
		}

		if(!WorldManager.harmony && !Menu.gameover) {
			agent.speed = aggressionSpeed;
			AggressionState();
		} else {
			agent.speed = harmonySpeed;
			HarmonyState();
		}
	}

	//when the bunnies are friendly
	void HarmonyState() {
		if(randStateRoutine == null) {
			randStateRoutine = StartCoroutine(RandomState());
		}

		if(enemyState == EnemyState.Moving) {
			if(Vector3.Distance(transform.position, destPos) < distanceCheck) {
				destPos = worldManager.RandomPos();
				agent.SetDestination(destPos);
			}
		}
	}

	//when the bunnies attack the player
	void AggressionState() {
		transform.LookAt(player);
		agent.SetDestination(player.position);

		RaycastHit hit;
		if(Physics.SphereCast(transform.position + sphereOffset, sphereRad, Vector3.forward, out hit, sphereDist)) {
			agent.isStopped = true;
			enemyState = EnemyState.Attack;
			Attack();
		} else {
			agent.isStopped = false;
			enemyState = EnemyState.Moving;
		}
	}

	//moves the enemy
	void Movement() {
		agent.isStopped = false;
		anim.SetInteger("AnimIndex", 1);
		anim.SetTrigger("Next");

		destPos = worldManager.RandomPos();
		agent.SetDestination(destPos);
	}

	//lets the enemy stand for a while
	void Idle() {
		agent.isStopped = true;
		anim.SetInteger("AnimIndex", 0);
		anim.SetTrigger("Next");
	}

	//death
	void Death() {
		transform.GetComponent<Collider>().enabled = false;
		agent.isStopped = true;
		anim.SetInteger("AnimIndex", 2);
		anim.SetTrigger("Next");

		worldManager.flooffyCounter--;
		worldManager.OverpopulationUI();
		Destroy(gameObject, 5f);

		if(worldManager.flooffyCounter < 2) {
			worldManager.menu.OpenEndScreen(false);
		}
	}

	//attacks (happens only in aggression)
	void Attack() {
		if(attackCooldown) {
			return;
		}

		if(Physics.Raycast(transform.position, transform.forward, out hit, attackRange) && hit.collider.tag == "Player") {
			player.GetComponent<Health>().GetDmg(attackStrength);
		}

		anim.SetInteger("AnimIndex", 3);
		anim.SetTrigger("Next");
		if(soundObj && !soundObj.isPlaying) soundObj.PlayOneShot(attackSound);
		StartCoroutine(AttackCooldown());
	}

	//reduces the health of the enemy and checks if the enemy is in harmony status
	//if the enemy is in harmony, the player gets their health reduced as well
	//if the enemy dies in harmony, the player takes double the dmg as punishment
	//if the enemy dies in aggression, heal the player
	public void GetDmg(float strength) {	
		hp.GetDmg(strength);

		Health playerHp = player.GetComponent<Health>();

		if(hp.isDeath) {
			if(!WorldManager.harmony) {
				float heal = Random.Range(strength * 0.5f, strength);
				playerHp.GetHeal(heal);
			}

			Death();
			strength *= 2;
		}
	
		if(WorldManager.harmony) {
			playerHp.GetDmg(strength);
		}
	}

	//every x sec, randomly selects a state between idle and moving
	IEnumerator RandomState() {
		enemyState = (EnemyState) Random.Range(0, 2);

		switch(enemyState) {
			case EnemyState.Idle:
				Idle();
				break;
			case EnemyState.Moving:
				Movement();
				break;
		}

		yield return new WaitForSeconds(stateDelay);
		randStateRoutine = null;
	}

	//makes the bunnies attack only once every few sec
	IEnumerator AttackCooldown() {
		attackCooldown = true;
		yield return new WaitForSeconds(attackDelay);
		attackCooldown = false;
	}
}
