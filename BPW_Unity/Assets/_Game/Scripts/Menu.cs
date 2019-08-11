using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
	public GameObject startScreen;
	public GameObject instructionScreen;
	public Transform winScreen;
	public Transform gameOverScreen;
	public float endScreenDelay = 3f;

	static public bool gameover = false;

	void Start() {
		Time.timeScale = 0;
		instructionScreen.SetActive(false);
		startScreen.SetActive(true);
		EndScreenStart(gameOverScreen);
		EndScreenStart(winScreen);
	}

	//starts the game
	public void StartGame() {
		Time.timeScale = 1;
		startScreen.SetActive(false);
		Cursor.lockState = CursorLockMode.Locked;
	}

	//retuns to start menu
	public void ToStartMenu() {
		SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
		Resources.UnloadUnusedAssets();
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
		gameover = false;
		WorldManager.harmony = true;
	}

	public void ExitGame() {
		Application.Quit();
	}

	public void InstructionsBt(bool open) {
		instructionScreen.SetActive(open);
	}

	//set the settings correct at the start
	void EndScreenStart(Transform screen) {
		screen.gameObject.SetActive(true);
		Image img = screen.GetComponent<Image>();
		img.canvasRenderer.SetAlpha(0.0f);
		screen.GetChild(0).gameObject.SetActive(false);
	}

	//decides which screen should open
	public void OpenEndScreen(bool win) {
		gameover = true;
		Cursor.lockState = CursorLockMode.None;

		if(win) {
			StartCoroutine(EndScreen(winScreen));
		} else {
			StartCoroutine(EndScreen(gameOverScreen));
		}
	}

	//enables the end screen
	IEnumerator EndScreen(Transform screen) {
		Image img = screen.GetComponent<Image>();
		img.CrossFadeAlpha(1.0f, endScreenDelay, false);

		yield return new WaitForSeconds(endScreenDelay);
		screen.GetChild(0).gameObject.SetActive(true);
	}
}
