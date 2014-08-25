using UnityEngine;
using System.Collections;

public class CameraHeight : MonoBehaviour {

	public Vector3 minPosVector;
	public Vector3 maxPosVector;
	public Vector3 minRotVector;
	public Vector3 maxRotVector;
	// should be between 0.0 and 1.0
	public float betweenAmount;
	public float speed;
	public int direction;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
void LateUpdate () {

		//changing the current camera by scrollWheel Input
		betweenAmount += Input.GetAxis ("Mouse ScrollWheel")* Time.deltaTime * speed * direction;
		betweenAmount = Mathf.Clamp (betweenAmount , 0F, 1F);
		transform.eulerAngles = new Vector3( Mathf.Lerp (minRotVector.x,maxRotVector.x, betweenAmount),Mathf.Lerp (minRotVector.y,maxRotVector.y,betweenAmount),Mathf.Lerp (minRotVector.z,maxRotVector.z,betweenAmount));
		transform.localPosition = new Vector3(Mathf.Lerp (minPosVector.x,maxPosVector.x, betweenAmount),Mathf.Lerp (minPosVector.y,maxPosVector.y,betweenAmount),Mathf.Lerp (minPosVector.z,maxPosVector.z,betweenAmount));

	}
}
