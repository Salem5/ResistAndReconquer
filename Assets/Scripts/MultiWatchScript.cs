using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MultiWatchScript : MonoBehaviour {

	public List<GameObject> objectsToWatch;
	public int transitionStartObjectIndex;
	public int transitionEndObjectIndex;
	public float horizontalVeer;
	//seconds
	public int idleDuration; 
	public int transitionDuration; 

	public DateTime lastSwitch;
	//public bool transitioning;
	//public Vector3 transitionPosition;
//	public float lerpValue;

	// Use this for initialization
	void Start () {
		this.GetComponent<WatchSelectedBehaviour> ().setSelected (objectsToWatch [transitionEndObjectIndex]);
		lastSwitch = DateTime.Now;
		Time.timeScale = 1;
	}


	// Update is called once per frame
	void LateUpdate () {

		if (lastSwitch.AddSeconds (idleDuration) < DateTime.Now) {
						transitionStartObjectIndex = transitionEndObjectIndex;
						transitionEndObjectIndex += 1;
						if (transitionEndObjectIndex == objectsToWatch.Count) {
								transitionEndObjectIndex = 0;
						}

			this.GetComponent<WatchSelectedBehaviour> ().transitionTo(objectsToWatch [transitionEndObjectIndex],transitionDuration);
						
			lastSwitch = DateTime.Now;
				}

	}
}
