using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {	
	
	private static readonly System.Random random = new System.Random();

	static GridManager tileFrom = null;
	static GridManager tileTo = null;
	public static bool movingBluesState = false;
	
	public static void resetMovingBluesState() {
		((GameManager) UnityEngine.Object.FindObjectOfType(typeof(GameManager))).replenishEmptySpacesCount();
		movingBluesState = false;
	}
	
	public static void tileClicked(GridManager tile) {
		if(tileFrom == null) {
			tileFrom = tile;
			return;
		}
		// first tile already set
		if(tileFrom == tile) Debug.Log("Invalid click: From and To tiles are the same!");
		else {
			tileTo = tile;
			// Check to make sure the tiles are valid neighbors and then apply the swipe
			if(GameManager.isNeighbor(tileFrom, tileTo)) {
				Debug.Log("Tiles are valid neighbors");
				GameManager.swipe(tileFrom, tileTo);
				roundMoves--;
				GameManager manager = (GameManager) UnityEngine.Object.FindObjectOfType(typeof(GameManager));
				manager.endGameCondition();
			}
			else Debug.Log("Invalid swipe, tiles are not neighbors");
		}
		if(tileFrom != null)tileFrom.cleanGrid();
		if(tileTo != null)tileTo.cleanGrid();
		tileFrom = null;
		tileTo = null;
	}
	
	public static void swipe(GridManager tileFrom, GridManager tileTo) {
		
		if(tileFrom.getType() == GridManager.TILE_BLUE) {
			if(!GameManager.movingBluesState) {
				Debug.Log("Invalid swipe from blue tile");
				return;
			} else {
				// for simplicity allow only one white to be created from blue per swipe
				// Perhaps encourages not to swipe from blue too many times
				tileTo.setCount(tileTo.getCount() + 1);
				tileFrom.setCount(tileFrom.getCount() - 1);
				return;
			}
		}
		
		
		if(tileFrom.getType() == GridManager.TILE_RED ||
			tileFrom.getType() == GridManager.TILE_BLUE ||
			tileFrom.getType() == GridManager.TILE_NONE ||
			tileTo.getType() == GridManager.TILE_NONE) {
			Debug.Log("Invalid swipe, tileFrom type is " + tileFrom.getTypeName() + ", tileTo type is " + tileTo.getTypeName());
			// do nothing
		} else {
			int origFromCount = tileFrom.getCount();
			string fromName = tileFrom.getTypeName();
			int origToCount = tileTo.getCount();
			string toName = tileTo.getTypeName();
			// tileFrom is white.
			if(tileTo.getType() == GridManager.TILE_WHITE) { // ||tileTo.getType() == GridManager.TILE_NONE) {
				// tileFrom count remains as is, tileTo count increases by tileFrom count
				tileTo.setCount(tileTo.getCount() + tileFrom.getCount());
			} else if(tileTo.getType() == GridManager.TILE_BLUE) {
				// tileFrom reset to count 0 and type none, tileTo count increases by tileFrom count
				tileTo.setCount(tileTo.getCount() + tileFrom.getCount());
				tileFrom.setCount(0);
			} else {
				// tileTo is red
				if(tileFrom.getCount() > tileTo.getCount()) {
					tileFrom.setCount(tileFrom.getCount() - tileTo.getCount());
					tileTo.setCount(0);
				} else if(tileFrom.getCount() == tileTo.getCount()) {
					tileFrom.setCount(0);
					tileTo.setCount(0);
				} else {
					tileTo.setCount(tileTo.getCount() - tileFrom.getCount());
					tileFrom.setCount(0);
				}
			}
			Debug.Log("From Tile " + fromName + ": orig count=" + origFromCount + ", new count=" + tileFrom.getCount());
			Debug.Log("To Tile " + toName + ": orig count=" + origToCount + ", new count=" + tileTo.getCount());
		}
	}
	
	// 8-connected works for any grid size
	public static bool isNeighbor(GridManager tileFrom, GridManager tileTo) {
		GridManager.Position posFrom = tileFrom.getPosition();
		GridManager.Position posTo = tileTo.getPosition();
		// center is neighbor to all in a 9-grid case
		if(posFrom.i == 1 && posFrom.j == 1) return true;
		if(posTo.i == 1 && posTo.j == 1) return true;
		// 4-connected
		if(posFrom.i == posTo.i &&
			(System.Math.Abs(posFrom.j - posTo.j) == 1)) return true;
		if(posFrom.j == posTo.j &&
			(System.Math.Abs(posFrom.i - posTo.i) == 1)) return true;
		// 8-connected
		if((System.Math.Abs(posFrom.i - posTo.i) == 1) &&
			(System.Math.Abs(posFrom.j - posTo.j) == 1)) return true;
		
		return false;
	}

	public static bool roundInProgress = false;
	//Instance
	public static int MOVES_PER_ROUND = 6;
	
	public static int TILE_WIDTH = 3;
	public static int TILE_HEIGHT = 3;
	
	public static bool easyMode = true;
	
	public static int roundMoves = MOVES_PER_ROUND;
	public static int roundCount = 0;
	bool startOfGame = false;
	// At start of game this = n-1 empty spaces and then gets updated from the tile status at beginning of 
	// a new round
	int emptySpaces = 0;

	// Our Prefab grid piece and a container
	// to hold them all for later reference
	public GameObject grid;
	
	//static List<GameObject> clones = new List<GameObject>();
	GridManager[,] clones = new GridManager[TILE_WIDTH,TILE_HEIGHT];
	

	// Use this for initialization
	void Start () {
		// all are empty to start with
		emptySpaces = TILE_WIDTH * TILE_HEIGHT - 1;

		// Calculate the middle box to set
		// grid piece locations
		int mid = TILE_WIDTH/2;
		
		// The z-position is fixed and sits
		// above our ground object
		float z = -.5f;
		
		// Nested loop structure to build our 
		// grid pieces. The middle box will have
		// x,y of 0,0. All boxes above and to the left
		// will span from here at +/- 50
		for (int i = 0; i < TILE_WIDTH; i++) {
			float y = (mid-i)*50;

			for (int j = 0; j < TILE_HEIGHT; j++) {
				float x = (mid-j)*-50;
				
				// Set rotation to 270 around the x-axis
				// since we're using planes
				Quaternion rotation = Quaternion.identity;
				rotation.eulerAngles = new Vector3(270,0,0);
				
				// Instantiate an instance of our GridPrefab
				GameObject clone = Instantiate(grid,new Vector3(x,y,z),rotation) as GameObject;
				if(clone != null) {
					clones[i,j] = clone.GetComponent<GridManager>();
					clones[i,j].Initialize(i,j,new Vector3(x,y,z));
				} else 	Debug.Log("GridManager clone is NULL!!!");

			}
		}
		startOfGame = true;
		Debug.Log("GameMaster: Let the game begin!!");
		startNewRound();

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	int getEmptySpaces() {
		return emptySpaces;
	}
	
	// called at the end of each round
	public void setEmptySpaces() {
		emptySpaces = 0;
		for(int i=0; i < TILE_WIDTH; i++)
			for(int j=0; j < TILE_HEIGHT; j++) {
			if(i == 1 && j == 1) continue;
			GridManager tile = clones[i, j];
			if(tile.setEmptySpace()) emptySpaces ++;
		}
		Debug.Log("setEmptySpaces: calculated emptySpaces = " + emptySpaces);
	}

	public void replenishEmptySpacesCount() {
		emptySpaces = 0;
		for(int i=0; i < TILE_WIDTH; i++)
			for(int j=0; j < TILE_HEIGHT; j++) {
			if(i == 1 && j == 1) continue;
			GridManager tile = clones[i, j];
			if(tile.getType() == GridManager.TILE_NONE) emptySpaces ++;
		}
		Debug.Log("replenishEmptySpacesCount: re-calculated emptySpaces = " + emptySpaces);

	}
	
	// Called at the beginning of each round
	// Double the blues and triple the reds
	void compoundTiles() {
		for(int i=0; i < TILE_WIDTH; i++)
			for(int j=0; j < TILE_HEIGHT; j++) {
			GridManager tile = clones[i, j];
			tile.compound();
		}
	}
	
	void moveBlues() {
		GameManager.movingBluesState = true;
	}
	
	public void endGameCondition() {
		if(GameManager.movingBluesState) return;
		// check for win
		if(checkForWin()) Debug.Log("CONGRATULATIONS!! You won the game!");
		if(checkNoMoreMoves()) {
			Debug.Log("You have no more moves this round! Allocate blue chips and continue.");
			setEmptySpaces();

			moveBlues(); // To be implemented
		}
	}
	
	public bool checkForWin() {
		// Win condition:
		// No reds and Blue exists
		bool hasBlue = false;
		for(int i=0; i < TILE_WIDTH; i++) {
			for(int j=0; j < TILE_HEIGHT; j++) {
				GridManager tile = clones[i, j];
				if(tile.getType() == GridManager.TILE_RED) {
					return false;
				}
				if(tile.getType() == GridManager.TILE_BLUE) {
					hasBlue = true;
				}
			}
		}
		return hasBlue;
	}
	
	public bool checkNoMoreMoves() {
		if(roundMoves == 0) return true;
		for(int i=0; i < TILE_WIDTH; i++) {
			for(int j=0; j < TILE_HEIGHT; j++) {
				GridManager tile = clones[i, j];
				if(tile.getType() == GridManager.TILE_WHITE) {
					return false;
				}
			}
		}
		
		return true;
	}
	// Called at the beginning of each round
	void replaceBlanks() {
		int whitesToAdd = emptySpaces;
		int redsToAdd = emptySpaces;
		List<string> existingReds = new List<string>();
		List<string> existingWhites = new List<string>();
		Debug.Log("Replacing " + emptySpaces + " empty spaces");
		
		bool placedAtleastOneWhite = false;
		bool placedAtleastOneRed = false;
		// For each blank tile, if random even add a white (positive) else add a red (negative)
		for(int i=0; i < TILE_WIDTH; i++)
			for(int j=0; j < TILE_HEIGHT; j++) {
			GridManager tile = clones[i, j];
			if(i==1 & j==1) {
				tile.setType(GridManager.TILE_BLUE);
				continue;
			}
			//int rnd = (new System.Random()).Next(1,600);
			float rnd = GameManager.random.Next(1,600);
			//Debug.Log("Random value = " + rnd);
			if(tile.getCount() > 0) {
				if(tile.getType() == GridManager.TILE_RED) {
					existingReds.Add (Convert.ToString(i*TILE_WIDTH+j));
					//Debug.Log("Non-zero red at " + i + "," + j);
				}
				else if(tile.getType() == GridManager.TILE_WHITE) {
					existingWhites.Add (Convert.ToString(i*TILE_WIDTH+j));
					//Debug.Log("Non-zero red at " + i + "," + j);
				}
				continue; // this tile is blue or red and is non-empty
			}
			Debug.Log("Tile("+i+","+j+") type:"+tile.getTypeName()+" count="+tile.getCount());
			if(rnd % 2 == 0) {
			//if(rnd < 0.5) {
				tile.setType(GridManager.TILE_WHITE);
				tile.setCount(1);
				whitesToAdd--;
				placedAtleastOneWhite = true;
			} else {
				tile.setType(GridManager.TILE_RED);
				tile.setCount(1);
				redsToAdd--;
				placedAtleastOneRed = true;
			}
		}
		
		if(!placedAtleastOneWhite || !placedAtleastOneRed) {
			Debug.Log("Sorry! You have lost the game!");
			//Application.Quit();
			return;
		}

		if(whitesToAdd == 8) {
			clones[2,2].toggleTexture();
			clones[2,2].setCount(8);
			clones[0,0].setCount(2);
			Debug.Log("All red case, setting tile 8 to white");
			redsToAdd=0;
			whitesToAdd=0;
			placedAtleastOneWhite = true;
		}
		else if(redsToAdd == 8) {
			clones[2,2].toggleTexture();
			clones[2,2].setCount(8);
			clones[0,0].setCount(2);
			Debug.Log("All white case, setting tile 8 to red");
			redsToAdd=0;
			whitesToAdd=0;
		}
		
		if(!placedAtleastOneWhite) {
			Debug.Log("Sorry! You have lost the game!");
			//Application.Quit();
			return;
		}
		while(whitesToAdd > 0)
		for(int i=0; i < TILE_WIDTH; i++)
			for(int j=0; j < TILE_HEIGHT; j++) {
				if(whitesToAdd == 0) break;
				if(i==1 & j==1) continue;
				string pos = Convert.ToString(i*TILE_WIDTH+j);
				if(existingWhites.Contains(pos)) {
					Debug.Log("Skipping white at location " + i + ", " + j);
					continue;
				}
				GridManager tile = clones[i, j];
				if(tile.getType() == GridManager.TILE_WHITE) {
					if(easyMode) {
						tile.setCount(tile.getCount()+1);
						whitesToAdd--;
					} else {
						int rnd = (new System.Random()).Next(1,600);
						// add if even, skip if odd
						if(rnd % 2 == 0) {
							tile.setCount(tile.getCount()+1);
							whitesToAdd--;
						}
					}
				}
		}
		
		while(redsToAdd > 0)
		for(int i=0; i < TILE_WIDTH; i++)
			for(int j=0; j < TILE_HEIGHT; j++) {
				if(redsToAdd == 0) break;
				if(i==1 & j==1) continue;
				string pos = Convert.ToString(i*TILE_WIDTH+j);
				if(existingReds.Contains(pos)) {
					Debug.Log("Skipping red at location " + i + ", " + j);
					continue;
				}
				GridManager tile = clones[i, j];
				if(tile.getType() == GridManager.TILE_RED) {
					if(easyMode) {
						tile.setCount(tile.getCount()+1);
						redsToAdd--;
					} else {
						int rnd = (new System.Random()).Next(1,600);
						// add if even, skip if odd
						if(rnd % 2 == 0) {
							tile.setCount(tile.getCount()+1);
							redsToAdd--;
						}
					}
				}
		}
		
		for(int i=0; i < TILE_WIDTH; i++)
			for(int j=0; j < TILE_HEIGHT; j++) {
			GridManager tile = clones[i, j];
			//DEBUG only
			Debug.Log("Tile " + (i*TILE_WIDTH + j) + ": , type " + tile.getTypeName() + ", count " + tile.getCount());
		}

	}
	
	// called each time the boggle button is pressed
	public void startNewRound() {
		GameManager.roundMoves = MOVES_PER_ROUND;
		// reset movingBlues if needed
		GameManager.resetMovingBluesState();
		compoundTiles();
		replaceBlanks();
		GameManager.roundInProgress = true;
	}
	
	void OnGUI() {
		//GUI.Label(new Rect(0, 0, 100, 100), "Hi!");
    	GUI.Label(new Rect(300, 200, 100, 100), "Moves Remaining: " + GameManager.roundMoves);
    }
}