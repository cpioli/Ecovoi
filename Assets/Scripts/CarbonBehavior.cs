﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

// Allows Player to drag and change positions of objects on screen. Objects need a collider attached.

public class CarbonBehavior : MonoBehaviour {

	public GameObject Factory;
	public Transform pipePosition;
	private FactoryBehavior factoryBehavior;
	private bool carbonGrabbed;
	public float timeToComplete; // Should be the same as distance from camera to sprites, unless you want the size to change on drag
	//public float xUpBound, xLowBound, yUpBound, yLowBound;
	//public float factoryXUpBound, factoryXLowBound, factoryYUpBound, factoryYLowBound;
	public GameObject gameManager, factoryPanel, skyPanel, groundPanel, workingPanel, worldCanvas, coalParticle, leafParticle;
	public AudioClip coalAudio, leafAudio;
	public Sprite[] clouds, coals;
	public GameObject[] sequesterSlots, burnSlots;
	GameObject[] atmosphereSlots, undergroundSlots;

	public bool interactable = true;
	RectTransform carbonBoundRT;

	public Vector3 factoryScale, dragScale;
	Vector3 originalScale;

	public Vector2 originalLocation;

	public Vector2 factoryBoundMin;
	public Vector2 factoryBoundMax;
	public Vector2 boundMin;
	public Vector2 boundMax;


	public enum CarbonState {NONE = 0, UNDERGROUND, ATMOSPHERIC};
	public CarbonState carbonState;

	private float xResolutionScale, yResolutionScale;
	//because the canvas is working in world space with an absolute width and height
	//I'll have to change things around.


	public void Initialize(Vector2 startingLocation, CarbonState state) {
		this.originalLocation = startingLocation;
		this.carbonState = state;
		print("original Location: " + originalLocation);
	}

	Vector3 previousLocation;
	Vector3 currentLocation;

//	Necessary for Animation
//	Animator anim;
//
	void Start(){
		/*
		 * A block of code to keep track of the resolution scales 
		 */
		print("Dimensions: " + Screen.width + ", " + Screen.height);
		print("Size of World Object: " + GameObject.Find ("World").GetComponent<RectTransform>().sizeDelta);
		xResolutionScale = Screen.width / GameObject.Find ("World").GetComponent<RectTransform>().lossyScale.x;
		yResolutionScale = Screen.height / GameObject.Find ("World").GetComponent<RectTransform>().lossyScale.y ;

		print("xResolutionScale: " + xResolutionScale);
		print("yResolutionScale: " + yResolutionScale);

//		anim = GetComponent<Animator>();
		carbonBoundRT = skyPanel.GetComponent<RectTransform>();
		carbonBoundRT = groundPanel.GetComponent<RectTransform>();
		SetState(this.carbonState);
		//factoryBoundMin.x = factoryPanel.transform.position.x - factoryPanel.GetComponent<RectTransform>().rect.width/2;
		//factoryBoundMin.y = factoryPanel.transform.position.y - factoryPanel.GetComponent<RectTransform>().rect.height/2;
		//factoryBoundMax.x = factoryPanel.transform.position.x + factoryPanel.GetComponent<RectTransform>().rect.width/2;
		//factoryBoundMax.y = factoryPanel.transform.position.y + factoryPanel.GetComponent<RectTransform>().rect.height/2;
		carbonGrabbed = false;
		originalScale = transform.localScale;
		originalLocation = transform.position;
		timeToComplete = 5;
		factoryBehavior = Factory.GetComponent<FactoryBehavior>();

		atmosphereSlots = new GameObject[skyPanel.transform.childCount];
		for(int i = 0; i < skyPanel.transform.childCount; i++)
		{
			atmosphereSlots[i] = skyPanel.transform.GetChild(i).gameObject;
		}

		undergroundSlots = new GameObject[groundPanel.transform.childCount];
		for(int i = 0; i < skyPanel.transform.childCount; i++)
		{
			undergroundSlots[i] = groundPanel.transform.GetChild(i).gameObject;
		}

		int rand = Random.Range(0,clouds.Length);
		transform.FindChild("AtmosphericCarbon").GetComponent<Image>().sprite = clouds[rand];
		transform.FindChild("UndergroundCarbon").GetComponent<Image>().sprite = coals[rand];

		if (this.carbonState == CarbonState.ATMOSPHERIC){
			fillSlots("atmosphere");
		}
		if (this.carbonState == CarbonState.UNDERGROUND){
			fillSlots("ground");
		}
	}

	void SetState(CarbonState state){
		this.carbonState =  state;
		SetBounds(state);
	}

	void SetBounds(CarbonState state){
		if (this.carbonState == CarbonState.ATMOSPHERIC){
			carbonBoundRT = skyPanel.GetComponent<RectTransform>();
			workingPanel = skyPanel;
		}
		if (this.carbonState == CarbonState.UNDERGROUND){
			carbonBoundRT = groundPanel.GetComponent<RectTransform>();
			workingPanel = groundPanel;
		}
		boundMin.x = workingPanel.transform.position.x - carbonBoundRT.rect.width/2;
		boundMin.y = workingPanel.transform.position.y - carbonBoundRT.rect.height/2;
		boundMax.x = workingPanel.transform.position.x + carbonBoundRT.rect.width/2;
		boundMax.y = workingPanel.transform.position.y + carbonBoundRT.rect.height/2;
	}

//	We need to keep the objects from going out of the screen, or from ground to sky
	void Update(){
		if(transform.position.x > boundMax.x)
		{
			transform.position = new Vector2(boundMax.x, transform.position.y);
		}
		if(transform.position.x < boundMin.x)
		{
			transform.position = new Vector2(boundMin.x, transform.position.y);
		}
		if(transform.position.y > boundMax.y)
		{
			transform.position = new Vector2(transform.position.x, boundMax.y);
		}
		if(transform.position.y < boundMin.y)
		{
			transform.position = new Vector2(transform.position.x, boundMin.y);
		}
		// We cound do OnTriggerEnter, but I remember having some problems with it before.
		if(transform.position.x < factoryBoundMax.x && transform.position.x > factoryBoundMin.x
		 && transform.position.y < factoryBoundMax.y && transform.position.y > factoryBoundMin.y)
		{
			//gameManager.SendMessage("CarbonOnGoal");	// Do something in game manager, when carbon is in place

			//TODO: make mouse contract size by 10%

		}
	}

	public void OnMouseDrag()
	{
		if(gameManager.GetComponent<GameManager>().gameState != GameManager.GameState.INPLAY) return;
		if(!carbonGrabbed) carbonGrabbed = true;
		if (interactable){
			transform.position = Input.mousePosition;
		}

//	If we want to play Animation
//		anim.SetTrigger("NAME OF TRIGGER HERE");


//	If we want to play Audio on first drag only

//		if (slideFirst){
//			slideFirst = false;
//			audio.PlayOneShot(/*PUT AUDIO NAME HERE*/);
//		}
	}

	//called by OnMouseUp
	public void fillSlots(string slots){
		if (slots == "sequester"){ //sequestration queue
			for (int i =0; i < sequesterSlots.Length; i++){
				if(sequesterSlots[i].transform.childCount == 0){
					transform.SetParent(sequesterSlots[i].transform); //this doesn't seem to work now that the Factory is in a separate canvas
					transform.position = sequesterSlots[i].transform.position;
					transform.localScale = factoryScale;
					interactable = false;
					Instantiate(leafParticle, new Vector3 (transform.position.x, transform.position.y, -5), transform.rotation);
					audio.PlayOneShot(leafAudio);
					StartCoroutine("ProcessCarbon", timeToComplete);
					//transform.SetParent(worldCanvas.transform);
					i = sequesterSlots.Length;
				}
			}
		}
		if (slots == "burn"){ //burning queue
			for (int i =0; i < burnSlots.Length; i++){
				if(burnSlots[i].transform.childCount == 0){
					transform.SetParent(burnSlots[i].transform);
					transform.position = burnSlots[i].transform.position;
					transform.localScale = factoryScale;
					interactable = false;
					Instantiate(coalParticle, new Vector3 (transform.position.x, transform.position.y, -5), transform.rotation);
					audio.PlayOneShot(coalAudio);
					StartCoroutine("ProcessCarbon", timeToComplete);
					transform.SetParent(worldCanvas.transform);
					i = burnSlots.Length;
				}
			}
		}
		if (slots == "atmosphere"){ //fill the sky
			for (int i =0; i < atmosphereSlots.Length; i++){
				if(atmosphereSlots[i].transform.childCount == 0){
					transform.SetParent(atmosphereSlots[i].transform);
					transform.position = pipePosition.transform.position;
					transform.DOMoveX(atmosphereSlots[i].transform.position.x, 2);
					transform.DOMoveY(atmosphereSlots[i].transform.position.y, 1);

					transform.localScale = originalScale;
					//transform.SetParent(worldCanvas.transform);
					i = atmosphereSlots.Length;
				}
			}
		}
		if (slots == "ground"){ //fill the ground
			for (int i =0; i < undergroundSlots.Length; i++){
				if(undergroundSlots[i].transform.childCount == 0){
					transform.SetParent(undergroundSlots[i].transform);
					//transform.position = undergroundSlots[i].transform.position;
					transform.DOMoveX(undergroundSlots[i].transform.position.x, 2);
					transform.DOMoveY(undergroundSlots[i].transform.position.y, 1);
					transform.localScale = originalScale;
					//transform.SetParent(worldCanvas.transform);
					i = undergroundSlots.Length;
				}
			}
		}
	}

	public void OnMouseUp()
	{
		if(gameManager.GetComponent<GameManager>().gameState != GameManager.GameState.INPLAY) return;
		carbonGrabbed = false;
		print("RELEASED CARBON!");
		print("transform.position: " + transform.position);
		print("Min: " + factoryBoundMin + ", Max: " + factoryBoundMax);
		if(
			!(transform.position.x < factoryBoundMax.x 
		   && transform.position.x > factoryBoundMin.x
		   && transform.position.y < factoryBoundMax.y
		   && transform.position.y > factoryBoundMin.y)
		   )
		{
			transform.position = originalLocation;
			print("Underground carbon WAS NOT ADDED!");
			return;

		}
		if (carbonState == CarbonState.ATMOSPHERIC) {
			if(factoryBehavior.AddCarbonToSequestration(this.gameObject)) {
				fillSlots("sequester");
				print("Atmospheric Carbon WAS ADDED!");
				//TODO: animations for putting object into factory
			}
		}
		if (carbonState == CarbonState.UNDERGROUND) {
			if(factoryBehavior.AddCarbonToBurning (this.gameObject)) {
				fillSlots("burn");
				print("Underground carbon WAS ADDED!");
				//TODO: animations for putting object into factory
			}
		}

	}

	IEnumerator ProcessCarbon(float time){

		if (carbonState == CarbonState.ATMOSPHERIC) {
			transform.FindChild("AtmosphericCarbon").GetComponent<Image>().DOFade(0, time);
			int rand = Random.Range(0,coals.Length);
			transform.FindChild("UndergroundCarbon").GetComponent<Image>().sprite = coals[rand];
			transform.FindChild("UndergroundCarbon").GetComponent<Image>().DOFade(1, time);
			transform.DOMoveY(transform.position.y - 10, time);
			yield return new WaitForSeconds(time);
			SetState(CarbonState.UNDERGROUND);
			fillSlots("ground");
			print ("Filled ground");
		}
		else if (carbonState == CarbonState.UNDERGROUND) {
			transform.FindChild("UndergroundCarbon").GetComponent<Image>().DOFade(0, time);
			int rand = Random.Range(0,clouds.Length);
			transform.FindChild("AtmosphericCarbon").GetComponent<Image>().sprite = clouds[rand];
			transform.FindChild("AtmosphericCarbon").GetComponent<Image>().DOFade(1, time);
			transform.DOMoveY(transform.position.y + 10, time);
			yield return new WaitForSeconds(time);
			SetState(CarbonState.ATMOSPHERIC);
			fillSlots("atmosphere");
			print ("Filled sky");
		}
		interactable = true;
	}

// Set up in case we need variable cursors and carbon animation:

//	void OnMouseExit()
//	{
//		slideFirst = true;
//		anim.SetTrigger("NAME OF IDLE ANIMATION");
//		gameManager.SendMessage("NAME OF CURSOR");
//	}

// Set up in case we need variable cursors and carbon animation:

//	void OnMouseEnter()
//	{
//		anim.SetTrigger("NAME OF HOVER ANIMATION");
//		int random = Random.Range(0,coinSpins.Length);
//		audio.PlayOneShot(/*PUT AUDIO NAME HERE*/);
//		gameManager.SendMessage("NAME OF CURSOR");
//	}
}
