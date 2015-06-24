﻿//Alex Jungroth
//This controls shadow position placement
using UnityEngine;
using System.Collections;

public class Action3Script : MonoBehaviour {

	//This is from ActionScriptTemplate.cs and Action1Script.cs
	public int moneyRequired = 0;
	public int currentCost = 0;

	public GameObject gameController; //this is the game controller variable. It is obtained from the PlayerTurnsManager
	public GameObject inputManager; //this is the input manager varibale. Obtained from the PlayerTurnManager
	private GameObject[] players; //array which houses the players. Obtained from the Game Controller
	public GameObject uiController; //this is the UI controller variable. Obtained from the scene

	private int currentPlayer; //this variable finds which player is currently using his turn.

	private float distance;

	private Vector3 originalPosition; //this will save the original position that the player started at.
	private bool chosenPositionConfirmed = false; //final position has been chosen and confirmed.
	private Vector3 currentPos;

	//This is from game controller

	//holds the numbe of spawned players
	private int playersSpawned;

	private bool legalShadowPosition = true; //bool for checking if player is done

	//These are my variables

	//holds the shadow postion
	public GameObject shadowPosition;

	// Use this for initialization
	void Start () {
		gameController = GameObject.FindWithTag ("GameController");
		inputManager = GameObject.FindWithTag ("InputManager");
		uiController = GameObject.Find ("UI Controller");

		if (gameController != null) {
			//			voters = gameController.GetComponent<GameController> ().voters;
			players = gameController.GetComponent<GameController> ().players;
		} else {
			Debug.Log ("Failed to obtain voters and players array from Game Controller");
		}
		
		//Get's whose turn it is from the gameController. Then checks if he has enough money to perform the action
		currentPlayer = gameController.GetComponent<GameController> ().currentPlayerTurn;
		int actionCostMultiplier = this.transform.parent.GetComponent<PlayerTurnsManager> ().costMultiplier;
		if (players [currentPlayer].GetComponent<PlayerVariables> ().money < (moneyRequired + (moneyRequired * actionCostMultiplier))) {
			Debug.Log ("Current Player doesn't have enough money to make this action.");
			uiController.GetComponent<UI_Script>().toggleActionButtons();
			Destroy(gameObject);
		}
		
		originalPosition = players[currentPlayer].transform.position;
		//this.transform.position = originalPosition;

		//spawns in a shadow position for the player

		//instantiates a new instance of player that will be the shadow postion
		shadowPosition = Instantiate(gameController.GetComponent<GameController>().playerTemplate,new Vector3(0,0,0), Quaternion.identity) as GameObject;
		
		//changes the shadow postion transform's postion to the player who spawned it
		shadowPosition.transform.position =  players[currentPlayer].transform.position;

		//adds the shadow position to the players array list of shadowpositions
		gameController.GetComponent<GameController> ().playerTemplate.GetComponent<PlayerVariables> ().shadowPositions.Add (shadowPosition);

		//gets the number of spawned players
		playersSpawned = gameController.GetComponent<GameController> ().playersSpawned;

		//Disables the Action UI buttons
		uiController.GetComponent<UI_Script>().disableActionButtons();
	}
	
	// Update is called once per frame
	void Update () {
		//These below if statements check if the player has pressed a button down to move the character.
		//Right now, it makes sure that the player can only move up, down, left, right, forward, and backward from its original postion.
		//It also checks to make sure the player doesn't move outside the grid.
		//When we figure out the GUI system, we can change the inputs to then be button presses.

		//Tests to make sure that the shadow position does not overlap another player,
		//another shadow position, and that the shadow position does not move more than one space from the player
		currentPos = players [currentPlayer].transform.position;
		if (inputManager.GetComponent<InputManagerScript> ().rightButtonDown) {
			if (shadowPosition.transform.position.x < (gameController.GetComponent<GameController>().gridSize - 1))
			{	
				if(TestShadowPosition() && (shadowPosition.transform.position.x - originalPosition.x) <= 0) 
				{
					shadowPosition.transform.position += new Vector3(1,0,0);

					Debug.Log ("Cost to place a shadow position: $" + currentCost + ".");
				}
			}
		}
		if (inputManager.GetComponent<InputManagerScript> ().leftButtonDown) {
			if (shadowPosition.transform.position.x > 0)
			{
				if(TestShadowPosition() && (originalPosition.x - shadowPosition.transform.position.x) <= 0)
				{
					shadowPosition.transform.position += new Vector3(-1,0,0);

					Debug.Log ("Cost to place a shadow position: $" + currentCost + ".");
				}
			}
		}
		if (inputManager.GetComponent<InputManagerScript> ().downButtonDown) {
			if (shadowPosition.transform.position.z > 0)
			{	
				if(TestShadowPosition() && (originalPosition.z - shadowPosition.transform.position.z) <= 0)
				{
					shadowPosition.transform.position += new Vector3(0,0,-1);

					Debug.Log ("Cost to place a shadow position: $" + currentCost + ".");
				}
			}
		}
		if (inputManager.GetComponent<InputManagerScript> ().upButtonDown) {
			if (shadowPosition.transform.position.z < (gameController.GetComponent<GameController>().gridSize - 1))
			{
				if(TestShadowPosition()  && (shadowPosition.transform.position.z - originalPosition.z) <= 0)
				{
					shadowPosition.transform.position += new Vector3(0,0,1);

					Debug.Log ("Cost to place a shadow position: $" + currentCost + ".");
				}
			}
		}
		if (inputManager.GetComponent<InputManagerScript> ().qButtonDown) {
			if (shadowPosition.transform.position.y < (gameController.GetComponent<GameController>().gridSize - 1))
			{	
				if(TestShadowPosition() && (shadowPosition.transform.position.y - originalPosition.y) <= 0)
				{
					shadowPosition.transform.position += new Vector3(0,1,0);

					Debug.Log ("Cost to place a shadow position: $" + currentCost + ".");
				}
			}
		}
		if (inputManager.GetComponent<InputManagerScript> ().eButtonDown) {
			if (shadowPosition.transform.position.y > 0)
			{	
				if(TestShadowPosition() && (originalPosition.y - shadowPosition.transform.position.y) <= 0)
				{
					shadowPosition.transform.position += new Vector3(0,-1,0);

					Debug.Log ("Cost to place a shadow position: $" + currentCost + ".");
				}
			}
		}
		
		//Right Now, if you press Space bar, it confirms the chosen position.
		//You can only confirm the position if it isn't the exact same position you started at, or you are not sharing a position that another player is in
		//You also must have enough money to move to that position.
		if (Input.GetKeyDown (KeyCode.B)) {
			for (int i = 0; i < players.Length; i++) {
				if (i != currentPlayer && players[i].transform.position != shadowPosition.transform.position && shadowPosition.transform.position != originalPosition) {
					if (currentCost > players[currentPlayer].GetComponent<PlayerVariables>().money) {
						Debug.Log ("You don't have enough money to move to this spot!");
					} else {
						chosenPositionConfirmed = true;
						players[currentPlayer].GetComponent<PlayerVariables>().money = players[currentPlayer].GetComponent<PlayerVariables>().money - currentCost;
					}
				}
			}
		}
		
		if (chosenPositionConfirmed) {
			EndAction ();
		}

	}
	
	void EndAction() {
		uiController.GetComponent<UI_Script>().toggleActionButtons();
		this.transform.parent.GetComponent<PlayerTurnsManager> ().costMultiplier += 1;
		Destroy(gameObject);
	}

	bool TestShadowPosition(){

		//temporary holds a shadow position
		GameObject tempShadowPosition;

		//resets playerConifrmsPlacement
		legalShadowPosition = true;

		//checks the shadow position against all of the previous players to ensure that they don't overlap
		for(int i = 0; i < playersSpawned && legalShadowPosition; i++)
		{
			//we don't want to compare the current player to the new shadow postion as it will always fail that test
			//if (players[i].GetComponent<PlayerVariables>().transform.position != originalPosition
			//    && shadowPosition.transform.position == players[i].GetComponent<PlayerVariables>().transform.position)
			//{//if they are on the same spot
			//		legalShadowPosition = false;
			//}//if

			//if the count of any array list is zero, than that means there is nothing in it
			//and it should not be looked at as it will cause a runtime error
			if(players[i].GetComponent<PlayerVariables>().shadowPositions.Count > 0)
			{
				//the current player will have the shadow postion stored in its arraylist and we don't want the shadow position we
				//are creating to be compared to itself as that comparison would always fail the test
				if(players[i].GetComponent<PlayerVariables>().transform.position != originalPosition)
				{
					//checks the shadow position against all other shadow postions owned by a single player to ensure that they don't overlap
					for(int j = 0; j < players[i].GetComponent<PlayerVariables>().shadowPositions.Count && legalShadowPosition; j++)
					{
						//gets the next shadow postion in the arraylist
						tempShadowPosition = (GameObject)players[i].GetComponent<PlayerVariables>().shadowPositions[j];
						
						if(shadowPosition.transform.position == tempShadowPosition.transform.position)
						{
							//if two shadow positions are on the same spot
							legalShadowPosition = false;
						}//if
					}//for
				}//if
				else
				{
					//thus we have an additional test to make sure that the current player's arraylist of shadow postions is greater than one,
					//other wise there is no point in checking
					if(players[i].GetComponent<PlayerVariables>().shadowPositions.Count > 1)
						{
						//checks the shadow position against all other shadow postions owned by a single player to ensure that they don't overlap
						for(int j = 1; j < players[i].GetComponent<PlayerVariables>().shadowPositions.Count && legalShadowPosition; j++)
						{
							//gets the next shadow postion in the arraylist
							tempShadowPosition = (GameObject)players[i].GetComponent<PlayerVariables>().shadowPositions[j];
							
							if(shadowPosition.transform.position == tempShadowPosition.transform.position)
							{
								//if two shadow positions are on the same spot
								legalShadowPosition = false;
							}//if
						}//for
					}//if
				}//else
			}//if
		}//for

		//if the player placment is legal
		return legalShadowPosition;

	}//SpawnPlayer
	
}