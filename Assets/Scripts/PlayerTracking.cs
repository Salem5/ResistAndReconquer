using UnityEngine;
using System.Collections;
using System;

public class PlayerTracking : MonoBehaviour {
	public PlayerBehaviour playerToTrack;
	public MainBehaviour mainGoBehav;
	public GUISkin customSkin;
	// Use this for initialization

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		GUI.skin = customSkin;

				this.GetComponent<GUIText> ().text = 
			"Round: " + mainGoBehav.round + Environment.NewLine +
				"Score: " + this.playerToTrack.score + Environment.NewLine +
				"Silicon: " + this.playerToTrack.graphene
				;

				if (mainGoBehav.gameOver) {
						if (GUI.Button (new Rect (Screen.width / 3, Screen.height / 3, Screen.width / 3, Screen.height / 3), "Game Over!" + Environment.NewLine +
								"Rounds: " + mainGoBehav.round + Environment.NewLine +
								"Final Score: " + this.playerToTrack.score + Environment.NewLine +
								"Click to return to menu.")) {
								Application.LoadLevel (0);
						}
				}
	}
}