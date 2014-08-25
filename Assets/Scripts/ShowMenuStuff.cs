using UnityEngine;
using System.Collections;
using System;

public class ShowMenuStuff : MonoBehaviour {
	public GUISkin customSkin;
	public bool showStageSelect;

	// Use this for initialization
	void Start () {
		GetComponent<AudioSource> ().Play ();
	}
	
	// Update is called once per frame
	void Update () {
	//stages = new string[]{ "Alpha"};
	}


	//public String[] stages; = new string[]{ "Alpha", "Nope", "Nyet","Nada", "U uh","Unyes" };


	public String[] stages;// = new string[]{ "Alpha"};
	public int selectedStage = -1;

	void OnGUI()
	{		
		GUI.skin = customSkin;

		if (showStageSelect) {

			 
			GUILayout.BeginArea (new Rect (0 ,Screen.height /4 , Screen.width / 2,Screen.height/2) );
			//GUILayout.FlexibleSpace ();
			GUILayout.Label ( "Select Stage");

			GUILayout.BeginVertical( GUILayout.ExpandHeight(true) );
			selectedStage = GUILayout.SelectionGrid(selectedStage, stages,3, GUILayout.ExpandHeight(true) );


//			//row 1
//			GUILayout.BeginHorizontal();
//			GUI.enabled = true;
//			if (GUILayout.Button ("Alpha",GUILayout.ExpandWidth(true))) {
//				Application.LoadLevel(1);
//				//showStageSelect = false;
//			}
//			GUI.enabled = true;
//
//			GUI.enabled = false;
//			if (GUILayout.Button ("Nope",GUILayout.ExpandWidth(true))) {
//				//Application.LoadLevel(1);
//				//showStageSelect = false;
//			}
//			GUI.enabled = true;
//
//			GUI.enabled = false;
//			if (GUILayout.Button ("Nyet",GUILayout.ExpandWidth(true))) {
//				//Application.LoadLevel(1);
//				//showStageSelect = false;
//			}
//			GUI.enabled = true;
//
//			GUILayout.EndHorizontal();
//
//			//row 2
//			GUILayout.BeginHorizontal();
//			GUI.enabled = false;
//			if (GUILayout.Button ("Nada",GUILayout.ExpandWidth(true))) {
//				//Application.LoadLevel(1);
//				//showStageSelect = false;
//			}
//			GUI.enabled = true;
//
//			GUI.enabled = false;
//			if (GUILayout.Button ("U uh",GUILayout.ExpandWidth(true))) {
//				//Application.LoadLevel(1);
//				//showStageSelect = false;
//			}
//			GUI.enabled = true;
//
//			GUI.enabled = false;
//			if (GUILayout.Button ("Unyes",GUILayout.ExpandWidth(true))) {
//				//Application.LoadLevel(1);
//				//showStageSelect = false;
//			}
//			GUI.enabled = true;
//
//			GUILayout.EndHorizontal();
//			//return
//

			GUILayout.BeginHorizontal();

			GUI.enabled = (selectedStage >= 0);

			if (GUILayout.Button ("START SELECTED",GUILayout.ExpandHeight(true))) {
				//TODO: Doing something with selected stage.
				Application.LoadLevel(1);
			}

			GUI.enabled = true;

			if (GUILayout.Button ("RETURN",GUILayout.ExpandHeight(true))) {
				showStageSelect = false;
			}

			GUILayout.EndHorizontal();
//
			GUILayout.EndVertical();

			//GUILayout.FlexibleSpace ();
			GUILayout.EndArea ();


				} 
		else {

			GUILayout.BeginArea (new Rect (0 ,Screen.height/4 , Screen.width / 2,Screen.height/2));
			//GUILayout.FlexibleSpace ();
			GUILayout.Label ( "Resist & Reconquer - Early Alpha v0.9\n" +
			                 "Start a new game or read the quickstart guide by pressing on \"Instructions\"" +
			                 "\nThis is a battle for survival, so try your best to defeat the enemy! Good Luck :)\nMade by Omar Ajerray"
			                 );
			
			if (GUILayout.Button ("BEGIN CHALLENGE",GUILayout.ExpandHeight(true))) {
				//Application.LoadLevel(1);
				showStageSelect = true;
			}
		
			//string path= "file:///"+ Application.dataPath + "/Guide/QuickstartGuide.html";
			string path= "http://ajerray.info/resist-and-reconquer-guide";
			
			if (GUILayout.Button ("INSTRUCTIONS (Opens browser)",GUILayout.ExpandHeight(true))) {
				
				Application.OpenURL(path);
			}
			
			if (GUILayout.Button ( "EXIT",GUILayout.ExpandHeight(true)))
			{
				Application.Quit();
			}
			
			//GUILayout.FlexibleSpace ();
			GUILayout.EndArea ();

		}
		
	}
}