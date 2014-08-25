using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts;

public class UnitBehaviour : MonoBehaviour 
{
	public HexTileBehaviour hexPosition;
	public int resourcePrice;
	public int energy;
	public int energyLimit;

	public int normalAttackStrength;
	//Thinking about adding a behaviour Enumeration, which decides what the unit does depending on how loud the own tileis, the neighbouring ones, and the loudest ones.
	//i.e. lets say a turtling type ignores the loudest tile and stays where he is, while a curious one get's distracted easily by it's neighbours, while focused ones only keep moving forward to the loudest one.
	//public int curiosiasdsa asd ty = 0;
	private GameObject infoBoard;
	TextMesh infoBoardText;
    

	public int currentAttackStrength {
				get {
			return Convert.ToInt32(normalAttackStrength / 100f * energy);
				}
		} 

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {	
	}

}