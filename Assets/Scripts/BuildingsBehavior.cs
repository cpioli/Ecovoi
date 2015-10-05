using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class BuildingsBehavior : MonoBehaviour {

	public float growthDuration;
	private Image[] buildings;
	public int currentBuilding;
	private int maxBuildings = 12;

	// Use this for initialization
	void Start () {
		buildings = this.gameObject.GetComponentsInChildren<Image>();
		currentBuilding = 0;
		for(int i = 0; i < maxBuildings; i++) {
			buildings[i].rectTransform.localScale = Vector3.zero;
		}
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void AddBuilding() {
		if(currentBuilding == maxBuildings) return;
		Image building = buildings[currentBuilding];
		building.rectTransform.DOScale (new Vector3(1f, 1f, 1f), growthDuration).SetEase (Ease.OutBounce);
		currentBuilding++;
	}

	public void RemoveBuilding() {
		//if(currentBuilding == 0) return;
		Image building = buildings[currentBuilding];
		if(building.rectTransform.localScale != Vector3.zero)
			building.rectTransform.DOScale( Vector3.zero, growthDuration).SetEase(Ease.Linear);
		currentBuilding--;
		if(currentBuilding < 0)
			currentBuilding = 0;
		print("Removing! currentBuilding is: " + currentBuilding);
	}
}
