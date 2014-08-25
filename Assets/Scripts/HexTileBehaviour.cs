using UnityEngine;
using System.Collections;
using System;

public class HexTileBehaviour : MonoBehaviour {
	public Selectable containedUnit;

	public HextTileDefinitions hexTileDefintion;
	public Passability passable;
	 	public Cover cover;
	public double noise = -1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {	
	}	

	void LateUpdate () {	

	}


}



//has the names of enum values hardcoded
//Could be helpfull for localization
public static class EnumNames
{
	public static string GetEasyName(this Enum aEnum)
	{
		if (aEnum.GetType() == typeof(Passability)) {
			return "Nope, not implemented yet";
				}
		else if(aEnum.GetType() == typeof(HextTileDefinitions)) {

			switch (((HextTileDefinitions)aEnum)) {
			case HextTileDefinitions.bunker:
				{
					return "Bunker";
				}

			case HextTileDefinitions.classic:
				{
					return "Classic";
				}
			case HextTileDefinitions.spawner:
				{
					return "Spawner";
				}
			default:
				return "Nope, HextTileDefinitions enum not found";
			}
		}

		else if(aEnum.GetType() == typeof(Cover)) {
			switch (((Cover)aEnum)) {
			case Cover.extreme:
			{
				return "Extreme";
			}
			case Cover.high:
			{
				return "High";
			}

			case Cover.normal:
			{
				return "Normal";
			}

			case Cover.low:
			{
				return "Low";
			}

			case Cover.none:
			{
				return "None";
			}


			default:
				return "Nope, Cover enum not found";

			}
		}
		else {
			return "Nope, not found";
		}


	}

}
	
public enum HextTileDefinitions
{
	classic = 0,
	spawner = 1,
	bunker = 2,
	mine = 3,
	factory = 4
}

public enum Passability
{
	impassable = -1,
	fast  = 0,
	normal = 1,
	slow = 2,
}

public enum Cover
{
	none = 1,
	low  = 2,
	normal = 3,
	high = 4,
	extreme = 5,
}