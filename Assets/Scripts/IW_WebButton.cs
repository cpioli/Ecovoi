////////////////////////////////////////////////////////////////////////////////////////
// IW_Button_Internet
// Purpose:	TThis is the manual (button clicking) walkthrough of the inspirational text.
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(InternetReachabilityVerifier))]

public class IW_WebButton : MonoBehaviour
{
	public GameObject thisButton;
	//public Texture2D Active;
	//public Texture2D InActive;

	public string WhereTo01;
	public string MyName;

	//float targetTime = 0.5f;



	void Update()
	{

		if(Input.GetMouseButtonDown(0))
		{
		
			Debug.Log("ButtonScript");
			RaycastHit rc_hit;
			Ray hud_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(	Physics.Raycast(hud_ray, out rc_hit, Mathf.Infinity, 1<<LayerMask.NameToLayer("HUD")) )
			{
				if ( rc_hit.transform.gameObject == thisButton)
				{
					Debug.Log ("Touched");
					//audio.Play();
					Application.LoadLevel(WhereTo01);
					}


	
				}
			}
		}
	}
