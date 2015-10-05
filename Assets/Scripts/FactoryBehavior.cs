using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class FactoryBehavior : MonoBehaviour {

	public Text buildingsAdded;
	public Text buildingsReduced;
	public Text accumulatedBuildingsText;
	public WorldManager worldManager;
	public GameManager gameManager;
	public GameObject Buildings;
	private BuildingsBehavior buildingsBehavior;

	public int totalAccumulatedBuildings;
	public int requiredBuildings;
	
	private struct CarbonUnit {
		public GameObject carbon;
		public float duration; //how long the carbon is inside
	}

	private Vector2 defaultAddedLocation = new Vector2(179f, 23.5f);
	private Vector2 defaultReducedLocation = new Vector2(-144f, -4f);
	private CarbonUnit[] sequestrationQueue;
	private int sequestrationQueueSize;
	private CarbonUnit[] burningQueue;
	private int burningQueueSize;

	public int maxQueueSize = 3;
	public float processDuration = 5f;
	public float carbonUnitSize = 5f; //for every X units in the meter, we produce one piece of carbon
	private float currentAtmosCarbonMeterAccumulation;
	private float currentGroundCarbonMeterAccumulation; //every time these reach "carbonUnitSize", a new cloud or coal forms



	void Awake() {
	}

	void Start () {
		buildingsAdded.gameObject.SetActive(false);
		buildingsAdded.rectTransform.anchoredPosition = defaultAddedLocation;
		buildingsReduced.gameObject.SetActive(false);
		buildingsReduced.rectTransform.anchoredPosition = defaultReducedLocation;
		accumulatedBuildingsText.gameObject.SetActive(false);
		totalAccumulatedBuildings = 0;
		requiredBuildings = 0; //to be reset every level
		buildingsBehavior = Buildings.GetComponent<BuildingsBehavior>();

		currentGroundCarbonMeterAccumulation = 0f;
		currentAtmosCarbonMeterAccumulation = 0f;
		worldManager = GameObject.Find ("GameManager").GetComponent<WorldManager>();
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager>();


		//creating the Factory's burning and sequestration queues
		burningQueue = new CarbonUnit[maxQueueSize];        
		for(int i = 0; i < maxQueueSize; i++) {
			burningQueue[i] = new CarbonUnit();
			burningQueue[i].carbon = null;
			burningQueue[i].duration = 0f;
		}
		burningQueueSize = 0;

		sequestrationQueue = new CarbonUnit[maxQueueSize];
		for(int i = 0; i < maxQueueSize; i++) {
			sequestrationQueue[i] = new CarbonUnit();
			sequestrationQueue[i].carbon = null;
			sequestrationQueue[i].duration = 0f;
		}
		sequestrationQueueSize = 0;

		//
	}

	// Update is called once per frame
	void Update () {
		if(gameManager.gameState != GameManager.GameState.INPLAY) return;
		for(int i = maxQueueSize-1; i > -1; i--) {
			if(sequestrationQueue[i].carbon == null) break;
			sequestrationQueue[i].duration += Time.deltaTime;
			//print("Sequestered #" + i + "'s Time: " + sequestrationDuration[i]);
			if(sequestrationQueue[i].duration >= processDuration) {
				ReleaseSequesteredCarbon();
			}
		}

		for(int i = maxQueueSize-1; i > -1; i--) {
			if(burningQueue[i].carbon == null) break;
			burningQueue[i].duration += Time.deltaTime;
			//print("Burning #" + i + "'s Time: " + burningDuration[i]);
			if(burningQueue[i].duration >= processDuration) {
				ReleaseBurnedCarbon();
			}
		}

		worldManager.AddCarbonToUndergroundCarbonMeter( sequestrationQueueSize * Time.deltaTime);
		currentGroundCarbonMeterAccumulation += sequestrationQueueSize * Time.deltaTime;
		if(currentGroundCarbonMeterAccumulation >= carbonUnitSize) {
			//TODO: call the function to add one more coal to the ground
			currentGroundCarbonMeterAccumulation -= carbonUnitSize;
		}

		switch(burningQueueSize) {

		case 1:
			worldManager.AddCarbonToAtmosphericCarbonMeter (1 * Time.deltaTime);
			currentAtmosCarbonMeterAccumulation += 1 * Time.deltaTime;
			break;

		case 2:
			worldManager.AddCarbonToAtmosphericCarbonMeter (3 * Time.deltaTime);
			currentAtmosCarbonMeterAccumulation += 3 * Time.deltaTime;
			break;

		case 3:
			worldManager.AddCarbonToAtmosphericCarbonMeter (5 * Time.deltaTime);
			currentAtmosCarbonMeterAccumulation += 5 * Time.deltaTime;
			break;

		default:
			break;
		}

		if(currentAtmosCarbonMeterAccumulation >= carbonUnitSize) {
			//TODO: call the function to add one more carbon cloud into the air
			currentAtmosCarbonMeterAccumulation -= carbonUnitSize;
		}
	}

	public void LevelSetUp(int requiredBuildings) {
		this.requiredBuildings = requiredBuildings;
		this.totalAccumulatedBuildings = 0;
		accumulatedBuildingsText.gameObject.SetActive(true);
		accumulatedBuildingsText.text = "" + totalAccumulatedBuildings + "/" + requiredBuildings;
	}

	public bool Success() {
		return totalAccumulatedBuildings >= requiredBuildings;
	}

	public void AccumulateBuildings() {
		buildingsAdded.text = "+5";
		totalAccumulatedBuildings += 5;
		accumulatedBuildingsText.text = "" + totalAccumulatedBuildings + "/" + requiredBuildings;
		buildingsAdded.gameObject.SetActive(true);
		Sequence buildingSequence = DOTween.Sequence ();
		buildingSequence.Append (buildingsAdded.rectTransform.DOLocalMoveY (120f, 3.0f))
			.Join (buildingsAdded.DOColor (new Color(1f, 1f, 1f, 0f), 3.0f).SetEase(Ease.OutCubic))
			.AppendCallback(ResetAddedPositionAndAlpha);

		if(totalAccumulatedBuildings >= (buildingsBehavior.currentBuilding + 1) * 5) {
			buildingsBehavior.AddBuilding ();
		}

		if(totalAccumulatedBuildings >= requiredBuildings) {
			gameManager.gameState = GameManager.GameState.LEVELWON;
		}
	}

	public void ReduceBuildings() {
		buildingsReduced.text = "-7";
		totalAccumulatedBuildings -= 7;
		accumulatedBuildingsText.text = "" + totalAccumulatedBuildings + "/" + requiredBuildings;
		buildingsReduced.gameObject.SetActive(true);
		Sequence buildingSequence = DOTween.Sequence ();
		buildingSequence.Append (buildingsReduced.rectTransform.DOLocalMoveY (120f, 3.0f))
			.Join (buildingsReduced.DOColor (new Color(1f, 1f, 1f, 0f), 3.0f).SetEase (Ease.OutCubic))
				.AppendCallback (ResetReducedPositionAndAlpha);

		if(totalAccumulatedBuildings < 0) {
			totalAccumulatedBuildings = 0;
			return;
		}

		if(totalAccumulatedBuildings < (buildingsBehavior.currentBuilding * 5)) {
			buildingsBehavior.RemoveBuilding();
			//if the current ones digit of totalAccumulatedBuildings is 3, 4, 8, or 9
			//that means two buildings have been removed due to the subtraction of 7
			//so just remove an additional building
			if(totalAccumulatedBuildings % 10 == 3
			   || totalAccumulatedBuildings % 10 == 4
			   || totalAccumulatedBuildings % 10 == 8
			   || totalAccumulatedBuildings % 10 == 9) {

				buildingsBehavior.RemoveBuilding ();
			}
		}
	}
	
	private void ResetAddedPositionAndAlpha() {
		buildingsAdded.gameObject.SetActive(false);
		buildingsAdded.rectTransform.anchoredPosition = defaultAddedLocation;
		buildingsAdded.DOColor(Color.black, 0f);
	}

	private void ResetReducedPositionAndAlpha() {
		buildingsReduced.gameObject.SetActive(false);
		buildingsReduced.rectTransform.anchoredPosition = defaultReducedLocation;
		buildingsReduced.DOColor (Color.black, 0f);
	}

	public bool AddCarbonToSequestration(GameObject carbon)
	{
		if(sequestrationQueueSize == maxQueueSize) return false;
		print("Added Carbon to factory for sequestration!");
		for(int j = maxQueueSize - 1; j > -1; j--) { //find the first available location
			if(sequestrationQueue[j].carbon == null) {
				sequestrationQueue[j].carbon = carbon; //add the carbon inside
				sequestrationQueue[j].duration = 0f; //reset the timer
				sequestrationQueueSize++; //increment queue size
				break;
			}
		}
		print("Sequestered Size: " + sequestrationQueueSize);
		print(sequestrationQueue);
		//TODO: Remove an equivalent amount of carbon from the Atmosphere
		worldManager.RemoveCarbonFromAtmosphericCarbonMeter (5f);
		ReduceBuildings ();
		return true;
	}

	public bool AddCarbonToBurning(GameObject carbon) {
		if(burningQueueSize == maxQueueSize) return false;

		print("Added Carbon to factory for burning!");
		for(int j = maxQueueSize - 1; j > -1; j--) {
			if(burningQueue[j].carbon == null) 
			{
				burningQueue[j].carbon = carbon;
				burningQueue[j].duration = 0f;
				burningQueueSize++;
				break;
			}
		}
		print("Burning size: " + burningQueueSize);
		print(burningQueueSize);
		worldManager.RemoveCarbonFromUndergroundCarbonMeter(5f);
		AccumulateBuildings();
		return true;
	}



	//Releases carbon from the queue
	//moves the remaining carbon forward one position in the queue
	//tells Carbon Prefab object to move to its designated position in its new home
	private void ReleaseSequesteredCarbon() {
		//TODO: Complete
	//	print("Released Sequestered Carbon into the earth.");
		sequestrationQueue[maxQueueSize-1].carbon = null;
		sequestrationQueue[maxQueueSize-1].duration = 0f;
		for(int j = maxQueueSize-1; j > 0; j--) {
			sequestrationQueue[j].carbon = sequestrationQueue[j-1].carbon;
			sequestrationQueue[j].duration = sequestrationQueue[j-1].duration;
		}
		sequestrationQueue[0].carbon = null;
		sequestrationQueue[0].duration = 0f;
		sequestrationQueueSize--;
//		print("Sequestered Size: " + sequestrationQueueSize);
//		print(sequestrationQueue);
	}

	private void ReleaseBurnedCarbon() {
		//burningQueue[2].GetComponent<CarbonBehavior>().MoveToNewHome(); //TODO: implement
	//	print("Released Burning Carbon into atmosphere!");
		burningQueue[maxQueueSize - 1].carbon = null;
		for(int j = maxQueueSize - 1; j > 0; j--) {
			burningQueue[j] = burningQueue[j-1];
			burningQueue[j].duration = burningQueue[j-1].duration;
		}
		burningQueue[0].carbon = null;
		burningQueue[0].duration = 0f;
		burningQueueSize--;
	//	print("Burning size: " + burningQueueSize);
		print(burningQueue);
	}

	public void Restart() {
		totalAccumulatedBuildings = 0;
		requiredBuildings = 0; //to be reset every level
		currentGroundCarbonMeterAccumulation = 0f;
		currentAtmosCarbonMeterAccumulation = 0f;
		
		
		//creating the Factory's burning and sequestration queues      
		for(int i = 0; i < maxQueueSize; i++) {
			burningQueue[i].carbon = null;
			burningQueue[i].duration = 0f;
		}
		burningQueueSize = 0;
		
		for(int i = 0; i < maxQueueSize; i++) {
			sequestrationQueue[i].carbon = null;
			sequestrationQueue[i].duration = 0f;
		}
		sequestrationQueueSize = 0;
	}

}
