using UnityEngine;
using System.Collections;

public class WatchSelectedBehaviour : MonoBehaviour {

	//TODO: make in to property for layers switching
	public GameObject selected;
	public GameObject nextSelected;
	public int watcherLayer;
	public int oldSelectedLayer;
	public Vector3 currentTargetPosition;
	public float horizontalVeer;

	public bool inTransition;
	public float transitionDuration; 
	public float lerpVal;
	//public bool transitionOver;

	public void setSelected(GameObject aSelected)
	{
		if (selected != null) {		
			SetLayerRecursively (selected,oldSelectedLayer);
		}

		if (aSelected != null) {
			// adding to current rotation
			setPositionAndRotation(aSelected);

						selected = aSelected;
						oldSelectedLayer = selected.layer;
						SetLayerRecursively (aSelected, watcherLayer);
				}
		else {
			selected = null;
		}
	}

	public void transitionTo(GameObject aNextSelected, float aDuration)
	{
		//horizontalVeer = aHorizontalVeer;
		nextSelected = aNextSelected;
		inTransition = true;
		lerpVal = 0;
		lerpSeconds = 0;
		transitionDuration = aDuration;
	}

	float lerpSeconds =0;

	// Update is called once per frame
	void LateUpdate () {
		if (selected == null) {
			return;
		}

		if (inTransition) {

						if (lerpVal > 1) {
								inTransition = false;
								lerpVal = 0;
				setSelected(nextSelected);
						} else {
				lerpSeconds += Time.deltaTime;
				lerpVal = lerpSeconds / transitionDuration;

				//Vector3 lerpVector = Vector3.MoveTowards(selected.transform.position,nextSelected.transform.position,Time.deltaTime * 3);

								Vector3 lerpVector = new Vector3 (Mathf.Lerp (selected.transform.position.x, nextSelected.transform.position.x, lerpVal),
				                                 Mathf.Lerp (selected.transform.position.y, nextSelected.transform.position.y, lerpVal),
				                                 Mathf.Lerp (selected.transform.position.z, nextSelected.transform.position.z, lerpVal));

								setPositionAndRotationInBetween (lerpVector);
						}
				}
		else if (currentTargetPosition != selected.transform.position) {

			setPositionAndRotation(selected);
		}
		this.transform.RotateAround (currentTargetPosition, Vector3.up, 25 * Time.deltaTime);
		
	}

	private void setPositionAndRotation(GameObject sourceObject)
	{
		if (selected == null) {
			this.transform.position = new Vector3 (sourceObject.transform.position.x, sourceObject.transform.position.y + 3, sourceObject.transform.position.z - 5);	
			this.transform.eulerAngles = new Vector3 (25, 0, 0);
				}
		else {
			Vector3 differenceOfSelecs = sourceObject.transform.position - currentTargetPosition;
			this.transform.position = this.transform.position + differenceOfSelecs;
				}

		currentTargetPosition = sourceObject.transform.position;
	}

	private void setPositionAndRotationInBetween(Vector3 sourcePosition)
	{
		{
			Vector3 differenceOfSelecs = sourcePosition - currentTargetPosition;
			this.transform.position = this.transform.position + differenceOfSelecs;
		}
		
		currentTargetPosition = sourcePosition;
	}

	void SetLayerRecursively(GameObject obj, int newLayer)	
	{		
		if (null == obj)			
		{			
			return;			
		}		
		
		obj.layer = newLayer;
		
		foreach (Transform child in obj.transform)			
		{			
			if (null == child)				
			{				
				continue;				
			}
			
			SetLayerRecursively(child.gameObject, newLayer);			
		}		
	}

	// Use this for initialization
	void Start () {

	}
	

}

