using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	// Our Prefab grid piece
	public Transform grid;
	
	// Use this for initialization
	void Start () {

		// Instatiate grid sizing
		int rows = 3;
		int cols = 3;
		
		// Calculate the middle box to set
		// grid piece locations
		int mid = rows/2;
		
		// The z-position is fixed and sits
		// above our ground object
		float z = -.5f;
		
		// Nested loop structure to build our 
		// grid pieces. The middle box will have
		// x,y of 0,0. All boxes above and to the left
		// will span from here at +/- 50
		for (int i = 0; i < rows; i++) {
			float y = 0;
		
			if(i < mid) {
				y = (mid-i)*50;
			}
			else if(i > mid) {
				y = (mid-i)*50;
			}

			for (int j = 0; j < cols; j++) {
				float x = 0;
			
				if(j < mid) {
					x = (mid-j)*50;
				}
				else if(j > mid) {
					x = (mid-j)*50;
				}

				// Set rotation to 270 around the x-axis
				// since we're using planes
				Quaternion rotation = Quaternion.identity;
				rotation.eulerAngles = new Vector3(270,0,0);
				
				// Instantiate an instance of our GridPrefab
				Instantiate(grid,new Vector3(x,y,z),rotation);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}