using UnityEngine;
using System.Collections;

public class GridManager : MonoBehaviour {
	
	public GameObject chip;
	
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
		
	}
	
	// Set chip type and chip count for this square
	// Called by master class
	public void Initialize(int ChipType, int ChipCount, Vector3 location) {
		this.ChipType = ChipType;
		this.ChipCount = ChipCount;
		
		// Create the needed chip prefab
		if(this.ChipType == 1) {
			Debug.Log("I am a blue square with " + ChipCount + " chips");
			
			if(ChipCount >= 0) {
				// Set rotation to 270 around the x-axis
				// since we're using planes
				Quaternion rotation = Quaternion.identity;
				rotation.eulerAngles = new Vector3(90,0,0);
					
				// Instantiate an instance of our GridPrefab
				GameObject bChip = Instantiate(chip,location,rotation) as GameObject;
				bChip.renderer.material.SetTexture("_MainTex",blueChip);
			}
//			renderer.material.SetTexture("_MainTex",blueChip);
		}
		else if(this.ChipType == 2) {
			Debug.Log("I am a red square with " + ChipCount + " chips");
			
			if(ChipCount >= 0) {
				// Set rotation to 270 around the x-axis
				// since we're using planes
				Quaternion rotation = Quaternion.identity;
				rotation.eulerAngles = new Vector3(90,0,0);
					
				// Instantiate an instance of our GridPrefab
				GameObject rChip = Instantiate(chip,location,rotation) as GameObject;
				rChip.renderer.material.SetTexture("_MainTex",redChip);
			}
//			renderer.material.SetTexture("_MainTex",redChip);
		}
		else if(this.ChipType == 3) {
			Debug.Log("I am a white square with " + ChipCount + " chips");
			
			if(ChipCount >= 0) {
				// Set rotation to 270 around the x-axis
				// since we're using planes
				Quaternion rotation = Quaternion.identity;
				rotation.eulerAngles = new Vector3(90,0,0);
					
				// Instantiate an instance of our GridPrefab
				GameObject wChip = Instantiate(chip,location,rotation) as GameObject;
				wChip.renderer.material.SetTexture("_MainTex",whiteChip);
			}			
//			renderer.material.SetTexture("_MainTex",whiteChip);
		}
		else {
			Debug.Log("I am an enigma!");

			if(ChipCount >= 0) {
				// Set rotation to 270 around the x-axis
				// since we're using planes
				Quaternion rotation = Quaternion.identity;
				rotation.eulerAngles = new Vector3(90,0,0);
					
				// Instantiate an instance of our GridPrefab
				GameObject wChip = Instantiate(chip,location,rotation) as GameObject;
				wChip.renderer.material.SetTexture("_MainTex",whiteChip);
			}
		}
	}
	
	void OnMouseDown() {
		Debug.Log("Clicked on " + this.name);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
