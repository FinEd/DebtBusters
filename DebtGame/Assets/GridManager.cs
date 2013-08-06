using UnityEngine;
using System.Collections;

public class GridManager : MonoBehaviour {
	
	public Texture blueChip;
	public Texture redChip;
	public Texture whiteChip;
	// What type of "chip" is this?
	//  0 = unitialized
	//  1 = blue  (savings)
	//  2 = red   (debt)
	//  3 = white (resource)
	int ChipType = 0;
	int ChipCount = 0;
	
	// Use this for initialization
	void Start () {
		Debug.Log("???");
	}
	
	// Set chip type and chip count for this square
	// Called by master class
	void Initialize(int ChipType, int ChipCount) {
		this.ChipType = ChipType;
		this.ChipCount = ChipCount;
		
		// Set our texture based on our chip type
		if(this.ChipType == 1) {
			renderer.material.SetTexture("_MainTex",blueChip);
			Debug.Log("I am a blue square" + this.guiTexture.name);
		}
		else if(this.ChipType == 2) {
			renderer.material.SetTexture("_MainTex",redChip);
			Debug.Log("I am a red square" + this.guiTexture.name);
		}
		else if(this.ChipType == 3) {
			renderer.material.SetTexture("_MainTex",whiteChip);
			Debug.Log("I am a white square" + this.guiTexture.name);
		}
	}
	
	void OnMouseDown() {
		Debug.Log("Clicked on " + this.name);
		renderer.material.SetTexture("_MainTex",whiteChip);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
