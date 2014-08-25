using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;

public class GameFieldBehaviour : MonoBehaviour {

	//public static Hashtable hexTiles;
	public static Dictionary<HexKey,HexTileBehaviour> hexTiles;
	public static float size = 1;
	public GameObject mainGo;
	public static int scale = 5;

	// Use this for initialization

	public static int GetMapCost(int x,int y)
	{
		HexKey key = new HexKey (x, y);
		if (hexTiles[key] == null) {
			return(-1);
				}

		return ((int) ((HexTileBehaviour)hexTiles[key]).passable);
	}

	public static int GetMapCost(HexKey aKey)
	{
		if (!hexTiles.ContainsKey(aKey)) {
			return(-1);
		}
		
		return ((int) ((HexTileBehaviour)hexTiles[aKey]).passable);
	}

	private HexTileBehaviour createPlainField(int posQ, int posR, PlayerBehaviour player)
	{		
		GameObject tempGo = ((HexTileBehaviour)Instantiate (GameObject.Find("Main").GetComponent<MainBehaviour>().hexTileTypes[0])).gameObject;		
		tempGo.transform.parent = this.transform;
		Selectable tempGoSelBehav = tempGo.GetComponent<Selectable> ();
		HexTileBehaviour hexTileBehav = tempGo.GetComponent<HexTileBehaviour> ();
		tempGo.transform.localScale = scaleHexagon();
		tempGo.name = "Plains";
		hexTileBehav.passable = Passability.normal;
		MainBehaviour.setTeam(tempGoSelBehav, player);
		//Adding the tile to the players land list.
		player.land.Add(tempGoSelBehav);
		tempGoSelBehav.positionQ =  posQ;
		tempGoSelBehav.positionR = posR ;
		Vector3 pos = placeHexagon (tempGoSelBehav.positionQ, tempGoSelBehav.positionR,0, size,scale) ;
		tempGo.transform.position = pos;
		tempGoSelBehav.deSelectMarker();

		return hexTileBehav;
		}

	private HexTileBehaviour createMineField(int posQ, int posR, PlayerBehaviour player)
	{		
		GameObject tempGo = ((HexTileBehaviour)Instantiate (GameObject.Find("Main").GetComponent<MainBehaviour>().hexTileTypes[4])).gameObject;		
		tempGo.transform.parent = this.transform;
		Selectable tempGoSelBehav = tempGo.GetComponent<Selectable> ();
		HexTileBehaviour hexTileBehav = tempGo.GetComponent<HexTileBehaviour> ();
		tempGo.transform.localScale = scaleHexagon();
		tempGo.name = "Mine";
		hexTileBehav.passable = Passability.normal;
		MainBehaviour.setTeam(tempGoSelBehav, player);
		//Adding the tile to the players land list.
		player.land.Add(tempGoSelBehav);
		tempGoSelBehav.positionQ =  posQ;
		tempGoSelBehav.positionR = posR ;
		Vector3 pos = placeHexagon (tempGoSelBehav.positionQ, tempGoSelBehav.positionR,0, size,scale) ;
		tempGo.transform.position = pos;
		tempGoSelBehav.deSelectMarker();
		
		return hexTileBehav;
	}

	//generates a bunker field
	private HexTileBehaviour createBunkerField(int posQ, int posR, PlayerBehaviour player)
	{		
		GameObject tempGo = ((HexTileBehaviour)Instantiate (GameObject.Find("Main").GetComponent<MainBehaviour>().hexTileTypes[2])).gameObject;		
		tempGo.transform.parent = this.transform;
		Selectable tempGoSelBehav = tempGo.GetComponent<Selectable> ();
		HexTileBehaviour hexTileBehav = tempGo.GetComponent<HexTileBehaviour> ();
		tempGo.transform.localScale = scaleHexagon();
		tempGo.name = "Bunker";
		hexTileBehav.passable = Passability.normal;
		MainBehaviour.setTeam(tempGoSelBehav, player);
		player.land.Add(tempGoSelBehav);
		tempGoSelBehav.positionQ =  posQ;
		tempGoSelBehav.positionR = posR ;
		//tempGoSelBehav.orderCollection.Add (OrderStatic.SandBags ());
		Vector3 pos = placeHexagon (tempGoSelBehav.positionQ, tempGoSelBehav.positionR,0, size,scale) ;
		tempGo.transform.position = pos;
		tempGoSelBehav.deSelectMarker();

		return hexTileBehav;
	}


	private HexTileBehaviour createSpawnerField(int posQ, int posR, PlayerBehaviour player)
	{		
		GameObject tempGo = ((HexTileBehaviour)Instantiate (GameObject.Find("Main").GetComponent<MainBehaviour>().hexTileTypes[1])).gameObject;		
		tempGo.transform.parent = this.transform;
		Selectable tempGoSelBehav = tempGo.GetComponent<Selectable> ();
		HexTileBehaviour hexTileBehav = tempGo.GetComponent<HexTileBehaviour> ();
		tempGo.transform.localScale = scaleHexagon();
		tempGo.name = "Spawner";
		hexTileBehav.passable = Passability.normal;
		MainBehaviour.setTeam(tempGoSelBehav, player);
			player.land.Add(tempGoSelBehav);
		tempGoSelBehav.positionQ =  posQ;
		tempGoSelBehav.positionR = posR ;
		Vector3 pos = placeHexagon (tempGoSelBehav.positionQ, tempGoSelBehav.positionR,0, size,scale) ;
		tempGo.transform.position = pos;
		tempGoSelBehav.deSelectMarker();

		return hexTileBehav;
	}

	private HexTileBehaviour createMainBaseField(int posQ, int posR, PlayerBehaviour player)
	{		
		GameObject tempGo = ((HexTileBehaviour)Instantiate (GameObject.Find("Main").GetComponent<MainBehaviour>().hexTileTypes[3])).gameObject;		
		tempGo.transform.parent = this.transform;
		Selectable tempGoSelBehav = tempGo.GetComponent<Selectable> ();
		HexTileBehaviour hexTileBehav = tempGo.GetComponent<HexTileBehaviour> ();
		tempGo.transform.localScale = scaleHexagon();
		tempGo.name = "Base";
		hexTileBehav.passable = Passability.normal;
		MainBehaviour.setTeam(tempGoSelBehav, player);
		player.land.Add (tempGoSelBehav);
		tempGoSelBehav.positionQ =  posQ;
		tempGoSelBehav.positionR = posR ;
		Vector3 pos = placeHexagon (tempGoSelBehav.positionQ, tempGoSelBehav.positionR,0, size,scale) ;
		tempGo.transform.position = pos;
		tempGoSelBehav.deSelectMarker();
		
		return hexTileBehav;
	}

	private HexTileBehaviour createConstructionField(int posQ, int posR, PlayerBehaviour player)
	{		
		GameObject tempGo = ((HexTileBehaviour)Instantiate (GameObject.Find("Main").GetComponent<MainBehaviour>().hexTileTypes[5])).gameObject;		
		tempGo.transform.parent = this.transform;
		Selectable tempGoSelBehav = tempGo.GetComponent<Selectable> ();
		HexTileBehaviour hexTileBehav = tempGo.GetComponent<HexTileBehaviour> ();
		tempGo.transform.localScale = scaleHexagon();
		tempGo.name = "Facility";
		hexTileBehav.passable = Passability.normal;
		MainBehaviour.setTeam(tempGoSelBehav, player);
		player.land.Add (tempGoSelBehav);
		tempGoSelBehav.positionQ =  posQ;
		tempGoSelBehav.positionR = posR ;
		Vector3 pos = placeHexagon (tempGoSelBehav.positionQ, tempGoSelBehav.positionR,0, size,scale) ;
		tempGo.transform.position = pos;
		tempGoSelBehav.deSelectMarker();
		
		return hexTileBehav;
	}

    private void generateMap1()
	{        
		//hexTiles = new Hashtable();
		hexTiles = new Dictionary<HexKey, HexTileBehaviour> ();



		//getting players
		PlayerBehaviour aiPBH = GameObject.Find("Ai").GetComponent<PlayerBehaviour>();
		PlayerBehaviour humanPBH = GameObject.Find("Human").GetComponent<PlayerBehaviour>();
		PlayerBehaviour neutralPBH = GameObject.Find("Neutral").GetComponent<PlayerBehaviour>();
     
		//q-3
		hexTiles.Add (new HexKey (-3, 8), createPlainField (-3, 8,neutralPBH));
		hexTiles.Add (new HexKey (-3, 7), createPlainField (-3, 7,neutralPBH));
		hexTiles.Add (new HexKey (-3, 6), createPlainField (-3, 6,neutralPBH));
		hexTiles.Add (new HexKey (-3, 5), createPlainField (-3, 5,neutralPBH));
		hexTiles.Add (new HexKey (-3, 4), createPlainField (-3, 4,neutralPBH));
		hexTiles.Add (new HexKey (-3, 3), createPlainField (-3, 3,neutralPBH));
		hexTiles.Add (new HexKey (-3, 2), createPlainField (-3, 2,neutralPBH));
		hexTiles.Add (new HexKey (-3, 1), createPlainField (-3, 1,neutralPBH));

		//q-2
		hexTiles.Add (new HexKey (-2, 8), createPlainField (-2, 8,neutralPBH));
		hexTiles.Add (new HexKey (-2, 7), createPlainField (-2, 7,neutralPBH));
		hexTiles.Add (new HexKey (-2, 6), createPlainField (-2, 6,neutralPBH));
		hexTiles.Add (new HexKey (-2, 5), createPlainField (-2, 5,neutralPBH));
		hexTiles.Add (new HexKey (-2, 4), createPlainField (-2, 4,neutralPBH));
		hexTiles.Add (new HexKey (-2, 3), createPlainField (-2, 3,neutralPBH));
		hexTiles.Add (new HexKey (-2, 2), createConstructionField (-2, 2,neutralPBH));
		hexTiles.Add (new HexKey (-2, 1), createPlainField (-2, 1,neutralPBH));
		hexTiles.Add (new HexKey (-2, 0), createPlainField (-2, 0,neutralPBH));

		//q-1
		hexTiles.Add (new HexKey (-1, 7), createPlainField (-1, 7,neutralPBH));
		hexTiles.Add (new HexKey (-1, 6), createPlainField (-1, 6,neutralPBH));
		hexTiles.Add (new HexKey (-1, 5), createPlainField (-1, 5,neutralPBH));
		hexTiles.Add (new HexKey (-1, 4), createPlainField (-1, 4,neutralPBH));
		hexTiles.Add (new HexKey (-1, 3), createPlainField (-1, 3,neutralPBH));
		hexTiles.Add (new HexKey (-1, 2), createPlainField (-1, 2,neutralPBH));
		hexTiles.Add (new HexKey (-1, 1),  createBunkerField(-1, 1,humanPBH));
		hexTiles.Add (new HexKey (-1, 0), createPlainField (-1, 0,neutralPBH));

		//q0
		hexTiles.Add (new HexKey (0, 7), createPlainField (0, 7,neutralPBH));
		hexTiles.Add (new HexKey (0, 6), createPlainField (0, 6,neutralPBH));
		hexTiles.Add (new HexKey (0, 5), createPlainField (0, 5,neutralPBH));
		hexTiles.Add (new HexKey (0, 4), createPlainField (0, 4,neutralPBH));
		hexTiles.Add (new HexKey (0, 3), createPlainField (0, 3,neutralPBH));
		hexTiles.Add (new HexKey (0, 2), createPlainField (0, 2,neutralPBH));
		hexTiles.Add (new HexKey (0, 1),  createBunkerField(0, 1,humanPBH));
		hexTiles.Add (new HexKey (0, 0), createMainBaseField (0, 0,humanPBH));
		hexTiles.Add (new HexKey (0, -1), createPlainField (0, -1,neutralPBH));

		//q1
		hexTiles.Add (new HexKey (1, 6), createPlainField (1, 6,neutralPBH));
		hexTiles.Add (new HexKey (1, 5), createPlainField (1, 5,neutralPBH));
		hexTiles.Add (new HexKey (1, 4), createPlainField (1, 4,neutralPBH));
		hexTiles.Add (new HexKey (1, 3), createPlainField (1, 3,neutralPBH));
		hexTiles.Add (new HexKey (1, 2), createPlainField (1, 2,neutralPBH));
		hexTiles.Add (new HexKey (1, 1), createPlainField(1, 1,neutralPBH));
		hexTiles.Add (new HexKey (1, 0), createBunkerField (1, 0,humanPBH));
		hexTiles.Add (new HexKey (1, -1), createPlainField (1, -1,neutralPBH));

		//q2

		hexTiles.Add (new HexKey (2, 6), createConstructionField (2, 6,aiPBH));
		hexTiles.Add (new HexKey (2, 5), createPlainField (2, 5,neutralPBH));
		hexTiles.Add (new HexKey (2, 4), createPlainField (2, 4,neutralPBH));
		hexTiles.Add (new HexKey (2, 3), createPlainField (2, 3,neutralPBH));
		hexTiles.Add (new HexKey (2, 2), createPlainField (2, 2,neutralPBH));
		hexTiles.Add (new HexKey (2, 1), createMineField(2, 1,neutralPBH));
		hexTiles.Add (new HexKey (2, 0), createPlainField (2, 0,neutralPBH));
		hexTiles.Add (new HexKey (2, -1), createPlainField (2, -1,neutralPBH));
		hexTiles.Add (new HexKey (2, -2), createPlainField (2, -2,neutralPBH));

		//q3
		hexTiles.Add (new HexKey (3, 5), createMineField (3, 5,aiPBH));
		hexTiles.Add (new HexKey (3, 4), createPlainField (3, 4,neutralPBH));
		hexTiles.Add (new HexKey (3, 3), createPlainField (3, 3,neutralPBH));
		hexTiles.Add (new HexKey (3, 2), createPlainField (3, 2,neutralPBH));
		hexTiles.Add (new HexKey (3, 1), createPlainField(3, 1,neutralPBH));
		hexTiles.Add (new HexKey (3, 0), createPlainField (3, 0,neutralPBH));
		hexTiles.Add (new HexKey (3, -1), createPlainField (3, -1,neutralPBH));
		hexTiles.Add (new HexKey (3, -2), createPlainField (3, -2,neutralPBH));

		//q4
		hexTiles.Add (new HexKey (4, 5), createMainBaseField (4, 5,aiPBH));
		hexTiles.Add (new HexKey (4, 4), createPlainField (4, 4,neutralPBH));
		hexTiles.Add (new HexKey (4, 3), createPlainField (4, 3,neutralPBH));
		hexTiles.Add (new HexKey (4, 2), createPlainField (4, 2,neutralPBH));
		hexTiles.Add (new HexKey (4, 1), createPlainField(4, 1,neutralPBH));
		hexTiles.Add (new HexKey (4, 0), createPlainField (4, 0,neutralPBH));
		hexTiles.Add (new HexKey (4, -1), createPlainField (4, -1,neutralPBH));
		hexTiles.Add (new HexKey (4, -2), createPlainField (4, -2,neutralPBH));
		hexTiles.Add (new HexKey (4, -3), createPlainField (4, -3,neutralPBH));
    }

	private GameObject shiftTileTo(GameObject source, GameObject target )
	{
		target.transform.parent = source.transform.parent;

		Selectable sourceSelBehav =   source.GetComponent<Selectable> () ;
		Selectable targetSelBehav =   target.GetComponent<Selectable> () ;
		targetSelBehav.positionQ = sourceSelBehav.positionQ;
		targetSelBehav.positionR = sourceSelBehav.positionR;
		targetSelBehav.normalColor = sourceSelBehav.normalColor;
		MainBehaviour.setTeam(targetSelBehav,sourceSelBehav.team);

		HexTileBehaviour sourceHexTileBehav = source.GetComponent<HexTileBehaviour> ();
		HexTileBehaviour targetHexTileBehav = target.GetComponent<HexTileBehaviour> ();
		targetHexTileBehav.containedUnit = sourceHexTileBehav.containedUnit;
		Vector3 pos = placeHexagon (targetSelBehav.positionQ, targetSelBehav.positionR,0, size,scale);
		target.transform.position = pos;
		target.transform.localScale = source.transform.localScale;

	
		HexKey key = new HexKey (targetSelBehav.positionQ, targetSelBehav.positionR);
		hexTiles[key] = targetHexTileBehav;
		Destroy (source);
		return target;
		}

	void Start () {
		mainGo = GameObject.Find ("Main");

		generateMap1 ();

		//Create Player Units
		//TODO: move to mapformat

		PlayerBehaviour humanBehav = GameObject.Find ("Human").GetComponent<PlayerBehaviour> ();
		List<Selectable> units =   humanBehav.units;
		UnitBehaviour unitType = GameObject.Find("Human").GetComponent<PlayerBehaviour>().unitType;

		for (int i = 0; i < 4; i++) {

			Selectable playerObject = ((GameObject)Instantiate (unitType.gameObject)).GetComponent<Selectable>();
            playerObject.orderCollection.Add(OrderStatic.Attack());
            playerObject.orderCollection.Add(OrderStatic.Move());
			playerObject.orderCollection.Add(OrderStatic.Capture());
			playerObject.orderCollection.Add(OrderStatic.Grenade());
            playerObject.orderCollection.Add(OrderStatic.HealthKit());
            playerObject.orderCollection.Add(OrderStatic.Silencer());
            
			playerObject.transform.parent = this.transform;
			playerObject.transform.localScale = scaleHexagon();

			units.Add (playerObject);
				}


		units[0].name = "player1";
		Selectable selec = units[0].GetComponent<Selectable>();

		HexKey key = new HexKey (0,0);
		MainBehaviour.positionUnit (selec,(HexTileBehaviour)hexTiles [key]);        

		selec.deSelectMarker();
		 
		MainBehaviour.setTeam(selec,GameObject.Find("Human").GetComponent<PlayerBehaviour>());

		UnitBehaviour unitBehav = units [0].GetComponent<UnitBehaviour> ();

		//unitBehav.updateInfo ();
	
		units[1].name = "player2";
		selec = units[1].GetComponent<Selectable>();
		key = new HexKey (0,1);
		MainBehaviour.positionUnit (selec,(HexTileBehaviour)hexTiles [key]);
        
		selec.deSelectMarker();
		MainBehaviour.setTeam(selec,GameObject.Find("Human").GetComponent<PlayerBehaviour>());
		unitBehav = units [1].GetComponent<UnitBehaviour> ();

	
		//unitBehav.updateInfo ();

		units[2].name = "player3";
		selec = units[2].GetComponent<Selectable>();
		key = new HexKey (-1,1);
		MainBehaviour.positionUnit (selec,(HexTileBehaviour)hexTiles [key]);
       

		selec.deSelectMarker();
		MainBehaviour.setTeam(selec,GameObject.Find("Human").GetComponent<PlayerBehaviour>());

		unitBehav = units [2].GetComponent<UnitBehaviour> ();

		units[3].name = "player4";
		selec = units[3].GetComponent<Selectable>();

		key = new HexKey (1,0);
		MainBehaviour.positionUnit (selec,(HexTileBehaviour)hexTiles [key]);

		selec.deSelectMarker();
		MainBehaviour.setTeam(selec,GameObject.Find("Human").GetComponent<PlayerBehaviour>());
		unitBehav = units [3].GetComponent<UnitBehaviour> ();

		GameObject.Find ("Main").GetComponent<MainBehaviour> ().StartByCall();
	}



	public static Vector3 placeHexagon( float q, float r, int layer, float size, int aScale)
	{
		return new Vector3 (size * 3f / 2f * q* aScale,layer  , size * Mathf.Sqrt(3f) * (r+q/2f) * aScale);
	}


	public static Vector3 placeHexagon( float q, float r, int layer)
	{
		return placeHexagon (q, r, layer, size, GameFieldBehaviour.scale);		
	}

	public static Vector3 scaleHexagon()
	{
		return new Vector3 (Mathf.Sqrt (3f) / 2f * size * 1f, 1f, Mathf.Sqrt (3f) / 2f * size * 1f)* scale ;		
	}


	// Update is called once per frame
	void Update () {	
	}




	public static class DrawBuddy
	{
		public static Vector3 CurrentPosition = Vector3.zero;

		public static void MoveTo(Vector3 targetVector)
		{
			CurrentPosition = targetVector;
		}

		public static void LineTo(Vector3 targetVector)
		{
			Debug.DrawLine (CurrentPosition, targetVector,Color.white,999999999999);
			MoveTo (targetVector);
		}

		public static void LineTo(Vector3 targetVector, Color lineColor)
		{
			Debug.DrawLine (CurrentPosition, targetVector,lineColor,999999999999);
			MoveTo (targetVector);
		}
	}

//	// A Star search helper
//	public class HexNode {
//		public HexNode parent;
//		public int q, r;
//		public float f, g, h;
//
//		public override string ToString ()
//		{
//			return String.Format("==q = {0}; r = {1}==",q,r);
//		}
//	}

	private static int[][] makeNeighbours()
	{	
		 int[][] neighbours = new int[6][];
		neighbours [0] = new int[2];
		neighbours [0] [0] = 1;
		neighbours [0] [1] = 0;
		neighbours [1] = new int[2];
		neighbours [1] [0] = 1;
		neighbours [1] [1] = -1;
		neighbours [2] = new int[2];
		neighbours [2] [0] = 0;
		neighbours [2] [1] = -1;
		neighbours [3] = new int[2];
		neighbours [3] [0] = -1;
		neighbours [3] [1] = 0;
		neighbours [4] = new int[2];
		neighbours [4] [0] = -1;
		neighbours [4] [1] = 1;
		neighbours [5] = new int[2];
		neighbours [5] [0] = 0;
		neighbours [5] [1] = 1;	

		return neighbours;
	}

	public static class NavigationBuddy
	{
		private static int[][] _neighbours;

	 public static int[][] neighbours {
			get{ return _neighbours ?? (
					_neighbours = makeNeighbours()
					);
						}
		}

		public static float hexDistance( float q1, float r1, float q2, float r2)
		{
			return (Mathf.Abs (q1 - q2) + Mathf.Abs (r1 - r2) + Mathf.Abs (q1 + r1 - q2 - r2)) / 2f;
		}

		public static int[][] getNeighbours(int q, int r)
		{
			int [][] res = new int[6][];

			for (int i = 0; i < 6; i++) {
				res[i] = new int[2];
								res [i] [0] = neighbours [i] [0] + q;
								res [i] [1] = neighbours [i] [1] + r;	
						}
			return res;
				}

        public static IEnumerable<int> getNeighboursEnumerable(int q, int r)
        {
            int[][] res = new int[6][];

            for (int i = 0; i < 6; i++)
            {
                res[i] = new int[2];
                res[i][0] = neighbours[i][0] + q;
                res[i][1] = neighbours[i][1] + r;
            }
            var ret =  from e in res.Cast<int>() select e;
            return ret;
//            return res.ToList<int>();
        }

		public static bool checkIfNeighbours(int q1, int r1, int q2, int r2)
		{
			//int [][] res = new int[6][];
			
			for (int i = 0; i < 6; i++) {
			
				if (
				neighbours [i] [0] + q1 == q2 &&
				neighbours [i] [1] + r1 ==r2)
				
				{return true; }				
			}
			return false;
		}
		
	}
}