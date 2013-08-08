using UnityEngine;
using System.Collections;

public class GridManager : MonoBehaviour {
	
		// Tile type
	public const int TILE_NONE = 0;
	public const int TILE_WHITE = 1;
	public const int TILE_RED = 2;
	public const int TILE_BLUE = 3;

	public GameObject chip;
	
	public Texture blueChip;
	public Texture redChip;
	public Texture whiteChip;
	public Texture selectedGrid;

	// What type of "chip" is this?
	//  0 = unitialized
	//  1 = blue  (savings)
	//  2 = red   (debt)
	//  3 = white (resource)
	int ChipType = 0;
	int ChipCount = 0;
	Position ChipPosition;
	Vector3 ChipLocation;

	
	// Use this for initialization
	void Start () {
		Debug.Log("GridManager: in start()");
		ChipType = TILE_NONE;
		ChipCount = 0;

	}
	
	public void Initialize(int i, int j, Vector3 location) {
		this .ChipPosition = new Position(i, j);
		this.ChipLocation = location;
	}
	
	// Set chip type and chip count for this square
	// Called by master class
	public void ShowChip() {
		// Create the needed chip prefab
		if(this.ChipType == TILE_BLUE) {
			Debug.Log("I am a blue square with " + ChipCount + " chips");
			
			// Set rotation to 270 around the x-axis
			// since we're using planes
			Quaternion rotation = Quaternion.identity;
			rotation.eulerAngles = new Vector3(90,0,0);
					
			// Instantiate an instance of our GridPrefab
			GameObject bChip = Instantiate(chip,ChipLocation,rotation) as GameObject;
			bChip.renderer.material.SetTexture("_MainTex",blueChip);
		}
		else if(this.ChipType == TILE_RED) {
			Debug.Log("I am a red square with " + ChipCount + " chips");
			
			// Set rotation to 270 around the x-axis
			// since we're using planes
			Quaternion rotation = Quaternion.identity;
			rotation.eulerAngles = new Vector3(90,0,0);
				
			// Instantiate an instance of our GridPrefab
			GameObject rChip = Instantiate(chip,ChipLocation,rotation) as GameObject;
			rChip.renderer.material.SetTexture("_MainTex",redChip);
		}
		else if(this.ChipType == TILE_WHITE) {
			Debug.Log("I am a white square with " + ChipCount + " chips");
			
			// Set rotation to 270 around the x-axis
			// since we're using planes
			Quaternion rotation = Quaternion.identity;
			rotation.eulerAngles = new Vector3(90,0,0);
					
			// Instantiate an instance of our GridPrefab
			GameObject wChip = Instantiate(chip,ChipLocation,rotation) as GameObject;
			wChip.renderer.material.SetTexture("_MainTex",whiteChip);
		}
		else {
			Debug.Log("I am an enigma! Chip type is " + ChipType);
		}
	}
	
	void OnMouseEnter() {
		Debug.Log("Mouse Enter " + this.name + " position: " + ChipPosition.i + ", " + ChipPosition.j);
	}
	
	void OnMouseExit() {
		Debug.Log("Mouse Exit " + this.name + " position: " + ChipPosition.i + ", " + ChipPosition.j);
	}
	
	void OnMouseDown() {
		Debug.Log("Mouse Down " + this.name + " position: " + ChipPosition.i + ", " + ChipPosition.j);
		renderer.material.SetTexture("_MainTex",selectedGrid);	
	}
	
	void OnMouseUp() {
		Debug.Log("Mouse Up " + this.name + " position: " + ChipPosition.i + ", " + ChipPosition.j);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void setCount(int count) {
		this.ChipCount = count;
		if(count == 0) ChipType = TILE_NONE;
	}
	
	public int getCount() {
		if(ChipCount == 0) ChipType = TILE_NONE;
		return ChipCount;
	}
	
	public Position getPosition() {
		return ChipPosition;
	}
	
	public bool setEmptySpace() {
		// remove white tiles
		if(ChipType == TILE_WHITE) ChipCount = 0;
		
		if(ChipCount == 0) {
			ChipType = TILE_NONE;
			return true;
		}
		return false;
	}
	
	public int getType() {
		return ChipType;
	}
	
	public void setType(int type) {
		ChipType = type;
	}
	
	public string getTypeName() {
		switch(ChipType) {
		case TILE_WHITE: return "WHITE";
		case TILE_RED: return "RED";
		case TILE_BLUE: return "BLUE";
		case TILE_NONE: return "NONE";
		default: return "unknown";
		}
	}
	
	// compound logic
	public void compound() {
		if(ChipType == TILE_BLUE) ChipCount *= 2;
		else if(ChipType == TILE_RED) ChipCount *= 3;
		else if(ChipType == TILE_WHITE) {
			ChipCount = 0;
			ChipType = TILE_NONE;
		}
	}

	public class Position {
		public int i; //row
		public int j; // col
		
		public Position(int i, int j) {
			this.i = i;
			this.j = j;
		}
	}
}