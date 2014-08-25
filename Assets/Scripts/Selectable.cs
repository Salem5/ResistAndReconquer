using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using Assets.Scripts;

public class Selectable : MonoBehaviour, IHasNeighbours<Selectable> {
	public int positionQ = 0;
	public int positionR = 0;
	public byte rotationIndex = 0;
	public Color normalColor;
	public PlayerBehaviour team;

	public int currentLoudness;
	public int baseLoudness;
	public List<Order> orderCollection= new List<Order> ();
    public OrderLevel availableOrders;

	public Func<Selectable, bool> InField;

	private bool isMouseDown;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown() {

		isMouseDown = true;
	}

	void OnMouseUp()
	{
		if (isMouseDown) {
			GameObject.Find("Main").GetComponent<MainBehaviour> ().clickedSelectable = this;
				}
		isMouseDown = false;
		}

	void OnMouseExit() {
		isMouseDown = false;
	}
	
	public override int GetHashCode ()
	{
		return this.positionQ * 10000 + this.positionR;
	}

	public override bool Equals (object o)
	{
		Selectable temp = o as Selectable;

		if (temp == null) {
			return false;
				}

		HexTileBehaviour myTile = temp.GetComponent<HexTileBehaviour> ();

		if (myTile == null) {
			return base.Equals(o);
				}

		HexTileBehaviour tempTile = temp.GetComponent<HexTileBehaviour> ();

		if (tempTile  == null) {
			return base.Equals(o);
		}

		return (positionQ == temp.positionQ && positionR == temp.positionR);
	}
	
	public IEnumerable<Selectable> Neighbours
	{
		get
		{			
			List<Selectable> resList = new List<Selectable>(6);                
			HexKey key;

			for (int i = 0; i < 6; i++)
			{
				key = new HexKey(this.positionQ + Selectable.PossibleNeighbour[i][0],this.positionR + Selectable.PossibleNeighbour[i][1]);
				int resCost = GameFieldBehaviour.GetMapCost(key);

				if (resCost > -1) 
				{				
					HexTileBehaviour theTile = GameFieldBehaviour.hexTiles[key] as HexTileBehaviour;
					if (theTile == null || (theTile.containedUnit != null)) {
						continue;
					}
					resList.Add( theTile.GetComponent<Selectable>());
				}
			}
			return resList;
		}
	}

	public IEnumerable<Selectable> NeighbouringUnits
	{
		get
		{			
			List<Selectable> resList = new List<Selectable>(6);                
			HexKey key;
			
			for (int i = 0; i < 6; i++)
			{
				key = new HexKey(this.positionQ + Selectable.PossibleNeighbour[i][0],this.positionR + Selectable.PossibleNeighbour[i][1]);
				int resCost = GameFieldBehaviour.GetMapCost(key);
				
				if (resCost > -1) 
				{				
					HexTileBehaviour theTile = GameFieldBehaviour.hexTiles[key] as HexTileBehaviour;
					if (theTile == null || theTile.containedUnit == null) {
						continue;
					}
					resList.Add( theTile.containedUnit.GetComponent<Selectable>());
				}
			}
			return resList;
		}
	}

	public void selectMarker ()
	{

		this.transform.FindChild ("Model").renderer.material.SetFloat ("_Outline", 0.005f);
		//this.transform.FindChild("Model").renderer.material.shader = Shader.Find("Self-Illumin/Specular");
	}
	
	public void deSelectMarker()
	{
		this.transform.FindChild ("Model").renderer.material.SetFloat ("_Outline", 0);
		//this.transform.FindChild("Model").renderer.material.shader = Shader.Find("Diffuse");
	}

	public static List<int[]> PossibleNeighbour = new List<int[]>(6) {
		new int[]{0,1},
		new int[]{1,0},
		new int[]{1,-1},
		new int[]{0,-1},
		new int[]{-1,0},
		new int[]{-1,1},
	};
}
