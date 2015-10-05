using UnityEngine;
using System.Collections;

public class IW_MusicBox : MonoBehaviour {

	//This is to be attached to an empty game object used to generate a sound of piece of music that is to be heard in all scenes.
	//This will check to see if there is already an instance of this game object in the scene and if so, it will delete itslef and 
	//allow the existing object to continue.

	private static IW_MusicBox instance = null;
	public static IW_MusicBox Instance {
			get { return instance; }
		}

	void Awake() {
			if (instance != null && instance != this) {
				Destroy(this.gameObject);
				return;
			} else {
				instance = this;
			}
			DontDestroyOnLoad(this.gameObject);
		}
		
		// any other methods you need
	}
			