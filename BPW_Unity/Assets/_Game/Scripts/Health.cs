using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {
	[Header("Basic")]
	public Image hpFill;
	public float maxHp = 100f;
	public float invulnerableTime = 1f;
	public float unregenerateTime = 1f;
	public string deathcall;

	[HideInInspector]
	public bool isDeath = false;
	private float curHp;
	private bool invulnerable;
	private bool unregenerate;

	[Header("Effects")]
	public Animator animator;
	public string dmgAnim = "Dmg";
	public string deathAnim = "Death";
	public string healAnim = "Heal";

	[Space(10)]
	public AudioSource soundObj;
	public AudioClip dmgSound;
	public AudioClip dyingSound;
	public AudioClip healSound;

	[Space(10)]
	public Transform effectPos;
	public ParticleSystem dmgEffect;
	public ParticleSystem dyingEffect;
	public ParticleSystem healEffect;

	private void Start() {
		curHp = maxHp;
		UpdateUI();
	}

	//reduces hp by given amount
	public void GetDmg(float dmg) {
		if(invulnerable) {
			return;
		}

		curHp -= dmg;

		if(curHp <= 0) {
			curHp = 0;

			SoundEffect(dyingSound);
			GraphicEffect(dyingEffect);
			AnimationEffect(deathAnim);
			isDeath = true;
			if(deathcall != "")
				SendMessage(deathcall);
		}

		AnimationEffect(dmgAnim);
		SoundEffect(dmgSound);
		GraphicEffect(dmgEffect);
		UpdateUI();

		StartCoroutine(Invulnerable());
	}

	//kills in one hit
	public void OneHitKill() {
		if(invulnerable) {
			return;
		}

		curHp = 0;

		SoundEffect(dyingSound);
		GraphicEffect(dyingEffect);
		AnimationEffect(deathAnim);
		UpdateUI();
		isDeath = true;
		if(deathcall != "")
			SendMessage(deathcall);
	}

	//heals a given amount
	public void GetHeal(float heal) {
		if(unregenerate) {
			return;
		}

		curHp += heal;

		if(curHp > maxHp) {
			curHp = maxHp;
		}

		AnimationEffect(healAnim);
		SoundEffect(healSound);
		GraphicEffect(healEffect);
		UpdateUI();

		StartCoroutine(Unregenerate());
	}

	//heals completely
	public void GetHeal() {
		if(unregenerate) {
			return;
		}

		curHp = maxHp;

		AnimationEffect(healAnim);
		SoundEffect(healSound);
		GraphicEffect(healEffect);
		UpdateUI();

		StartCoroutine(Unregenerate());
	}

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//update the UI, if avaible
	void UpdateUI() {
		if(!hpFill)
			return;

		hpFill.fillAmount = curHp / maxHp;
	}

	//plays animation, if avaible
	void AnimationEffect(string anim) {
		if(!animator || !animator.GetBool(anim))
			return;

		animator.SetBool(anim, true);
	}

	//plays sound effect, if avaible
	void SoundEffect(AudioClip clip) {
		if(!soundObj || !clip)
			return;

		soundObj.PlayOneShot(clip);
	}

	//spawns particle effect, if avaible
	void GraphicEffect(ParticleSystem particle) {
		if(!particle)
			return;

		Vector3 spawnPos = transform.position;
		if(effectPos) {
			spawnPos = effectPos.position;
		}

		GameObject newParticle = Instantiate(particle, spawnPos, Quaternion.identity).gameObject;
		Destroy(newParticle, particle.main.duration);
	}
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//unable to get dmg for a short time to avoid getting hit many times at once
	IEnumerator Invulnerable() {
		invulnerable = true;
		yield return new WaitForSeconds(invulnerableTime);
		invulnerable = false;
	}

	//unable to heal for a short amount of time
	IEnumerator Unregenerate() {
		unregenerate = true;
		yield return new WaitForSeconds(unregenerateTime);
		unregenerate = false;
	}
}
