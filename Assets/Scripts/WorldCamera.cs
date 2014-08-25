using UnityEngine;
using System.Linq;
using System.Collections;

public class WorldCamera : MonoBehaviour {

	public struct BoxLimit{
		public float LeftLimit, RightLimit, TopLimit, BottomLimit;
	}

	#region

	public static BoxLimit cameraLimits = new BoxLimit();
	public static BoxLimit mouseScrollLimits = new BoxLimit();
	public static WorldCamera Instance;
	public float cameraMoveSpeed = 60f;
	public float mouseBoundary = 24f;

	public float mouseX;
	public float mouseY;

	private bool verticalRotationEnabled = true;
	private float verticalRotationMinimum = 0f;//degrees
	private float verticalRotationMaximum = 65f;//degrees

	public Terrain worldTerrain;
	public float worldTerrainPadding = 25f;

	#endregion

	void Awake()
	{
		Instance = this;
		}

	// Use this for initialization
	void Start () {
        //Getting the CameraHeight component from the first child.
		ownCameraHeightB = GetComponentInChildren<CameraHeight> ();
		cameraLimits.LeftLimit = worldTerrain.transform.position.x + worldTerrainPadding;
		cameraLimits.RightLimit = worldTerrain.terrainData.size.x - worldTerrainPadding;
		cameraLimits.TopLimit = worldTerrain.terrainData.size.z + worldTerrainPadding;
		cameraLimits.BottomLimit = worldTerrain.transform.position.z + worldTerrainPadding;

		mouseScrollLimits.LeftLimit = mouseScrollLimits.TopLimit = mouseScrollLimits.RightLimit = mouseScrollLimits.BottomLimit = mouseBoundary;
	}

	CameraHeight ownCameraHeightB;


	// Update is called once per frame
	void LateUpdate () {

		//HandleMouseRotation ();

		byte inputRes = CheckIfUserCameraInput ();

		if (inputRes== 1) {
						Vector3 cameraDesiredMove = GetDesiredTranslation ();

						if (!isDesiredPositionOverBoundaries (cameraDesiredMove)) {
								this.transform.Translate (cameraDesiredMove);
						}
		} else if(inputRes == 2) {
            //spazzing around
			//float pinchDelta = Input.touches[0].deltaPosition.y - Input.touches[1].deltaPosition.y;
			//ownCameraHeightB.betweenAmount += pinchDelta * Time.deltaTime * ownCameraHeightB.speed * ownCameraHeightB.direction;
			//ownCameraHeightB.betweenAmount = Mathf.Clamp (ownCameraHeightB.betweenAmount , 0F, 1F);
			//transform.eulerAngles = new Vector3( Mathf.Lerp (ownCameraHeightB.minRotVector.x,ownCameraHeightB.maxRotVector.x, ownCameraHeightB.betweenAmount),Mathf.Lerp (ownCameraHeightB.minRotVector.y,ownCameraHeightB.maxRotVector.y,ownCameraHeightB.betweenAmount),Mathf.Lerp (ownCameraHeightB.minRotVector.z,ownCameraHeightB.maxRotVector.z,ownCameraHeightB.betweenAmount));
			//transform.localPosition = new Vector3(Mathf.Lerp (ownCameraHeightB.minPosVector.x,ownCameraHeightB.maxPosVector.x, ownCameraHeightB.betweenAmount),Mathf.Lerp (ownCameraHeightB.minPosVector.y,ownCameraHeightB.maxPosVector.y,ownCameraHeightB.betweenAmount),Mathf.Lerp (ownCameraHeightB.minPosVector.z,ownCameraHeightB.maxPosVector.z,ownCameraHeightB.betweenAmount));


				}

		mouseX = Input.mousePosition.x;
		mouseY = Input.mousePosition.y;		
	}

	public void HandleMouseRotation ()
	{
				var easeFactor = 10f;
				if (Input.GetMouseButton (1)) {
						if (Input.mousePosition.x != mouseX) {
								var cameraRotationY = (Input.mousePosition.x - mouseX) * easeFactor * Time.deltaTime;
								this.transform.Rotate (0, cameraRotationY, 0);
						}
				

						//Vertical Rotation

						if (verticalRotationEnabled && Input.mousePosition.y != mouseY) {
		
								var cameraRotationX = (mouseY - Input.mousePosition.y) * easeFactor * Time.deltaTime;
								var desiredRotationX = Camera.main.transform.eulerAngles.x + cameraRotationX;

								if (desiredRotationX >= verticalRotationMinimum && desiredRotationX <= verticalRotationMaximum) {
										Camera.main.transform.Rotate (cameraRotationX, 0, 0);
								}
						}
				}
		}

	public byte CheckIfUserCameraInput()
	{
		bool keyboardMove;
		bool mouseMove;

		keyboardMove = WorldCamera.AreCameraKeyboardButtonPressed ();
		mouseMove = WorldCamera.IsMousePositionWithinBoundaries ();

		if (mouseMove || keyboardMove || Input.touches.Count() == 1) {
			return 1;
				}
		else if (Input.touches.Count() > 1) {
			return 2;
				}
		else {
			return 0;
		}
	}

	public Vector3 GetDesiredTranslation()
	{
				float moveSpeed = 0f;
				float desiredX = 0f;
				float desiredZ = 0f;

				moveSpeed = cameraMoveSpeed * Time.deltaTime;

				desiredX += Input.GetAxis ("ScrollHorizontal");
				desiredZ += Input.GetAxis ("ScrollVertical");

		if (!GameObject.Find("Main").GetComponent<MainBehaviour>().insideControl) {
			Touch touchRes = Input.touches.FirstOrDefault (t => t.phase == TouchPhase.Moved);

			desiredX += -touchRes.deltaPosition.x/8F;
			desiredZ += -touchRes.deltaPosition.y/4F;
				}

		return new Vector3 (desiredX, 0, desiredZ);
		}

	public bool isDesiredPositionOverBoundaries (Vector3 desiredPosition)
	{
				if ((this.transform.position.x + desiredPosition.x) < cameraLimits.LeftLimit)
		{ return true; }

				if ((this.transform.position.x + desiredPosition.x) > cameraLimits.RightLimit)
		{ return true; }

				if ((this.transform.position.z + desiredPosition.z) < cameraLimits.BottomLimit)
		{ return true; }

				if ((this.transform.position.z + desiredPosition.z) > cameraLimits.TopLimit)
		{ return true; }

		return false;						
	}



	public static bool AreCameraKeyboardButtonPressed()
	{
			return (
			Mathf.Abs(Input.GetAxis("ScrollHorizontal")) > 0.1 ||
			Mathf.Abs(Input.GetAxis("ScrollVertical")) > 0.1 
		        );	
		}

	public static bool IsMousePositionWithinBoundaries()
	{
		return (
			(Input.mousePosition.x < mouseScrollLimits.LeftLimit && Input.mousePosition.x > -5) ||
			(Input.mousePosition.x > (Screen.width - mouseScrollLimits.RightLimit) && Input.mousePosition.x < (Screen.width + 5)) ||
			(Input.mousePosition.y < mouseScrollLimits.BottomLimit && Input.mousePosition.y > -5) ||
			(Input.mousePosition.y > (Screen.height - mouseScrollLimits.TopLimit) && Input.mousePosition.y < (Screen.height + 5))
			);

	}
}
