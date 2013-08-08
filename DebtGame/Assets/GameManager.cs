using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {	
		public static void swipe(GridManager tileFrom, GridManager tileTo) {
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
	
	// works for 9-grid, needs modification for 25-grid or more
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
		
		return false;
	}

	public static GameManager _instance = null;
	
	public static GameManager getInstance() {
		if(_instance == null) _instance = new GameManager();
		_instance.Start();
		return _instance;
	}
	
	public static bool roundInProgress = false;
	static GridManager tileFrom = null;
	static GridManager tileTo = null;
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
			}
			else Debug.Log("Invalid swipe, tiles are not neighbors");
		}
		tileFrom = null;
		tileTo = null;
	}
	
	//Instance
	
	public static int TILE_WIDTH = 3;
	public static int TILE_HEIGHT = 3;
	
	public static bool easyMode = true;
	
	public static int roundMoves = 6;
	public static int roundCount = 0;
	
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
	void setEmptySpaces() {
		emptySpaces = 0;
		for(int i=0; i < TILE_WIDTH; i++)
			for(int j=0; j < TILE_HEIGHT; j++) {
			GridManager tile = clones[i, j];
			if(tile.setEmptySpace()) emptySpaces ++;
		}
	}

	// Called at the beginning of each round
	// Double the blues and triple the reds
	void compoundTiles() {
		for(int i=0; i < TILE_WIDTH; i++)
			for(int j=0; j < TILE_HEIGHT; j++) {
			GridManager tile = clones[i, j];
			tile.compound();
		}
		Debug.Log("Tiles compounded");
	}
	

	void moveBlues() {
	}
	
	// Called at the beginning of each round
	void replaceBlanks() {
		int whitesToAdd = emptySpaces;
		int redsToAdd = emptySpaces;
		Debug.Log("Replacing " + emptySpaces + " empty spaces");
		
		// For each blank tile, if random even add a white (positive) else add a red (negative)
		for(int i=0; i < TILE_WIDTH; i++)
			for(int j=0; j < TILE_HEIGHT; j++) {
			GridManager tile = clones[i, j];
			if(i==1 & j==1) {
				tile.setType(GridManager.TILE_BLUE);
				continue;
			}
			int rnd = (new System.Random()).Next(1,600);
			if(tile.getCount() > 0) {
				continue; // this tile is blue or red and is non-empty
			}
			if(rnd % 2 == 0) {
				Debug.Log("rnd even");
				tile.setType(GridManager.TILE_WHITE);
				tile.setCount(1);
				whitesToAdd--;
			} else {
				Debug.Log("rnd odd");
				tile.setType(GridManager.TILE_RED);
				tile.setCount(1);
				redsToAdd--;
			}
		}
		
		while(whitesToAdd > 0)
		for(int i=0; i < TILE_WIDTH; i++)
			for(int j=0; j < TILE_HEIGHT; j++) {
				if(whitesToAdd == 0) break;
				if(i==1 & j==1) continue;
				GridManager tile = clones[i, j];
				if(tile.getType() == GridManager.TILE_WHITE) {
					if(easyMode) {
						tile.setCount(tile.getCount()+1);
						whitesToAdd--;
					} else {
						int rnd = (new System.Random()).Next(1,600);
						// add if even, skip if odd
						if(rnd % 2 == 0) {
							Debug.Log("appending white");
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
				GridManager tile = clones[i, j];
				if(tile.getType() == GridManager.TILE_RED) {
					if(easyMode) {
						tile.setCount(tile.getCount()+1);
						redsToAdd--;
					} else {
						int rnd = (new System.Random()).Next(1,600);
						// add if even, skip if odd
						if(rnd % 2 == 0) {
							Debug.Log("appending red");
							tile.setCount(tile.getCount()+1);
							redsToAdd--;
						}
					}
				}
		}
		
		for(int i=0; i < TILE_WIDTH; i++)
			for(int j=0; j < TILE_HEIGHT; j++) {
			GridManager tile = clones[i, j];
			tile.ShowChip();
			//DEBUG only
			Debug.Log("Tile " + (i*TILE_WIDTH + j) + ": , type " + tile.getTypeName() + ", count " + tile.getCount());
		}

	}
	
	// called each time the boggle button is pressed
	public void startNewRound() {	
		if(easyMode) {
			// compound the tiles: double the blues and triple the reds
			Debug.Log("GameMaster: starting new round, empty spaces: " + emptySpaces);
			compoundTiles();
			moveBlues();
		} else {
			moveBlues();
			// compound the tiles: double the blues and triple the reds
			compoundTiles();
		}
			
		replaceBlanks();
		GameManager.roundInProgress = true;
	}

}