using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using Assets.Scripts;

public class MainBehaviour : MonoBehaviour
{
    // Read only
    public static readonly int maxNoiseStrength = 30;
	public  readonly float fastAnimationSpeed = 3;
	public  readonly float normalAnimationSpeed = 1;
	public  readonly float slowAnimationSpeed = 0.333F;

    // Fields
    public int round = 1;
    public int score = 0;
    private Vector2 scrollVector1 = Vector2.zero;
    private Vector2 scrollVector2 = Vector2.zero;
	public bool quickConfirm = true;
	bool affirmNextRound;
	public bool insideControl;
	public PlayerBehaviour watchingPlayer;

	public WatchSelectedBehaviour guiObjectCam1;
	public WatchSelectedBehaviour guiObjectCam2;

	public AudioClip menuButton;
    private Selectable _clickedObject;
    public bool doubleClick;
    public bool blockSelection;
    public List<PlayerBehaviour> players;
    public int currentPlayerIndex;
	public List<Group> teams = new List<Group>();
	public Group winnerTeam;
    public List<HexTileBehaviour> hexTileTypes;
    public bool gameOver;
    private Order activeOrder;

    public bool isAnimating;
    public DateTime animationStart;

	public TimeSpan animationDuration;
	public TimeSpan normalDuration;
	public Action animationFinalAction;
    public GameObject animationTargetGO;
    //public Vector3 animationStartLocation;
    //public Vector3 animationEndLocation;
	public AnimationInstruction[] animationInstructions;
	public int activeAnimationInstructionIndex;
    public bool delayedMonsterSpawning;
	public float activeAnimationSpeed;
	public GUISkin customSkin;
	float animProgress = 0;
	bool oldEscPressed = false;
	bool oldReturnPressed = false;

	private GUIStyle passiveRunningStyleOn;

	public GUIStyle PassiveRunningStyleOn {
		get {
			if (passiveRunningStyleOn == null) 
			{
				passiveRunningStyleOn = new GUIStyle(customSkin.button);
				passiveRunningStyleOn.normal.textColor = Color.red;
			}
			return passiveRunningStyleOn;
		}
		set {
			passiveRunningStyleOn = value;
		}
	}

	private GUIStyle passiveRunningStyleOff;

	public GUIStyle PassiveRunningStyleOff {
		get {
			if (passiveRunningStyleOff == null) 
			{

				passiveRunningStyleOff = new GUIStyle(customSkin.button);	
			}
			return passiveRunningStyleOff;
		}
		set {
			passiveRunningStyleOff = value;
		}
	}

    // Properties
    public Selectable clickedSelectable
    {
        get { return _clickedObject; }
        set
        {
            if (_clickedObject == value)
            {
                doubleClick = true;
            }
            else
            {
                _clickedObject = value;
            }
        }
    }

    public Selectable _selectedGOSBH;

    public Selectable selectedGOSBH
    {
        get { return _selectedGOSBH; }
        set
        {
            if (_selectedGOSBH != null)
            {
               // _selectedGOSBH.deSelectMarker();
				deSelectUnit(_selectedGOSBH);
            }
            _selectedGOSBH = value;
            if (_selectedGOSBH != null)
            {
                //_selectedGOSBH.selectMarker();
				selectUnit(_selectedGOSBH);

            }
        }
    }

    public Selectable _orderTarget;

    public Selectable orderTarget
    {
        get { return _orderTarget; }
        set
        {
            if (_orderTarget != null)
            {
                //_orderTarget.deSelectMarker();
				deSelectUnit(_orderTarget);
            }
            _orderTarget = value;
            if (_orderTarget != null)
            {
                //_orderTarget.selectMarker();
				selectUnit(_orderTarget);
            }
        }
    }

	public bool touchOptions;
	public Rect rightUnitTileInfoRect;
	
	public bool toggleTest;
	public string someText = "start";
	public Vector2 scrollTest = Vector2.zero;
	public Rect topScoreRect;
	public Rect bottomScrollRect;
	//Vector2 scrollVector = Vector2.zero;
	float scrollFloat;
	//GameObject model1;
	//GameObject model2;

	public int marginDistance;


	public void ScaleToScreen()
	{
		marginDistance = 10;
		
		//y position is last
		topScoreRect = new Rect (5,0, Screen.width - 10, Screen.height / 7);
		rightUnitTileInfoRect = new Rect (Screen.width - Screen.width / 5, 0, Screen.width / 5 - 5, 0);
		bottomScrollRect =  new Rect( 0, 0 , Screen.width ,Screen.height/6 );
		
//		if (!touchOptions) {
//			marginDistance /=2;
//			topScoreRect.height /= 2;
//			bottomScrollRect.height /= 2;		
//		}

		//rightUnitTileInfoRect.y = topScoreRect.height ;
		rightUnitTileInfoRect.y = 0 ;
		rightUnitTileInfoRect.height = Screen.height - topScoreRect.height - bottomScrollRect.height ;
		bottomScrollRect.y = rightUnitTileInfoRect.height + topScoreRect.height ;	
		
		
	}

	private void aiActions()
	{		
		// Final actions before the ai finishes
		if (delayedMonsterSpawning && animationFinalAction == null)
		{
			delayedMonsterSpawning = false;
			foreach (Selectable inTileSBH in players[currentPlayerIndex].land) {
				HexTileBehaviour inTileHTB = inTileSBH.GetComponent<HexTileBehaviour>();
				if (inTileHTB.hexTileDefintion ==  HextTileDefinitions.factory) {
					if (players[currentPlayerIndex].graphene >= players[currentPlayerIndex].unitType.resourcePrice) {
						createUnit(players[currentPlayerIndex],inTileHTB);
						players[currentPlayerIndex].graphene -= players[currentPlayerIndex].unitType.resourcePrice;
					}
					else {
						break;
					}
				}
			}

			foreach (Selectable inUnitSBH in players[currentPlayerIndex].units) {
				inUnitSBH.GetComponent<AiUnitBehaviour>().unableToActCount = 0;
			}
			nextPlayer();
			return;
		}

		//ai actions depending on available orders;

		if (sortedEnemyList != null && sortedEnemyList.Count() > 0)
		{
			Selectable aiUnitToActSB = sortedEnemyList.LastOrDefault();
			sortedEnemyList.Remove(aiUnitToActSB);
			
			
			if (
				(!aiUnitToActSB.availableOrders.SameFlags(OrderLevel.B | OrderLevel.C | OrderLevel.D | OrderLevel.E | OrderLevel.F))
				)
			{
				singleMonsterCapture(aiUnitToActSB);
			}
			
			
			// check if the current ai can act.
			if ((aiUnitToActSB.availableOrders & OrderLevel.B) != OrderLevel.B)
			{
				singleMonsterMoving(aiUnitToActSB);
			}
			
			if (!aiUnitToActSB.availableOrders.SameFlags(OrderLevel.C | OrderLevel.D | OrderLevel.E | OrderLevel.F))
			{
				singleMonsterAttacking(aiUnitToActSB);
			}
			
			if (
				!aiUnitToActSB.availableOrders.SameFlags(OrderLevel.B)
			    ||
			    !aiUnitToActSB.availableOrders.SameFlags(OrderLevel.C | OrderLevel.D | OrderLevel.E | OrderLevel.F)
				)
			{
				AiUnitBehaviour aiUnitToActAUB = aiUnitToActSB.GetComponent<AiUnitBehaviour>();
				aiUnitToActAUB.unableToActCount +=1;

				if (aiUnitToActAUB.unableToActCount < aiUnitToActAUB.unableToActCountMax) {
					sortedEnemyList.Insert(0,aiUnitToActSB);
				}
			}
		}
		 
		if (sortedEnemyList.Count <= 0)
		{
			delayedMonsterSpawning = true;
		}
		}

	public static void setTeam(Selectable selectableToSwitchSB, PlayerBehaviour newTeam)
	{
		selectableToSwitchSB.team = newTeam;
		Transform modelChild = selectableToSwitchSB.transform.FindChild ("Model");
		modelChild.renderer.material.color = newTeam.teamColor;

		Renderer[] childTransormRendererCollection = modelChild.GetComponentsInChildren<Renderer>(true);
		
		foreach (Renderer inChildRenderer in childTransormRendererCollection) {
						inChildRenderer.material.color = newTeam.teamColor;	
				}
	}

	private void animateTarget( GameObject targetGO)
    {
		if (activeAnimationInstructionIndex < animationInstructions.Count()) {
		//Check what kind of animation the instruction is.
			if ((animationInstructions[activeAnimationInstructionIndex].instructionDef & InstructionDef.Position) == InstructionDef.Position) {
				targetGO.transform.position = Vector3.Lerp (animationInstructions[activeAnimationInstructionIndex].startPosition, animationInstructions[activeAnimationInstructionIndex].endPosition, animProgress);	
			}
			if ((animationInstructions[activeAnimationInstructionIndex].instructionDef & InstructionDef.Rotation) == InstructionDef.Rotation) {
				targetGO.transform.FindChild("Model").transform.eulerAngles = Vector3.Lerp (animationInstructions[activeAnimationInstructionIndex].startRotation, animationInstructions[activeAnimationInstructionIndex].endRotation, animProgress);
			}

			if ((animationInstructions[activeAnimationInstructionIndex].instructionDef & InstructionDef.Scaling) == InstructionDef.Scaling) {				
				targetGO.transform.localScale = Vector3.Lerp (animationInstructions[activeAnimationInstructionIndex].startScale, animationInstructions[activeAnimationInstructionIndex].endScale, animProgress);
			}

			if (animProgress > 1) {
				animProgress = 0;
				activeAnimationInstructionIndex += 1;
			}
		}

		if (activeAnimationInstructionIndex < animationInstructions.Count()) {
			normalDuration = animationInstructions[activeAnimationInstructionIndex].duration;
			animationDuration = TimeSpan.FromMilliseconds(normalDuration.TotalMilliseconds * (1/ activeAnimationSpeed));								
			animationStart = DateTime.Now;	
		}
		else {
			isAnimating = false;
				}
	}

    public void initiateAnimation(Action animationFinal, GameObject targetGO, AnimationInstruction[] instructions)
    {
		isAnimating = true;
        animationFinalAction = animationFinal;
		activeAnimationInstructionIndex = 0;
		animationInstructions = instructions;
		normalDuration = animationInstructions[activeAnimationInstructionIndex].duration;
		animationDuration = TimeSpan.FromMilliseconds(normalDuration.TotalMilliseconds * (1/ activeAnimationSpeed));
		animationStart = DateTime.Now;
        animationTargetGO = targetGO;        
    }

    // Use this for initialization
    public void StartByCall()
    {
		Time.timeScale = 1;
        Camera.main.enabled = true;
		activeAnimationSpeed = normalAnimationSpeed;

		//Add Players to teams
		Group men = new Group ("Men");
		men.players.Add (GameObject.Find ("Human").GetComponent<PlayerBehaviour> ());
		teams.Add( men);

//		Group zero = new Group ("Zero");
//		zero.players.Add (GameObject.Find ("Neutral").GetComponent<PlayerBehaviour> ());
//		teams.Add( zero);

		Group malice = new Group ("Malice");
		malice.players.Add (GameObject.Find ("Ai").GetComponent<PlayerBehaviour> ());
		teams.Add( malice);

		//preparing gui
		touchOptions = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WP8Player || Application.platform == RuntimePlatform.MetroPlayerX86 || Application.platform == RuntimePlatform.MetroPlayerX64 || Application.platform == RuntimePlatform.MetroPlayerARM);		
		ScaleToScreen ();

		nextPlayerSetup ();
		roundSetup ();
	}

	void LateUpdate()
	{

		//Tab through available units
		if (Input.GetButtonDown("TabThroughUnits")) {
			selectedGOSBH = lookForAvailableUnits();
				}



		//Showing pause menu
		bool newEscPressed = Input.GetKey (KeyCode.Escape);
		bool newReturnPressed = Input.GetKey (KeyCode.Return);
		if(newEscPressed && !oldEscPressed){
			if(Time.timeScale == 0){
				Time.timeScale = 1;
			}
			else {
				Time.timeScale = 0;
			}
		}

		// start next round by keypress
		if ((players [currentPlayerIndex].name == "Human")) {
			if (newReturnPressed && !oldReturnPressed) {
				//nextPlayer ();
				affirmNextRound = true;
				audio.PlayOneShot (menuButton, 0.3F);	
			}
		}

		oldReturnPressed = newReturnPressed;
		oldEscPressed = newEscPressed;


		//TODO: Needs wayyyy more testing, and code calling improvements
		Touch touchRes = Input.touches.FirstOrDefault (t => t.phase == TouchPhase.Moved);
		
		if (IsTouchInsideList (touchRes.position, bottomScrollRect)) {			
			scrollFloat = Mathf.Clamp(scrollFloat  - touchRes.deltaPosition.x *3 ,0,Screen.width);
			insideControl = true;
		} else {
			insideControl = false;
			//Testing
		}


	}

    // Update is called once per frame
    void Update()
    {
		//sets the nextround logic bool for ongui
		//affirmNextRound = setAffirmNextRoundInUpdate;

		if (Time.timeScale == 0) {
			return;
				}

        if (isAnimating)
        {
            animProgress += (float)(1 / animationDuration.TotalSeconds) * Time.deltaTime;

            animateTarget(animationTargetGO);

            //isAnimating = (animationStart + animationDuration > DateTime.Now);

            if (!isAnimating)
            {
				selectUnit(selectedGOSBH);
                animationFinalAction();
                animationFinalAction = null;
                animProgress = 0;
				activeAnimationInstructionIndex = 0;
            }
            return;
        }

		//Check if a group has lost

		var losingTeam = from lTeam in this.teams
			where lTeam.players.All (p => p.lost)
				select lTeam;

		//TODO: may unnecessarily hit perfomance
		if (losingTeam != null && losingTeam.Count() > 0) {

						foreach (var inLosingTeam in losingTeam) {

								inLosingTeam.teamLost = true;
						}
		}

		if (teams.Count(lastP => !lastP.teamLost) <= 1) {
			winnerTeam = teams.First(lastP => !lastP.teamLost); 
			gameOver = true;
			return;
		}

		// check if current player lost.
		if (players[currentPlayerIndex].lost) {
			nextPlayer();
			return;
				}

        // Ai code
        if (players[currentPlayerIndex].name == "Ai")
        {
			aiActions();
        }

		if (activeOrder != null)
        {
            switch (activeOrder.Targeting)
            {
                case TargetType.Global:
                case TargetType.Self:
                    if (activeOrder.ValidTarget(selectedGOSBH, selectedGOSBH))
                    {
                        orderTarget = selectedGOSBH;
                    }
                    break;
                //			case TargetType.Target:
                //			case TargetType.TargetFriendly:
                //				//The player selected the target
                //				if (activeOrder.ValidTarget(selectedUnit, clickedObject.gameObject))
                //				{
                //					orderTarget = clickedSelectable;
                //				}                            
                //				break;
                default:
                    break;
            }
        }

        if (clickedSelectable != null) {

			Selectable clickedSelectableUpdateLocked = clickedSelectable;

			if (selectedGOSBH != null &&  selectedGOSBH.GetComponent<UnitBehaviour>() != null && selectedGOSBH.team == players [currentPlayerIndex]) {

				UnitBehaviour selectedGOUB = selectedGOSBH.GetComponent<UnitBehaviour>();
								//begin with action targeting.

								if (activeOrder != null) {
										switch (activeOrder.Targeting) {
										case TargetType.Target:
										case TargetType.TargetFriendly:
                            //The player selected the target
						if (activeOrder.ValidTarget (selectedGOSBH, clickedSelectableUpdateLocked)) {
							orderTarget = clickedSelectableUpdateLocked;
												}
						else {
							activeOrder = null;
							selectedGOSBH = clickedSelectableUpdateLocked;

						}
												break;
										case TargetType.Self:
										case TargetType.Global:
												break;
										default:
												throw new Exception ();
										}
								}
								if (selectedGOSBH.GetComponent<UnitBehaviour> () != null) {
										//the unit default actions like move and attack;

					if (doubleClick && orderTarget == clickedSelectableUpdateLocked) {
												activeOrder.ActiveAction (selectedGOSBH, orderTarget);
						audio.PlayOneShot(menuButton,0.3F);
												resetSelection ();
										} else {
												if (activeOrder == null) {
							HexTileBehaviour clickedSelectableHTB = clickedSelectableUpdateLocked.GetComponent<HexTileBehaviour> ();
														if (clickedSelectableHTB != null) {
																activeOrder = selectedGOSBH.orderCollection.FirstOrDefault ((Order su) => su.Title == "Move");

								if (!activeOrder.Usable (selectedGOSBH, clickedSelectableUpdateLocked) || !activeOrder.ValidTarget (selectedGOSBH, clickedSelectable)) 
								{
																		activeOrder = null;
																		orderTarget = null;
																		clickedSelectable = null;
									selectedGOSBH = clickedSelectableUpdateLocked;
																}
							
														}

							UnitBehaviour clickedSelectableUB = clickedSelectableUpdateLocked.GetComponent<UnitBehaviour> ();
														if (clickedSelectableUB != null) {
																activeOrder = selectedGOSBH.orderCollection.FirstOrDefault ((Order su) => su.Title == "Attack");

								if (!activeOrder.Usable (selectedGOSBH, clickedSelectableUpdateLocked) || !activeOrder.ValidTarget (selectedGOSBH, clickedSelectableUpdateLocked)) 
								{
																		activeOrder = null;
																		orderTarget = null;
																		clickedSelectable = null;
									selectedGOSBH = clickedSelectableUpdateLocked;
																}
														}
												}
										}
								}
			
						} else {
				selectedGOSBH = clickedSelectableUpdateLocked;
								clickedSelectable = null;
						}

			//to fast switch between the unit and tile

			if (selectedGOSBH != null) {
				UnitBehaviour selUBH = selectedGOSBH.GetComponent<UnitBehaviour>();
				if (selUBH != null && selUBH.hexPosition.GetComponent<Selectable>() == clickedSelectableUpdateLocked )							
				{
					selectedGOSBH = clickedSelectableUpdateLocked;
				}	
			}
        }

        if (Input.GetMouseButtonDown(1))
        {
            resetSelection();
        }

	

//		//TODO: Needs wayyyy more testing.
//		Touch touchRes = Input.touches.FirstOrDefault (t => t.phase == TouchPhase.Moved);
//		
//		if (IsTouchInsideList (touchRes.position, bottomScrollRect)) {			
//			scrollFloat -= touchRes.deltaPosition.x;					
//			wasDragging = true;
//		} else {
//			wasDragging = false;
//			//Testing
//			GameObject.Find ("WorldCam").GetComponent<WorldCamera>().
//		}
    }


	
	bool IsTouchInsideList(Vector2 touchPos, Rect targetBounds)
	{
		Vector2 screenPos = new Vector2(touchPos.x, Screen.height - touchPos.y);  // invert y coordinate
		return targetBounds.Contains(screenPos);
	}

    public static int damageCalculation(UnitBehaviour attackerUnit, UnitBehaviour targetUnit)
    {
        return targetUnit.energy - (attackerUnit.currentAttackStrength) / (int)targetUnit.hexPosition.cover;
    }

    private void nextPlayer()
    {
		foreach (Selectable inUnit in players[currentPlayerIndex].units)
		{            
			//inUnit.GetComponent<UnitBehaviour>().hexPosition.noise = inUnit.currentLoudness;
			inUnit.availableOrders = OrderLevel.None;
		}

        currentPlayerIndex += 1;


        if (currentPlayerIndex == players.Count)
        {
            newRound();
            currentPlayerIndex = 0;
        }

		nextPlayerSetup ();

        if (players[currentPlayerIndex].name == "Ai")
        {
            aiMoves(players[currentPlayerIndex]);
        }
    }

	private void nextPlayerSetup()
	{
		foreach (Selectable inUnit in players[currentPlayerIndex].units)
		{
			//Iterating through each units passives
			foreach (Order inOrder in inUnit.orderCollection)
			{
				//reset loudness from earlier round
				inUnit.currentLoudness = inUnit.baseLoudness;
				inOrder.PassiveAction(inUnit);
			}
		}
		
		foreach (Selectable inLand in players[currentPlayerIndex].land)
		{
			//Iterating through each units passives
			foreach (Order inOrder in inLand.orderCollection)
			{
				inOrder.PassiveAction(inLand);
			}
		}
	}

	private void roundSetup()
	{
		foreach (var inHextileEntry in GameFieldBehaviour.hexTiles)
		{
			if (inHextileEntry.Value != null)
			{
				HexTileBehaviour tileHTB = ((HexTileBehaviour)inHextileEntry.Value);
				
				Selectable resetSelectable = tileHTB.gameObject.GetComponent<Selectable>();
				
				resetSelectable.GetComponent<HexTileBehaviour>().noise = resetSelectable.currentLoudness;
				
				if (tileHTB.containedUnit != null)
				{
					resetSelectable.GetComponent<HexTileBehaviour>().noise += tileHTB.containedUnit.currentLoudness;
				}
				
				// Collecting Silicon
				if (tileHTB.hexTileDefintion == HextTileDefinitions.mine) {
					resetSelectable.team.graphene +=tileHTB.GetComponent<MineBehaviour >().amountPerRound;
				}
			}
		}
	}

    //TODO: Clean up
    Selectable noisiestSB;
    List<Selectable> sortedEnemyList;

    private void aiMoves(PlayerBehaviour theAiGo)
    {
        noisiestSB = GetTheNoisiestTile().GetComponent<Selectable>();
        sortedEnemyList = new List<Selectable>(players[currentPlayerIndex].units.OrderByDescending((Selectable sel) =>
                                                                       {
                                                                           return GameFieldBehaviour.NavigationBuddy.hexDistance(sel.positionQ, sel.positionR, noisiestSB.positionQ, noisiestSB.positionR);
                                                                       }));
    }

    private bool enemyNeighbourExists(int[][] neighbours, Selectable source)
    {
        //try
        {
            HexKey key;

            foreach (var inNeighbour in neighbours)
            {
                key = new HexKey(inNeighbour[0], inNeighbour[1]);
                if (
                    GameFieldBehaviour.GetMapCost(key) >= 0 &&
                    GameFieldBehaviour.hexTiles[key] != null
                 )
                {
                    HexTileBehaviour targetTile = GameFieldBehaviour.hexTiles[key] as HexTileBehaviour;
                    if (targetTile.GetComponent<HexTileBehaviour>().containedUnit != null &&
                        targetTile.GetComponent<HexTileBehaviour>().containedUnit.GetComponent<Selectable>().team != source.team)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //		catch (Exception ex)
        //		{
        //			Debug.LogWarning(ex);
        //			throw;
        //		}
    }

    private HexTileBehaviour GetTheNoisiestTile()
    {
        HexTileBehaviour loudestHB = null;

        //foreach (DictionaryEntry inHextileEntry in GameFieldBehaviour.hexTiles)
		foreach (var inHextileEntry in GameFieldBehaviour.hexTiles)
        {
            if (loudestHB == null)
            {
                loudestHB = ((HexTileBehaviour)inHextileEntry.Value);
            }
            else if (((HexTileBehaviour)inHextileEntry.Value) != null && ((HexTileBehaviour)inHextileEntry.Value).noise > loudestHB.noise)
            {
                loudestHB = ((HexTileBehaviour)inHextileEntry.Value);
            }
        }
        return loudestHB;
    }

    //TODO: Write even better code to walk around other units.
    private void moveMonsters(PlayerBehaviour theAiGo)
    {
        bool hasMovableUnits;
        Selectable movingMonsterSB = null;

        while (true)
        {
            hasMovableUnits = false;
            foreach (Selectable inMonsterSB in theAiGo.GetComponent<PlayerBehaviour>().units)
            {
                //Checking if the monster moved already, which has an orderLevel of B
                if (!((inMonsterSB.availableOrders & OrderLevel.B) == OrderLevel.B))
                {
                    hasMovableUnits = true;
                    movingMonsterSB = inMonsterSB;
                    break;
                }
            }
            if (!hasMovableUnits)
            {
                break;
            }

            //check if neighbours with enemy units.
            if (enemyNeighbourExists(GameFieldBehaviour.NavigationBuddy.getNeighbours(movingMonsterSB.positionQ, movingMonsterSB.positionR), movingMonsterSB))
            {
                movingMonsterSB.availableOrders = movingMonsterSB.availableOrders | OrderLevel.B;
                continue;
            }
			//else

            UnitBehaviour startUnitBehaviour = movingMonsterSB.GetComponent<UnitBehaviour>();

            //TODO: insert noisiest check in here and assign as targets.

            Selectable GoalTargetTile = null;

            foreach (Selectable inNeighbours in movingMonsterSB.Neighbours)
            {
                if (GoalTargetTile == null)
                {
                    GoalTargetTile = inNeighbours;
                }
				else if (inNeighbours != null && inNeighbours.GetComponent<HexTileBehaviour>().noise > GoalTargetTile.GetComponent<HexTileBehaviour>().noise)
                {
                    GoalTargetTile = inNeighbours;
                }
            }

            if (GoalTargetTile == null)
            {
                movingMonsterSB.availableOrders = movingMonsterSB.availableOrders | OrderLevel.B;
                continue;
            }

			if (GoalTargetTile.GetComponent<HexTileBehaviour>().noise < movingMonsterSB.GetComponent<UnitBehaviour>().hexPosition.noise || GoalTargetTile.GetComponent<HexTileBehaviour>().noise  < 0.01)
            {
                GoalTargetTile = GetTheNoisiestTile().GetComponent<Selectable>();
            }

            var res = NavigationHelper.FindPath<Selectable>(startUnitBehaviour.hexPosition.GetComponent<Selectable>(), GoalTargetTile, 1,
                                                  (Selectable su, Selectable gu) =>
                                                  {
                                                      return GameFieldBehaviour.NavigationBuddy.hexDistance(su.positionQ, su.positionR, gu.positionQ, gu.positionR);
                                                  }
            , (Selectable u) =>
            {
                return GameFieldBehaviour.NavigationBuddy.hexDistance(u.positionQ, u.positionR, GoalTargetTile.positionQ, GoalTargetTile.positionR);
            });

            if (res == null)
            {
                movingMonsterSB.availableOrders = movingMonsterSB.availableOrders | OrderLevel.B;
                continue;
            }
            var test = res.ToArray();

            //begin Navigation					

            HexKey key = new HexKey(test[test.Length - 2].positionQ, test[test.Length - 2].positionR);
            HexTileBehaviour tile = (HexTileBehaviour)GameFieldBehaviour.hexTiles[key];

            movingMonsterSB.orderCollection.First((order) => order.Title == "Move").ActiveAction(movingMonsterSB, tile.GetComponent<Selectable>());

            movingMonsterSB.availableOrders = movingMonsterSB.availableOrders | OrderLevel.B;

        }
    }

	private void singleMonsterCapture(Selectable capturingMonsterSB)
	{
		HexTileBehaviour aiHexHTB = capturingMonsterSB.GetComponent<UnitBehaviour>().hexPosition;
		if (aiHexHTB.hexTileDefintion != HextTileDefinitions.classic &&aiHexHTB.GetComponent<Selectable>().team != capturingMonsterSB.team ) 
		{
			capturingMonsterSB.orderCollection.First((order) => order.Title == "Capture").ActiveAction(capturingMonsterSB, capturingMonsterSB);
			//capturingMonsterSB.availableOrders = capturingMonsterSB.availableOrders | OrderLevel.B  | OrderLevel.C | OrderLevel.D | OrderLevel.E | OrderLevel.F;
		}
	}

    private void singleMonsterAttacking(Selectable attackingMonsterSB)
    {
        HexKey key;

        foreach (var inNeighbour in GameFieldBehaviour.NavigationBuddy.getNeighbours(attackingMonsterSB.positionQ, attackingMonsterSB.positionR))
        {
            key = new HexKey(inNeighbour[0], inNeighbour[1]);
            if (GameFieldBehaviour.GetMapCost(key) < 0 || GameFieldBehaviour.hexTiles[key] == null)
            {
                continue;
            }

            Selectable possibleTarget = (GameFieldBehaviour.hexTiles[key] as HexTileBehaviour).containedUnit;
            if (possibleTarget != null)
            {
				Selectable possibleTargetSB = possibleTarget.GetComponent<Selectable>();
				if (possibleTargetSB.team != attackingMonsterSB.team) {
					
					attackingMonsterSB.orderCollection.First((order) => order.Title == "Attack").ActiveAction(attackingMonsterSB, possibleTargetSB);

					break;
				}
            }
        }
        //attackingMonsterSB.availableOrders = attackingMonsterSB.availableOrders | OrderLevel.C | OrderLevel.D | OrderLevel.E | OrderLevel.F;
    }

    private void singleMonsterMoving(Selectable movingMonsterSB)
    {
        if (enemyNeighbourExists(GameFieldBehaviour.NavigationBuddy.getNeighbours(movingMonsterSB.positionQ, movingMonsterSB.positionR), movingMonsterSB))
        {
            //movingMonsterSB.availableOrders = movingMonsterSB.availableOrders | OrderLevel.B;
            return;
        }

        UnitBehaviour startUnitBehaviour = movingMonsterSB.GetComponent<UnitBehaviour>();

        //TODO: insert noisiest check in here and assign as targets.

        Selectable GoalTargetTile = null;

        foreach (Selectable inNeighbours in movingMonsterSB.Neighbours)
        {

            if (GoalTargetTile == null)
            {
                GoalTargetTile = inNeighbours;
            }
			else if (inNeighbours != null && inNeighbours.GetComponent<HexTileBehaviour>().noise > GoalTargetTile.GetComponent<HexTileBehaviour>().noise)
            {
                GoalTargetTile = inNeighbours;
            }
        }

        if (GoalTargetTile == null)
        {
            //movingMonsterSB.availableOrders = movingMonsterSB.availableOrders | OrderLevel.B;
            return;
        }

		if (GoalTargetTile.GetComponent<HexTileBehaviour>().noise < 0.01)
        {
            GoalTargetTile = GetTheNoisiestTile().GetComponent<Selectable>();
        }

        var res = NavigationHelper.FindPath<Selectable>(startUnitBehaviour.hexPosition.GetComponent<Selectable>(), GoalTargetTile, 1,
                                                        (Selectable su, Selectable gu) =>
                                                        {
                                                            return GameFieldBehaviour.NavigationBuddy.hexDistance(su.positionQ, su.positionR, gu.positionQ, gu.positionR);
                                                        }
        , (Selectable u) =>
        {
            return GameFieldBehaviour.NavigationBuddy.hexDistance(u.positionQ, u.positionR, GoalTargetTile.positionQ, GoalTargetTile.positionR);
        });

        if (res == null)
        {
           // movingMonsterSB.availableOrders = movingMonsterSB.availableOrders | OrderLevel.B;
            return;
        }
        var test = res.ToArray();

        //begin Navigation					

        HexKey key = new HexKey(test[test.Length - 2].positionQ, test[test.Length - 2].positionR);
        HexTileBehaviour tile = (HexTileBehaviour)GameFieldBehaviour.hexTiles[key];
		    
		Selectable tileSB = tile.GetComponent<Selectable>();
		movingMonsterSB.orderCollection.First((order) => order.Title == "Move").ActiveAction(movingMonsterSB, tileSB);
        //movingMonsterSB.availableOrders = movingMonsterSB.availableOrders | OrderLevel.B;
    }

    public static List<double[]> MakeNoise(Selectable epicenter, int strength)
    {
        List<double[]> hexagonCollection = new List<double[]>();
        int noiseRange = strength; //* 1/ 10;        

        for (int idx = -noiseRange; idx <= noiseRange; idx += 1)
        {
            for (int idy = Math.Max(-noiseRange, -idx - noiseRange); idy <= Math.Min(noiseRange, -idx + noiseRange); idy += 1)
            {
                double dz = idx - idy;
                hexagonCollection.Add(new double[] {   epicenter.positionQ + Convert.ToInt32(idx), epicenter.positionR + Convert.ToInt32(idy),
                        noiseRange - (Math.Abs(0 - idx) + Math.Abs(0 - idy) + Math.Abs(0 + 0 - idx - idy)) / 2});
            }
        }
        return hexagonCollection;
    }

    private void monstersAttack(PlayerBehaviour theAiGo)
    {
        bool hasActableUnits;
        Selectable actingMonsterSB = null;

        while (true)
        {
            hasActableUnits = false;
            foreach (Selectable inMonsterSB in theAiGo.GetComponent<PlayerBehaviour>().units)
            {
                if (!(inMonsterSB.availableOrders.SameFlags(OrderLevel.C | OrderLevel.D | OrderLevel.E | OrderLevel.F)))
                {
                    hasActableUnits = true;
                    actingMonsterSB = inMonsterSB;
                    break;
                }
            }
            if (!hasActableUnits)
            {
                break;
            }

            HexKey key;

            foreach (var inNeighbour in GameFieldBehaviour.NavigationBuddy.getNeighbours(actingMonsterSB.positionQ, actingMonsterSB.positionR))
            {
                key = new HexKey(inNeighbour[0], inNeighbour[1]);
                if (GameFieldBehaviour.GetMapCost(key) < 0 || GameFieldBehaviour.hexTiles[key] == null)
                {
                    continue;
                }

                Selectable possibleTarget = (GameFieldBehaviour.hexTiles[key] as HexTileBehaviour).containedUnit;
                if (possibleTarget != null)
                {
					Selectable possibleTargetSB = possibleTarget.GetComponent<Selectable>();
					if (possibleTarget.GetComponent<Selectable>().team != actingMonsterSB.team) {
						actingMonsterSB.orderCollection.First((order) => order.Title == "Attack").ActiveAction(actingMonsterSB, possibleTargetSB);
						actingMonsterSB.availableOrders = actingMonsterSB.availableOrders | OrderLevel.C | OrderLevel.D | OrderLevel.E | OrderLevel.F;
						
						break;
					}
                }
            }
            actingMonsterSB.availableOrders = actingMonsterSB.availableOrders | OrderLevel.C | OrderLevel.D | OrderLevel.E | OrderLevel.F;

        }
    }

//    private void killUnit(Selectable unitToDie)
//    {
//        unitToDie.GetComponent<UnitBehaviour>().hexPosition.containedUnit = null;
//        PlayerBehaviour unitTeam = unitToDie.team.GetComponent<PlayerBehaviour>();
//        unitTeam.units.Remove(unitToDie);
//        UnityEngine.Object.Destroy(unitToDie);
//
//        if (unitTeam.units.Count <= 0)
//        {
//            unitTeam.lost();
//        }
//    }

//    private void killUnit(UnitBehaviour unitToDie)
//    {
//        killUnit(unitToDie.GetComponent<Selectable>());
//    }

    //spawning Monsters
	private void createUnit(PlayerBehaviour player,HexTileBehaviour inTileHTB)
    {       
		Selectable inHexSelectBehav = inTileHTB.GetComponent<Selectable>();
		       
                if (inTileHTB.containedUnit == null)
                {
                    GameObject unitGO = (GameObject)Instantiate(player.GetComponent<PlayerBehaviour>().unitType.gameObject);
                    unitGO.transform.parent = player.transform;
                    unitGO.transform.localScale = new Vector3(Mathf.Sqrt(3f) / 2f * GameFieldBehaviour.size * 1f, 1f, Mathf.Sqrt(3f) / 2f * GameFieldBehaviour.size * 1f);

                    unitGO.name = "unit:" + Guid.NewGuid().ToString();
                    Selectable selec = unitGO.GetComponent<Selectable>();
                    selec.orderCollection.Add(OrderStatic.Attack());
                    selec.orderCollection.Add(OrderStatic.Move());
			selec.orderCollection.Add(OrderStatic.Capture());
			selec.availableOrders = OrderLevel.A| OrderLevel.B | OrderLevel.C |OrderLevel.D| OrderLevel.E |OrderLevel.F;

				MainBehaviour.positionUnit(selec, inTileHTB);
                    unitGO.transform.position = GameFieldBehaviour.placeHexagon(selec.positionQ, selec.positionR, 1);
                    unitGO.transform.localScale = GameFieldBehaviour.scaleHexagon();
					setTeam(selec,player);
                    //unitGO.GetComponent<UnitBehaviour>().updateInfo();
                    player.GetComponent<PlayerBehaviour>().units.Add(selec);
                }
    }

	private void selectUnit(Selectable aSelec)
	{
		if (aSelec == null) {
			guiObjectCam1.setSelected (null);
			guiObjectCam2.setSelected (null);
			return;
				}
		aSelec.selectMarker();
		UnitBehaviour selectedUB = aSelec.GetComponent<UnitBehaviour> ();
		if (selectedUB != null) {
						guiObjectCam2.setSelected (aSelec.gameObject);
						guiObjectCam1.setSelected (selectedUB.hexPosition.gameObject);			
				} else {
						//it is a hextile
						guiObjectCam1.setSelected (aSelec.gameObject);
						//Does the tile have a unit on it?
						Selectable containedSB = aSelec.GetComponent<HexTileBehaviour> ().containedUnit;
						if (containedSB != null) {
								guiObjectCam2.setSelected (aSelec.GetComponent<HexTileBehaviour> ().containedUnit.gameObject);
						} else {
								guiObjectCam2.setSelected (null);
						}
				}
	}

	private void deSelectUnit(Selectable aSelec)
	{
		aSelec.deSelectMarker();
	}



    private void newRound()
    {
        round++;

		roundSetup ();

    }

	//looks for the next unit which can move.
	private Selectable lookForAvailableUnits()
	{
		return watchingPlayer.units.FirstOrDefault (u => {
						return (
				!u.availableOrders.SameFlags (OrderLevel.B)
				&&
				!u.availableOrders.SameFlags (OrderLevel.C | OrderLevel.D | OrderLevel.E | OrderLevel.F)
				);
				});		
	}
	
	
	private void resetSelection()
    {		
		guiObjectCam1.setSelected (null);
		guiObjectCam2.setSelected (null);

        if (selectedGOSBH != null)
        {
            //selectedGOSBH.deSelectMarker();
			deSelectUnit(selectedGOSBH);

        }

        if (orderTarget != null)
        {
            //orderTarget.deSelectMarker();
			deSelectUnit(orderTarget);
        }

        if (clickedSelectable != null)
        {
            //clickedSelectable.GetComponent<Selectable>().deSelectMarker();
			deSelectUnit(clickedSelectable);
        }

		activeOrder = null;
        selectedGOSBH = null;
        orderTarget = null;
        clickedSelectable = null;
        doubleClick = false;
    }

	private void beginAct()
	{activeOrder.ActiveAction(selectedGOSBH, orderTarget);
					audio.PlayOneShot(menuButton,0.3F);
					resetSelection();
	}


	//bool setAffirmNextRoundInUpdate;

	private void showGreyBlend()
	{
		guiObjectCam1.camera.enabled = false;
		guiObjectCam2.camera.enabled = false;

		GameObject.Find("CameraBlend").GetComponent<MeshRenderer>().enabled = true;
		GameObject.Find("CameraBlend").GetComponent<MeshCollider>().enabled = true;
	}

	private void hideGreyBlend()
	{
		guiObjectCam1.camera.enabled = true;
		guiObjectCam2.camera.enabled = true;

		GameObject.Find("CameraBlend").GetComponent<MeshRenderer>().enabled = false;
		GameObject.Find("CameraBlend").GetComponent<MeshCollider>().enabled = false;
	}

    void OnGUI()
	{
		GUI.skin = customSkin;

		//unnecessary if not debugging gui
		//ScaleToScreen ();

		if(Time.timeScale == 0){

			showGreyBlend();

			GUILayout.BeginArea(new Rect(Screen.width/4,0,Screen.width/2,Screen.height));
			GUILayout.Box( "PAUSE");
			GUILayout.EndArea();

			GUILayout.BeginArea(new Rect(Screen.width/4,0,Screen.width/2,Screen.height));
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();

			GUILayout.FlexibleSpace();
			//Speed Options
			GUILayout.Label( "Animation Speed:");
			GUILayout.BeginHorizontal();
			if (GUILayout.Button( "Slow"))
					{
						activeAnimationSpeed = slowAnimationSpeed;
						audio.PlayOneShot(menuButton,0.3F);
					}
			
			if (GUILayout.Button( "Normal"))
					{
						activeAnimationSpeed = normalAnimationSpeed;
						audio.PlayOneShot(menuButton,0.3F);
					}
			
			if (GUILayout.Button("Fast"))
					{
						activeAnimationSpeed = fastAnimationSpeed;
						audio.PlayOneShot(menuButton,0.3F);
					}
			GUILayout.EndHorizontal();

			GUILayout.Label( "Toggle Fast Confirm");
			//fast confirm
			quickConfirm = GUILayout.Toggle(quickConfirm, quickConfirm.ToString());			

			//quit
			if (GUILayout.Button("Return to game"))
			        {
				Time.timeScale = 1;
			        }

			if (GUILayout.Button("Quit"))
			{
				Application.LoadLevel(0);
				audio.PlayOneShot(menuButton,0.3F);
			}

			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndArea();
			return;
		} 
		else {
			hideGreyBlend();
				}
		
		if (gameOver) {
						GUIText winDisplay = GameObject.Find ("WinBillboard").GetComponent<GUIText> ();
						winDisplay.text = "TEAM \"" + winnerTeam.title + "\" WON!";
						winDisplay.enabled = true;


						if (GUI.Button (new Rect (Screen.width / 3, Screen.height / 3, Screen.width / 3, Screen.height / 3), "Game Over!" + Environment.NewLine +
								"Rounds: " + round + Environment.NewLine +
								"Final Score: " + this.watchingPlayer.score + Environment.NewLine +
								"Click to return to menu.")) {
								Application.LoadLevel (0);
						}

						return;
				}

		if (affirmNextRound) {

			Selectable resUnitSB = lookForAvailableUnits ();
			if ( resUnitSB != null) {

				showGreyBlend();
				GUILayout.BeginArea(new Rect( Screen.width * 2/8, Screen.height/4, Screen.width /2,  Screen.height/4));
				GUILayout.BeginVertical();
				//GUILayout.FlexibleSpace();
				GUILayout.Box("There are still some units that can move, already end round?", GUILayout.ExpandHeight(false));
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Yes", GUILayout.ExpandHeight(true))) {
					affirmNextRound = false;
					nextPlayer();
					audio.PlayOneShot (menuButton, 0.3F);
				}
				if (GUILayout.Button("No", GUILayout.ExpandHeight(true))) {
					affirmNextRound = false;
					selectedGOSBH =  resUnitSB;
					audio.PlayOneShot (menuButton, 0.3F);
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
				//GUILayout.FlexibleSpace();
				GUILayout.EndArea();
			}
			else {
				hideGreyBlend();
				if (Event.current.type == EventType.Repaint)
				{
					affirmNextRound = false;
					nextPlayer();
					return;
				}
			}
			return;
		}

		//Display Round, Silicon and Next Round Button
		GUILayout.BeginArea (topScoreRect);
		GUILayout.BeginHorizontal (GUILayout.ExpandHeight (true));
		GUILayout.Label ("Round: " + round);
		GUILayout.Label ("Graphene: " + watchingPlayer.graphene  );

		if (players [currentPlayerIndex].lost) {
			return;
		}

		GUI.enabled = (players [currentPlayerIndex].name == "Human");

			if (GUILayout.Button ("End Round", GUILayout.ExpandHeight (true), GUILayout.ExpandWidth (false))) {
						//nextPlayer ();
			affirmNextRound = true;
						audio.PlayOneShot (menuButton, 0.3F);
				}

		GUI.enabled = true;

		GUILayout.EndHorizontal ();
		GUILayout.EndArea ();

		if (players[currentPlayerIndex].name != "Human")
		{
			return;
		}

		if (selectedGOSBH != null) {

						UnitBehaviour selectedGOUB = selectedGOSBH.GetComponent<UnitBehaviour> ();
						HexTileBehaviour selectedGOHTB;
						if (selectedGOUB == null) {
								selectedGOHTB = selectedGOSBH.GetComponent<HexTileBehaviour> ();
								if (selectedGOHTB.containedUnit != null) {
										selectedGOUB = selectedGOHTB.containedUnit.GetComponent<UnitBehaviour> ();	
								}
						} else {
								selectedGOHTB = selectedGOUB.hexPosition;
						}

						//Display Unit and Tile information
						GUILayout.BeginArea (rightUnitTileInfoRect);
						GUILayout.BeginVertical (GUILayout.ExpandWidth (true));

						if (selectedGOUB != null) {			
								GUILayout.FlexibleSpace ();
								string unitContent = "Unit: " + selectedGOUB.name + Environment.NewLine +
										"En:" + selectedGOUB.energy + Environment.NewLine + 
										"ATK:" + selectedGOUB.currentAttackStrength;


//				if (GUILayout.Button (unitContent, GUILayout.ExpandHeight (false), GUILayout.ExpandWidth (true))) {
//					selectedGOSBH = selectedGOUB.GetComponent<Selectable>();
//					return;	
//				}

				bool unitRes = GUILayout.Toggle ((selectedGOSBH == selectedGOUB.GetComponent<Selectable>()), unitContent, GUILayout.ExpandHeight (false), GUILayout.ExpandWidth (true));

				if (unitRes && selectedGOSBH != selectedGOUB.GetComponent<Selectable>()) {
					selectedGOSBH = selectedGOUB.GetComponent<Selectable>();

				}
			}

						GUILayout.FlexibleSpace ();
						string tileContent = "Tile: " + selectedGOHTB.name + Environment.NewLine +
				"Cover:" + selectedGOHTB.cover.ToString () + Environment.NewLine +
								"Noise:" + selectedGOHTB.noise;
						

//			if (GUILayout.Button (tileContent, GUILayout.ExpandHeight (false), GUILayout.ExpandWidth (true))) {
//				selectedGOSBH = selectedGOHTB.GetComponent<Selectable>();
//				return;
//				}

			bool tileRes = GUILayout.Toggle ((selectedGOSBH == selectedGOHTB.GetComponent<Selectable>()),tileContent, GUILayout.ExpandHeight (false), GUILayout.ExpandWidth (true));
			
			if (tileRes && selectedGOSBH != selectedGOHTB.GetComponent<Selectable>()){
				selectedGOSBH = selectedGOHTB.GetComponent<Selectable>();
			}

						GUILayout.EndVertical ();
						GUILayout.EndArea ();
		
						//Display Unit Orders and Tile Constructions
		
						int buttonWidth = Screen.width / 5;
						int buttonHeight = Screen.height / 5;
		
						if (!touchOptions) {
			
								buttonWidth /= 2;
						}
		

			if (selectedGOSBH.team == watchingPlayer) {
				
			
						//TODO: touch - need to make sure it snaps on limits
						GUI.BeginGroup (new Rect (-scrollFloat, bottomScrollRect.y, bottomScrollRect.width, bottomScrollRect.height));
		
			for (int i = 0; i < selectedGOSBH.orderCollection.Count; i++) {
					

				GUI.enabled = selectedGOSBH.orderCollection[i].Usable(selectedGOSBH, null);
							
							            GUIStyle currentStyle;
							
				if (selectedGOSBH.orderCollection[i].PassiveRunning)
							            {
							                currentStyle = PassiveRunningStyleOn;
							            }
							            else
							            {
							                currentStyle = PassiveRunningStyleOff;
							            }


				if (GUI.Toggle (new Rect (i * (marginDistance + buttonWidth), marginDistance, buttonWidth, buttonHeight), (activeOrder == selectedGOSBH.orderCollection[i]) , selectedGOSBH.orderCollection[i].Title ,currentStyle)) {
					activeOrder = selectedGOSBH.orderCollection[i];					
				}
				
				}

						GUI.EndGroup ();

				//if (touchOptions) {
				//	GUI.HorizontalScrollbar (new Rect (0, Screen.height - (marginDistance * 2), Screen.width, (marginDistance * 2)), scrollFloat, buttonWidth + marginDistance, 0, selectedGOSBH.orderCollection.Count * (marginDistance + buttonWidth) - Screen.width + buttonWidth + marginDistance);
			//	}
			}

		FactoryBehaviour selectedFB = selectedGOHTB.GetComponent<FactoryBehaviour> ();

			if (selectedFB != null && selectedGOSBH == selectedFB.GetComponent<Selectable>() &&  selectedFB.GetComponent<Selectable>().team == watchingPlayer) {
				
							HexTileBehaviour selectedHTB = selectedFB.GetComponent<HexTileBehaviour>();
							GUI.enabled = (selectedHTB.containedUnit == null && selectedGOSBH.team.graphene >= selectedGOSBH.team.unitType.resourcePrice);

				GUILayout.BeginArea(bottomScrollRect);
				
				GUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Width(Screen.width / 2));
				
				// Showing all Equipment on the tile
				GUILayout.BeginScrollView(scrollVector1);
				GUILayout.Label("Produce Unit");
				
				if (GUILayout.Button( selectedGOSBH.team.unitType.name ))
				{
					createUnit(selectedGOSBH.team,selectedHTB);
					selectedGOSBH.team.graphene -= selectedGOSBH.team.unitType.resourcePrice;
					audio.PlayOneShot(menuButton,0.3F);
				}
				
				GUILayout.EndScrollView();
				GUILayout.EndHorizontal();

				GUILayout.EndArea();
				
				GUI.enabled = true;
				return;
						}


		if (activeOrder != null && orderTarget != null)
		{
			if (quickConfirm) {
				beginAct();
			}
			else {
				String buttonMessage = "Confirm?";
				UnitBehaviour targetUB = orderTarget.GetComponent<UnitBehaviour>();
				
				if (activeOrder.Targeting.SameFlags(TargetType.Target | TargetType.TargetFriendly) && targetUB != null)
				{
					buttonMessage += String.Format("\nPrognosis:\nTeam: {0},\nUnit: {1}\nenergy = {2}", orderTarget.team.name, orderTarget.name, damageCalculation(selectedGOUB, targetUB).ToString());
				}
				
				if (GUI.Button(new Rect(10, 10, 200, 100), buttonMessage))
				{
					beginAct();
				}
			}
			
			return;
		}

		return;

		}

//
//
//        if (GUI.Button(new Rect(10, Screen.height - 110, 150, 100), "Quit"))
//        {
//			Application.LoadLevel(0);
//			audio.PlayOneShot(menuButton,0.3F);
//        }
//
//		if (GUI.Button(new Rect(220 , Screen.height - 110, 100, 50), "Slow"))
//		{
//			activeAnimationSpeed = slowAnimationSpeed;
//			audio.PlayOneShot(menuButton,0.3F);
//		}
//
//		if (GUI.Button(new Rect(330 , Screen.height - 110, 100, 50), "Normal"))
//		{
//			activeAnimationSpeed = normalAnimationSpeed;
//			audio.PlayOneShot(menuButton,0.3F);
//		}
//
//		if (GUI.Button(new Rect(440 , Screen.height - 110, 100, 50), "Fast"))
//		{
//			activeAnimationSpeed = fastAnimationSpeed;
//			audio.PlayOneShot(menuButton,0.3F);
//		}
//
//        PlayerBehaviour currentPB = players[currentPlayerIndex];
//
//        if (currentPB.name != "Human")
//        {
//            return;
//        }
//
//		if (players[currentPlayerIndex].lost) {
//			return;
//		}
//
//        if (selectedGOSBH == null)
//        {
//            if (GUI.Button(new Rect(Screen.width - 160, 10, 150, 100), "End Round"))
//			{nextPlayer();
//				audio.PlayOneShot(menuButton,0.3F);                
//            }
//            return;
//        }
//     
//
//        showGuiInfo(selectedGOSBH);
//
//		//FactoryBehaviour selectedFB = selectedGOSBH.GetComponent<FactoryBehaviour> ();
//       // UnitBehaviour selectedunitUB = selectedGOSBH.GetComponent<UnitBehaviour>();
//
//		//if (selectedunitUB == null && selectedFB == null)
//		{
//            return;
//        }
//
//        if (activeOrder != null && orderTarget != null)
//        {
//			if (quickConfirm) {
//				beginAct();
//			}
//			else {
//				String buttonMessage = "Confirm?";
//				UnitBehaviour targetUB = orderTarget.GetComponent<UnitBehaviour>();
//				
//				if (activeOrder.Targeting.SameFlags(TargetType.Target | TargetType.TargetFriendly) && targetUB != null)
//				{
//					buttonMessage += String.Format("\nPrognosis:\nTeam: {0},\nUnit: {1}\nenergy = {2}", orderTarget.team.name, orderTarget.name, damageCalculation(selectedunitUB, targetUB).ToString());
//				}
//				
//				if (GUI.Button(new Rect(10, 10, 200, 100), buttonMessage))
//				{
//					beginAct();
//				}
//			}
//            
//            return;
//        }
//
//        if (selectedGOSBH.team != players[currentPlayerIndex])
//        {
//            return;
//        }
//
//		if (selectedFB != null) {
//
//			HexTileBehaviour selectedHTB = selectedFB.GetComponent<HexTileBehaviour>();
//			GUI.enabled = (selectedHTB.containedUnit == null && selectedGOSBH.team.silicon >= selectedGOSBH.team.unitType.resourcePrice);
//			GUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Width(Screen.width / 2));
//			
//			// Showing all Equipment on the tile
//			GUILayout.BeginScrollView(scrollVector1);
//			GUILayout.Label("Produce Unit");
//			
//			if (GUILayout.Button( selectedGOSBH.team.unitType.name ))
//			{
//				createUnit(selectedGOSBH.team,selectedHTB);
//				selectedGOSBH.team.silicon -= selectedGOSBH.team.unitType.resourcePrice;
//				audio.PlayOneShot(menuButton,0.3F);
//			}
//
//			GUILayout.EndScrollView();
//			GUILayout.EndHorizontal();
//
//			GUI.enabled = true;
//			return;
//		}
//		
//		GUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Width(Screen.width / 2));
//		
//		// Showing all Equipment on the tile
//		GUILayout.BeginScrollView(scrollVector1);
//		GUILayout.Label("Order on Tile");
//		Selectable hexSelectable = selectedunitUB.hexPosition.GetComponent<Selectable>();
//
//        foreach (Order inOrder in hexSelectable.orderCollection)
//        {
//            //Check if the tiles options can be used
//
//            GUI.enabled = inOrder.Usable(selectedunitUB.hexPosition.GetComponent<Selectable>(), null);
//
//            GUIStyle currentStyle;
//
//            if (inOrder.PassiveRunning)
//            {
//                currentStyle = PassiveRunningStyleOn;
//            }
//            else
//            {
//                currentStyle = PassiveRunningStyleOff;
//            }
//
//            if (GUILayout.Toggle((activeOrder == inOrder), inOrder.Title + "x " + inOrder.Uses.ToString(), currentStyle))
//            {
//				//audio.PlayOneShot(menuButton,0.3F);
//                activeOrder = inOrder;
//            }
//
//            GUI.enabled = true;
//        }
//        GUILayout.EndScrollView();
//
//        // Showing all Equipment on the unit itself
//
//        GUILayout.BeginScrollView(scrollVector2);
//        GUILayout.Label("Order on Unit");
//        foreach (Order inOrder in selectedGOSBH.orderCollection)
//        {
//
//            GUI.enabled = inOrder.Usable(selectedGOSBH, null);
//
//            GUIStyle currentStyle;
//
//            if (inOrder.PassiveRunning)
//            {
//                currentStyle = PassiveRunningStyleOn;
//            }
//            else
//            {
//                currentStyle = PassiveRunningStyleOff;
//            }
//
//            if (GUILayout.Toggle((activeOrder == inOrder), inOrder.Title + " x " + inOrder.Uses.ToString(), currentStyle))
//            {
//				//audio.PlayOneShot(menuButton,0.3F);
//                activeOrder = inOrder;
//            }
//
//            GUI.enabled = true;
//        }
//
//        //Usability is decided by the order itself, and can still be carried even if "empty"
//        GUILayout.EndScrollView();
//        GUILayout.EndHorizontal();
//        return;
//
    }

    // shows information for uncommandable selectables
    private void showGuiInfo(String message)
    {
        GUI.Label(new Rect(10, 130, 150, 100), "Info:" + message);
    }

    private void showGuiInfo(Selectable goInfoSB)
    {
        StringBuilder messageBuild = new StringBuilder("\nname: " + goInfoSB.name);
        messageBuild.Append("\nteam: " + goInfoSB.team.name);

        HexTileBehaviour hexTile = goInfoSB.GetComponent<HexTileBehaviour>();
        if (hexTile != null)
        {
            messageBuild.Append("\ncover: " + hexTile.cover.ToString());
        }
        else
        {
            UnitBehaviour unit = goInfoSB.GetComponent<UnitBehaviour>();
            if (unit != null)
            {
                messageBuild.Append("\nenergy: " + unit.energy + "/" + unit.energyLimit);
            }
        }
        showGuiInfo(messageBuild.ToString());
    }

    public static void positionUnit(Selectable currentSelected, HexTileBehaviour clickedHexField)
    {
        UnitBehaviour currentSelUnitBehav = currentSelected.GetComponent<UnitBehaviour>();
        Selectable clickedSelectable = clickedHexField.gameObject.GetComponent<Selectable>();

        if (currentSelUnitBehav.hexPosition != null)
        {
            currentSelUnitBehav.hexPosition.containedUnit = null;
        }

        currentSelUnitBehav.hexPosition = clickedHexField;
        currentSelUnitBehav.hexPosition.containedUnit = currentSelected ;

        //turning
        //rotateUnit(currentSelected, clickedSelectable);

        //moving
        currentSelected.positionQ = clickedSelectable.positionQ;
        currentSelected.positionR = clickedSelectable.positionR;

        currentSelected.transform.position = GameFieldBehaviour.placeHexagon(clickedSelectable.positionQ, clickedSelectable.positionR, 1);
    }

    public  static void rotateUnit(Selectable toRotateSelectable, Selectable targetSelectable)
    {
        toRotateSelectable.transform.FindChild("Model").transform.eulerAngles =
			new Vector3(toRotateSelectable.transform.FindChild("Model").transform.eulerAngles.x, rotateEul(toRotateSelectable, targetSelectable).y, toRotateSelectable.transform.FindChild("Model").transform.eulerAngles.z);
    }

    public static Vector3 rotateEul(Selectable currentSelectable, Selectable targetSelectable)
    {
        float rotation = 0;

        for (int i = 0; i < Selectable.PossibleNeighbour.Count; i++)
        {
            if (Selectable.PossibleNeighbour[i][0] + currentSelectable.positionQ == targetSelectable.positionQ &&
                Selectable.PossibleNeighbour[i][1] + currentSelectable.positionR == targetSelectable.positionR)
            {         
                rotation = i * 60f;
                break;
            }
        }
        return new Vector3(currentSelectable.transform.eulerAngles.x, rotation, currentSelectable.transform.eulerAngles.z);
    }

	public static int rotateEulDegrees(Selectable currentSelectable, Selectable targetSelectable)
	{
		byte rotationTarget = 0;

		for (byte i = 0; i < Selectable.PossibleNeighbour.Count; i++)
		{
			if (Selectable.PossibleNeighbour[i][0] + currentSelectable.positionQ == targetSelectable.positionQ &&
			    Selectable.PossibleNeighbour[i][1] + currentSelectable.positionR == targetSelectable.positionR)
			{         
				rotationTarget = i;
				break;
			}
		}
	
		int finalRotation = 0;

		finalRotation = (rotationTarget) * 60;;
		currentSelectable.rotationIndex = rotationTarget;

		return finalRotation;
	}
}