using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBehaviour : MonoBehaviour {

	public UnitBehaviour unitType;
	public List<Selectable> units;
    public List<Selectable> land;
	public bool neutralPlayer;
	public int score;
	public int graphene;
	public Color teamColor;
	private bool _lost;
	public bool lost 
	{
		get{ return _lost;}
		set{ 
			if (!neutralPlayer) 
			_lost = value;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {	
	}
}
	