////////////////////////////////////////////////////////////////////////////////////////
// IWTestingMenu
// Purpose:	The starting point for the application. Typical usage would be for menus.
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

public class MoveToLoc : MonoBehaviour

{
	public Transform startMarker;
	public Transform endMarker;

	public AudioClip PING;

	public float speed = 1.0F;
	private float startTime;
	private float journeyLength;

	bool Pyd;

	public bool Sound;
	

	void Start() {
		startTime = Time.time;
		journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
		Pyd = false;
		}

	void Update() {
		float distCovered = (Time.time - startTime) * speed;
		float fracJourney = distCovered / journeyLength;
		transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);

		//Debug.Log ("Obj" + transform.position);
		//Debug.Log ("End" + endMarker.position);
		pingIt();

		}

	void pingIt()
	{
		if (transform.position == endMarker.position & Pyd == false)
		{
			if (Sound == true)
			{
				Debug.Log ("DONE");
				audio.Play();
				//audio.PlayOneShot(PING, 0.5F);
				Pyd = true;
			}
			else
			{
				Debug.Log (" QUIET DONE");
				//audio.Play();
				//audio.PlayOneShot(PING, 0.5F);
				Pyd = true;
			}

		}
	}
}
