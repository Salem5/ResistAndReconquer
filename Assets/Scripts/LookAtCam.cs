using UnityEngine;
using System.Collections;


public class LookAtCam : MonoBehaviour {

	public static GameObject cam;
	// Use this for initialization
	void Start () {
		cam = GameObject.Find ("Main Camera");	

	}
	
	// Update is called once per frame
	void Update () {
		

		transform.rotation = cam.transform.rotation;

	}
}
