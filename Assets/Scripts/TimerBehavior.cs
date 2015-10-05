using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimerBehavior : MonoBehaviour {

	public Text timerText;
	public GameObject gameManagerObject;

	private GameManager gameManagerScript;
	private float actualTimeRemaining;
	private int minutesRemaining, secondsRemaining;

	private enum TimerState {PAUSED = 0, RUNNING};
	private TimerState timerState;

	// Use this for initialization
	void Start () 
	{
		actualTimeRemaining = 0f;
		minutesRemaining = 0;
		secondsRemaining = 0;
		timerState = TimerState.PAUSED;
		gameManagerScript = gameManagerObject.GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(timerState)
		{
		case TimerState.PAUSED:
			break;

		case TimerState.RUNNING:
			DecrementTimer();
			break;
		}
	}

	private void DecrementTimer() 
	{

		actualTimeRemaining -= Time.deltaTime;
		minutesRemaining = Mathf.FloorToInt(actualTimeRemaining / 60);
		secondsRemaining = Mathf.FloorToInt (actualTimeRemaining % 60);

		timerText.text = "" + minutesRemaining + ":" + secondsRemaining;
		if(actualTimeRemaining > 0 && secondsRemaining == 0)
		{
			timerText.text = "" + minutesRemaining + ":00";
		}
		else if(actualTimeRemaining > 0 && secondsRemaining < 10)
		{
			timerText.text = "" + minutesRemaining + ":0" + secondsRemaining;
		}
		if(actualTimeRemaining <= 0f) 
		{

			timerText.text = "0:00";
			gameManagerScript.EndLevel();
			PauseTimer();
		}
	}

	public void StartTimer() {
		timerState = TimerState.RUNNING;
	}

	public void ResumeTimer() {
		timerState = TimerState.RUNNING;
	}

	public void PauseTimer() {
		print("TIMER PAUSED!");
		timerState = TimerState.PAUSED;
	}
	
	//Used to set the time in seconds
	public void SetTimer(float time) {
		actualTimeRemaining = time;
	}
}
