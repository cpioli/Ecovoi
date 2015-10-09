using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	
	public enum GameState {GAMEOVER = 0, LEVELSTART, INPLAY, PAUSE, LEVELEND, LEVELWON};
	public GameState gameState;
	
	public GameObject timer;
	public Image winSign;
	public Image loseSign;
	public Image startSign;
	public Image pauseSign;

	public AudioClip winAudio, lossAudio;
	private FactoryBehavior factoryBehavior;
	private WorldManager worldManager;

	private TimerBehavior timerBehavior;
	private const int LMB = 0;
	private const int RMB = 1;
	public int[] levelGoals = new int[4];
	public float[] levelTime = new float[4];
	public int currentLevel;

	//these bools indicate whether the switch to the current state is complete.
	bool gameOver, levelStartActive, levelWonActive;

	void Awake() {
		winSign.gameObject.SetActive (false);
		loseSign.gameObject.SetActive (false);
		startSign.gameObject.SetActive (false);
		pauseSign.gameObject.SetActive (false);
	}

	void Start() {
		gameState = GameState.LEVELSTART;
		print("Switched to gameState " + gameState);
		levelStartActive = false;
		levelWonActive = false;
		gameOver = false;
		currentLevel = 0;
		timerBehavior = timer.GetComponent<TimerBehavior>();
		factoryBehavior = GameObject.Find("Factory").GetComponent<FactoryBehavior>();
		worldManager = GameObject.Find ("GameManager").GetComponent<WorldManager>();
	}
	
	
	
	void Update() {
		//print(Input.mousePosition);
		switch(gameState) {
		case GameState.GAMEOVER:
			GameOver();
			break;
			
		case GameState.LEVELSTART:
			LevelStart(); //await player input to start
			break;
			
		case GameState.INPLAY:
			InPlay();
			break;
		
		case GameState.PAUSE:
			Paused();
			break;
		
		case GameState.LEVELEND:
			this.EndLevel();
			break;

		case GameState.LEVELWON:
			LevelWon();
			break;
		}
	}

	public void GameOver() {
		if(!gameOver) {
			timerBehavior.PauseTimer();
			audio.Stop();
			audio.PlayOneShot(lossAudio);
			loseSign.gameObject.SetActive(true);
			gameOver = true;
			return;
		}
		if(Input.GetMouseButtonUp(LMB)) {
			currentLevel = 0;
			levelStartActive = false;
			levelWonActive = false;
			gameOver = false;
			loseSign.gameObject.SetActive(false);
			factoryBehavior.Restart ();
			worldManager.Restart();
			gameState = GameState.LEVELSTART;
			print("Switched to gameState " + gameState);
			return;
		}

	}
	
	void LevelStart() {
		if(!levelStartActive) {
			audio.Stop();
			audio.Play();
			startSign.gameObject.SetActive(true);
			timerBehavior.SetTimer (levelTime[currentLevel]);
			factoryBehavior.LevelSetUp(levelGoals[currentLevel]);
			levelStartActive = true;
			return;
		}
		if(Input.GetMouseButtonUp(LMB)) {
			timerBehavior.StartTimer ();
			gameState = GameState.INPLAY;
			print("Switched to gameState " + gameState);
			startSign.gameObject.SetActive (false);
			levelStartActive = false;
		}
	}
	
	
	//things that happen during play, like pausing, go here.
	private void InPlay() {
		//TODO: track if ESC button is pressed, then go into pause
		if(Input.GetKeyUp(KeyCode.Escape)) {
			gameState = GameState.PAUSE;
			print("Switched to gameState " + gameState);
			timerBehavior.PauseTimer();
			pauseSign.gameObject.SetActive(true);
		}
	}

	void Paused() {
		//TODO: track if Esc is pressed, then unpause
		if(Input.GetKeyUp(KeyCode.Escape)) {
			gameState = GameState.INPLAY;
			print("Switched to gameState " + gameState);
			timerBehavior.ResumeTimer();
			pauseSign.gameObject.SetActive (false);
		}
	}

	public void EndLevel() {
		if(gameState != GameState.LEVELEND) {
			gameState = GameState.LEVELEND;
			print("Switched to gameState " + gameState);
		}

		//TODO: detect win or loss, and call either/or
		//bring up the end of level overlay
		if(!factoryBehavior.Success()) {
			gameState = GameState.GAMEOVER;
			print("Switched to gameState " + gameState);
		}
		else {
			gameState = GameState.LEVELWON;
			print("Switched to gameState " + gameState);
		}

		return;

	}

	public void LevelWon() {
		//TODO: continue onto the next level from this function
		if(!levelWonActive) {
			audio.Stop();
			audio.PlayOneShot(winAudio);
			winSign.gameObject.SetActive(true);
			levelWonActive = true;
			timerBehavior.PauseTimer ();
			return;
		}

		if(Input.GetMouseButtonUp(LMB)) {
			gameState = GameState.LEVELSTART;
			print("Switched to gameState " + gameState);
			currentLevel++;
			winSign.gameObject.SetActive(false);
			levelWonActive = false;
		}
	}

	//deprecated
	void CarbonOnGoal () {
		// Do what we want with Carbon/ Game here
		Debug.Log ("You put the carbon in the factory!");
	}




}
