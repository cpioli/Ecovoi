using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class WorldManager : MonoBehaviour {

	
	public GameObject Factory;
	public Image redWarningFlash;
	private Tween flashingRed;
	private bool isFlashing;
	private FactoryBehavior factoryBehavior;

	public float carbonIncrement = 5f;
	private float atmosphericCarbonCount;
	public float atmosphereCarbonCapacity;
	private float undergroundCarbonCount;
	public float undergroundCarbonCapacity;

	private int electricityGenerated;
	private int totalElectricityGenerated;

	// Use this for initialization
	void Start () {
		factoryBehavior = Factory.GetComponent<FactoryBehavior>();

		atmosphericCarbonCount = 0f;
		undergroundCarbonCount = undergroundCarbonCapacity;

		isFlashing = false;
		redWarningFlash.color = new Color(0.87f, 0f, 0f, 0.0f);
		redWarningFlash.gameObject.SetActive(false);
		flashingRed = redWarningFlash.DOColor(new Color(.87f, 0f, 0f, 0.5f), 1.5f).SetLoops (-1, LoopType.Restart).SetEase (Ease.OutCubic);
		flashingRed.Pause();
	}
	
	// Update is called once per frame
	//CPIOLI: this is unused right now.
	void Update () {
	}

	public void AddCarbonToUndergroundCarbonMeter(float carbon) {
		undergroundCarbonCount += carbon;
		if(undergroundCarbonCount > undergroundCarbonCapacity) {
			undergroundCarbonCount = undergroundCarbonCapacity;
		}
	}

	public void RemoveCarbonFromUndergroundCarbonMeter(float carbon) {
		undergroundCarbonCount -= carbon;
		if(undergroundCarbonCount < 0) { //no negative values allowed
			undergroundCarbonCount = 0;
		}
	}

	public void AddCarbonToAtmosphericCarbonMeter(float carbon) {
		atmosphericCarbonCount += carbon;
		if(atmosphericCarbonCount >= 40 && !isFlashing) {
			print("WARNING!");
			isFlashing = true;
			redWarningFlash.gameObject.SetActive(true);
			flashingRed.Play ();
		}
		if(atmosphericCarbonCount >= atmosphereCarbonCapacity) {
			GameObject.Find ("GameManager").GetComponent<GameManager>().gameState = GameManager.GameState.GAMEOVER;
		}
	}

	public void RemoveCarbonFromAtmosphericCarbonMeter(float carbon) {
		atmosphericCarbonCount -= carbon;
		if(atmosphericCarbonCount < 40 && isFlashing) {
			print("WARNING OVER!");
			isFlashing = false;
			redWarningFlash.gameObject.SetActive(false);
			flashingRed.Pause ();

		}
		if(atmosphericCarbonCount < 0) { //no negative values are allowed!
			atmosphericCarbonCount = 0;
		}
	}

	public void Restart() {
		atmosphericCarbonCount = 0f;
		undergroundCarbonCount = undergroundCarbonCapacity;
	}
}
