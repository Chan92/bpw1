using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfo : MonoBehaviour {
	[Header("Parts")]
	public Renderer bodyMat;
	public Renderer eyesMat;
	public Image hpFillImg;
	[Header("Harmony")]
	public Material harmonyBodyMat;
	public Material harmonyEyesMat;	
	public Color harmonyHpFill;
	[Header("Aggression")]
	public Material aggressionBodyMat;
	public Material aggressionEyesMat;
	public Color aggressionHpFill;

	private void Awake() {
		if(WorldManager.harmony) {
			SetHarmonyColors();
		} else {
			SetAggressionColors();
		}
	}

	void FixedUpdate() {
		transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }

	public void SetHarmonyColors() {
		bodyMat.material = harmonyBodyMat;
		eyesMat.material = harmonyEyesMat;
		hpFillImg.color = harmonyHpFill;
	}

	public void SetAggressionColors() {
		bodyMat.material = aggressionBodyMat;
		eyesMat.material = aggressionEyesMat;
		hpFillImg.color = aggressionHpFill;
	}
}
