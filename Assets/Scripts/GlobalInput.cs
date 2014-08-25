using UnityEngine;
using System.Collections;
using System;

public class GlobalInput : MonoBehaviour {

	bool oldF12Pressed = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate()
	{
		//Fast reset for testing.
		if (Input.GetKey(KeyCode.F10)) {
			Application.LoadLevel(Application.loadedLevel);
		}
				
		//making screenshots
		
		bool newF12Pressed = Input.GetKey (KeyCode.F12);
		
		if(newF12Pressed && !oldF12Pressed){
			Application.CaptureScreenshot("Screenshot " + DateTime.Now.ToString("MM_dd_yy H-mm-ss") + ".png",2);
		}		
	}
}
