using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class GuiTestBehaviour : MonoBehaviour {

	public GUISkin customSkin;
	public bool touchOptions;
	public Rect rightUnitTileInfoRect;

	public bool toggleTest;
	public string someText = "start";
	public Vector2 scrollTest = Vector2.zero;
	public Rect topScoreRect;
	public Rect bottomScrollRect;
	Vector2 scrollVector = Vector2.zero;
	float scrollFloat;
	//GameObject model1;
	//GameObject model2;

	WatchSelectedBehaviour guiObjectCam1;
	WatchSelectedBehaviour guiObjectCam2;


	public int marginDistance;


	// Use this for initialization
	void Start () {
		toggleTest = true;
		touchOptions = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WP8Player || Application.platform == RuntimePlatform.MetroPlayerX86 || Application.platform == RuntimePlatform.MetroPlayerX64 || Application.platform == RuntimePlatform.MetroPlayerARM);

		ScaleToScreen ();


		//guiObjectCam1 = GameObject.Find ("GuiObjectCam1").GetComponent<Camera> ();
		//guiObjectCam2 = GameObject.Find ("GuiObjectCam2").GetComponent<Camera> ();

		//model1 = guiObjectCam1.GetComponent<WatchSelectedBehaviour> ().selected;
		//model2 = guiObjectCam2.GetComponent<WatchSelectedBehaviour> ().selected;
		
			
		//guiObjectCam2.transform.position = new Vector3 (model2.transform.position.x ,model2.transform.position.y + 3,model2.transform.position.z - 5);
		//guiObjectCam2.transform.eulerAngles = new Vector3 (25,guiObjectCam2.transform.position.y,guiObjectCam2.transform.eulerAngles.z);


	}

	public void ScaleToScreen()
	{	
		marginDistance = 10;

		//y position is last
		topScoreRect = new Rect (5,0, Screen.width - 10, Screen.height / 5);
		rightUnitTileInfoRect = new Rect (Screen.width - Screen.width / 5, 0, Screen.width / 5 - 5, 0);
		bottomScrollRect =  new Rect( 0, 0 , Screen.width ,Screen.height/5 );
		
		if (!touchOptions) {
			marginDistance /=2;
			topScoreRect.height /= 2;
			bottomScrollRect.height /= 2;		
		}
		rightUnitTileInfoRect.y = topScoreRect.height ;
		rightUnitTileInfoRect.height = Screen.height - topScoreRect.height - bottomScrollRect.height ;
		bottomScrollRect.y = rightUnitTileInfoRect.height + topScoreRect.height ;	
		

	}


	// Update is called once per frame
	void Update () {

		//TODO: Needs wayyyy more testing.
		Touch touchRes = Input.touches.FirstOrDefault (t => t.phase == TouchPhase.Moved);

		if (IsTouchInsideList (touchRes.position, bottomScrollRect)) {			
						scrollFloat -= touchRes.deltaPosition.x;					
						wasDragging = true;
				} else {
			wasDragging = false;
				}

		//guiObjectCam1.transform.RotateAround (model1 .transform.position, Vector3.up, 25 * Time.deltaTime);
		//guiObjectCam2.transform.RotateAround (model1.transform.position, Vector3.up, 25 * Time.deltaTime);
	
	}

	bool wasDragging;


	bool IsTouchInsideList(Vector2 touchPos, Rect targetBounds)
	{
		Vector2 screenPos = new Vector2(touchPos.x, Screen.height - touchPos.y);  // invert y coordinate
			return targetBounds.Contains(screenPos);
	}

	void OnGUI()
	{
				GUI.skin = customSkin;

		//Display Round, Graphene and Next Round Button
						GUILayout.BeginArea (topScoreRect);
						GUILayout.BeginHorizontal (GUILayout.ExpandHeight (true));
						GUILayout.Label ("Round: x");
		GUILayout.Label ("Graphene: x");
						GUILayout.Button ("Ne. R.\t", GUILayout.ExpandHeight (true), GUILayout.ExpandWidth (false));
						GUILayout.EndHorizontal ();
						GUILayout.EndArea ();

						//Display Unit and Tile information
		GUILayout.BeginArea (rightUnitTileInfoRect);
		GUILayout.BeginVertical (GUILayout.ExpandWidth (true));

		GUILayout.FlexibleSpace ();
		string unitContent = "Unit: " + Environment.NewLine +
						"En:" + Environment.NewLine + 
						"ATK:";
		GUILayout.Box ( unitContent, GUILayout.ExpandHeight (false), GUILayout.ExpandWidth (true));

		GUILayout.FlexibleSpace ();
		string tileContent = "Tile: " + Environment.NewLine +
						"Cover:" + Environment.NewLine;
		GUILayout.Box(tileContent, GUILayout.ExpandHeight (false), GUILayout.ExpandWidth (true));
						GUILayout.EndVertical ();
						GUILayout.EndArea ();

						//Display Unit Orders and Tile Constructions
	

						int buttonWidth = Screen.width / 5;
						int buttonHeight = Screen.height / 5;

						if (!touchOptions) {

								buttonWidth /= 3;
						}

		//GUI.BeginGroup (new Rect (bottomScrollRect));
		GUI.BeginGroup (new Rect (-scrollFloat, bottomScrollRect.y  ,bottomScrollRect.width,bottomScrollRect.height));
		                
		                for (int i = 0; i < 15; i++) {
			toggleTest = GUI.Toggle (new Rect (i * (marginDistance + buttonWidth), marginDistance, buttonWidth, buttonHeight), toggleTest, "Test" + i);			
								toggleTest = wasDragging ? toggleTest : false;
						}

				//		GUI.EndGroup ();
		GUI.EndGroup ();

						if (touchOptions) {
			GUI.HorizontalScrollbar (new Rect (0, Screen.height - (marginDistance*2), Screen.width, (marginDistance*2)), scrollFloat, buttonWidth + marginDistance, 0, 15 * (marginDistance + buttonWidth) - Screen.width + buttonWidth + marginDistance);
						}
				
	}
}
