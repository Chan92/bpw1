using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour{
	[Header("Flooffy")]
	public Transform flooffyPrefab;
	public Transform flooffyFamilly;
	public float maxflooffy = 10;
	public float maxIncreaser = 5;
	public float spawnDelay = 10;
	private float flooffyIncreaser;

	[Header("Item")]
	public Transform itemObjects;

	[Header("UI")]
	public Text flooffyCountTxt;
	public Text overpopulationTxt;
	public Image overpopBarFill;
	public Camera minimapCam;
	public float minimapCamSize = 35;
	public Transform[] icons;
	public float iconSize = 2;
	
	[Header("World")]
	public float increaseSize = 5;
	public Transform ground;
	public float radiusCorrector;

	[Header("WorldChange")]
	public Material skyHarmony;
	public Material skyAggression;
	public Image rateImg;
	public Sprite rateHarmony;
	public Sprite rateAggression;
	public Image rateFillImg;
	public Sprite rateFillHarmony;
	public Sprite rateFillAggression;
	public Color harmonyOverpopCol;
	public Text[] rateTexts;

	static public bool harmony = true;

	[HideInInspector] public int flooffyCounter = 2;
	[HideInInspector] public Menu menu;
	private PlayerController playerControler;
	private float worldScale;	

	private void Awake() {	
		menu = transform.GetComponent<Menu>();
		playerControler = GameObject.FindObjectOfType<PlayerController>();
		flooffyIncreaser = maxflooffy;
		worldScale = ground.localScale.x + ground.localScale.z;
		SpawnItem();
		StartCoroutine(NewFlooffy());
		overpopulationTxt.text = "Max population: " + maxflooffy;
		OverpopulationUI();
	}

	//expand the world
	public void Expand(float size) {
		flooffyIncreaser += maxIncreaser; 
		maxflooffy += flooffyIncreaser;
		ground.localScale += new Vector3(size, 0, size);
		MiniMapUpdate();
		worldScale = ground.localScale.x + ground.localScale.z;
		overpopulationTxt.text = "Max population: " + maxflooffy;
		OverpopulationUI();
	}

	//updates the minimap when the map grows
	void MiniMapUpdate() {
		minimapCam.orthographicSize += minimapCamSize;

		for(int i = 0; i < icons.Length; i++) {
			float newScale = icons[i].localScale.x + iconSize;
			icons[i].localScale = new Vector3(newScale, newScale, newScale);
		}
	}

	//spawn an item which expands the world
	public void SpawnItem() {
		if(maxflooffy < 100) {
			int id = Random.Range(0, itemObjects.childCount);
			Transform obj = itemObjects.GetChild(id);
			Transform item = Instantiate(obj, RandomPos(), Quaternion.identity);
		}
	}

	//check for gameover and spawn flooffy 
	void SpawnFlooffy() {
		if(maxflooffy > flooffyCounter) {
			Transform newFlooffy = Instantiate(flooffyPrefab, RandomPos(), Quaternion.identity);
			newFlooffy.parent = flooffyFamilly;
			flooffyCounter++;			
			OverpopulationUI();

			if(flooffyCounter == 100) {
				menu.OpenEndScreen(true);
			}
		} else {
			GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().isDeath = true;
			menu.OpenEndScreen(false);
		}
	}

	//shows howmany bunnies in the game are 
	//and when the world gets overpopulated
	public void OverpopulationUI() {
		flooffyCountTxt.text = "" + flooffyCounter;
		overpopBarFill.fillAmount = flooffyCounter / maxflooffy;
	}

	//generate a random position
	public Vector3 RandomPos() {
		Vector3 pos = new Vector3(RandomRadius(), 0, RandomRadius());
		
		while (Vector3.Distance(Vector3.zero, pos) >= WorldRadius()) {
			pos = new Vector3(RandomRadius(), 0, RandomRadius());
		}
		pos.y += 5;
		
		return pos;
	}

	//generate a random radius
	private float RandomRadius() {
		return Random.Range(-WorldRadius(), WorldRadius());
	}

	//gets the world radius
	private float WorldRadius() {
		float groundChild = ground.GetChild(0).transform.localScale.x;
		float groundself = ground.localScale.x;

		return (groundself * groundChild)/2 - radiusCorrector;
	}

	//Spawn a new flooffy every couple seconds as long as the game is going on
	IEnumerator NewFlooffy() {
		while(!Menu.gameover) {
			float balancedDelay = spawnDelay / (maxflooffy * 0.5f);
			yield return new WaitForSeconds(balancedDelay);
			SpawnFlooffy();
		}
	}

	//changes the art between harmony and aggression mode
	public void WorldChange() {
		EnemyInfo[] enemyInfo = GameObject.FindObjectsOfType<EnemyInfo>();

		if(WorldManager.harmony) {
			RenderSettings.skybox = skyHarmony;
			rateImg.sprite = rateHarmony;
			rateFillImg.sprite = rateFillHarmony;
			overpopulationTxt.color = harmonyOverpopCol;

			for(int i = 0; i < enemyInfo.Length; i++) {
				enemyInfo[i].SetHarmonyColors();
			}

			for(int j = 0; j < rateTexts.Length; j++) {
				rateTexts[j].color = Color.black;
			}
		} else {
			RenderSettings.skybox = skyAggression;
			rateImg.sprite = rateAggression;
			rateFillImg.sprite = rateFillAggression;
			overpopulationTxt.color = Color.gray;

			for(int i = 0; i < enemyInfo.Length; i++) {
				enemyInfo[i].SetAggressionColors();
			}

			for(int j = 0; j < rateTexts.Length; j++) {
				rateTexts[j].color = Color.gray;
			}
		}
	}
}