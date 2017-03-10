using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BoundsTest {
	center, //Is the center of the game object on the screen?
	onScreen, //are the bounds entirely on the screen?
	offScreen //are the bounds entirely off the screen?
}

public class Utils : MonoBehaviour {
	//============================= Bounds Functions =============================\\
	//Creates bounds that encapsulate the two Bounds passed in
	public static Bounds BoundsUnion(Bounds b0, Bounds b1){
		//if the size of one of the bounds is Vector3.zero, ignore
		if (b0.size == Vector3.zero && b1.size != Vector3.zero) {
			return (b1);
		} else if (b0.size != Vector3.zero && b1.size == Vector3.zero) {
			return (b0);
		} else if (b0.size == Vector3.zero && b1.size == Vector3.zero) {
			return (b0);
		}
		//Stretch b0 to include b1.min and b1.max
		b0.Encapsulate(b1.min);
		b0.Encapsulate (b1.max);
		return (b0);
	}

	public static Bounds CombineBoundsOfChildren(GameObject go){
		//Create an empty bounds b
		Bounds b = new Bounds(Vector3.zero, Vector3.zero);
		//If this GameObject has a renderer component
		if (go.GetComponent<Renderer> () != null) {
			//expand b to contain the renderer's bounds
			b = BoundsUnion(b, go.GetComponent<Renderer>().bounds);
		}
		//If this GameObject has a Collider component
		if (go.GetComponent<Collider> () != null) {
			//expand b to contain the collider's bounds
			b = BoundsUnion(b, go.GetComponent<Collider>().bounds);
		}
		//recursively iterate through each child of this gameobject.transform
		foreach (Transform t in go.GetComponent<Transform>()) {
			//expand b to contain their Bounds as well
			b = BoundsUnion(b, CombineBoundsOfChildren(t.gameObject));
		}
		return (b);
	}

	//Make a static read-only property camBounds
	static public Bounds camBounds {
		get {
			//if _camBounds hasn't been set yet
			if (_camBounds.size == Vector3.zero){
				//Set CameraBounds using the default Camera
				SetCameraBounds();
			}
			return (_camBounds);	
		}
	}

	//Private static field that camBounds uses
	static private Bounds _camBounds;

	//Function is used by camBounds to set _camBounds and can also be called directly
	public static void SetCameraBounds(Camera cam = null){
		//If no camera is passed in, use the main Camera
		if (cam == null) cam = Camera.main;
			//this sets the Camera to orthographic and rotation of R[0,0,0]
		//Make Vector3's at topLeft and bottomRight of Screen coords
		Vector3 topLeft = new Vector3(0,0,0);
		Vector3 bottomRight = new Vector3 (Screen.width, Screen.height, 0);
		//convert to world coordinates
		Vector3 boundTLN = cam.ScreenToWorldPoint(topLeft);
		Vector3 boundBRF = cam.ScreenToWorldPoint (bottomRight);
		//Adjust their z's to be at the near and far Camera clipping planes
		boundTLN.z += cam.nearClipPlane;
		boundBRF.z += cam.farClipPlane;
		//Find center of the bounds
		Vector3 center = (boundTLN + boundBRF)/2f;
		_camBounds = new Bounds (center, Vector3.zero);
		//expand _camBounds to encapsulate extents
		_camBounds.Encapsulate(boundTLN);
		_camBounds.Encapsulate (boundBRF);
	}

	//checks to see whether Bounds bnd are within the camBounds
	public static Vector3 ScreenBoundsCheck(Bounds bnd, BoundsTest test = BoundsTest.center){
		return (BoundsInBoundsCheck (camBounds, bnd, test));
	}

	//checks to see whether Bounds lilB are within Bounds bigB
	public static Vector3 BoundsInBoundsCheck(Bounds bigB, Bounds lilB, BoundsTest test = BoundsTest.onScreen){
		//behaviour of function is different based on BoundsTest that has been selected
		//get center of lilB
		Vector3 pos = lilB.center;
		//initialize offset at [0,0,0]
		Vector3 off = Vector3.zero;

		switch (test) {
		//center test determines what off has to be applied to lilB to move its center back inside bigB
		case BoundsTest.center:
			if (bigB.Contains (pos)) {
				return (Vector3.zero);
			}
			if (pos.x > bigB.max.x) {
				off.x = pos.x - bigB.max.x;
			} else if (pos.x < bigB.min.x) {
				off.x = pos.x - bigB.min.x;
			}
			if (pos.y > bigB.max.y) {
				off.y = pos.y - bigB.max.y;
			} else if (pos.y < bigB.min.y) {
				off.y = pos.y - bigB.min.y;
			}
			if (pos.z > bigB.max.z) {
				off.z = pos.z - bigB.max.z;
			} else if (pos.z < bigB.min.z) {
				off.z = pos.z - bigB.min.z;
			}
			return (off);
		
		//onScreen test determines what off has to be applied to keep all of lilB inside bigB
		case BoundsTest.onScreen:
			if (bigB.Contains (lilB.min) && bigB.Contains (lilB.max)) {
				return (Vector3.zero);
			}
			if (lilB.max.x > bigB.max.x) {
				off.x = lilB.max.x - bigB.max.x;
			} else if (lilB.min.x < bigB.min.x) {
				off.x = lilB.min.x - bigB.min.x;
			}
			if (lilB.max.y > bigB.max.y) {
				off.y = lilB.max.y - bigB.max.y;
			} else if (lilB.min.y < bigB.min.y) {
				off.y = lilB.min.y - bigB.min.y;
			}
			if (lilB.max.z > bigB.max.z) {
				off.z = lilB.max.z - bigB.max.z;
			} else if (lilB.min.z < bigB.min.z) {
				off.z = lilB.min.z - bigB.min.z;
			}
			return (off);

		//offScreen test determines what needs to be applied to move any tiny bit of lilB into bigB
		case BoundsTest.offScreen:
			bool cMin = bigB.Contains (lilB.min);
			bool cMax = bigB.Contains (lilB.max);
			if (cMin || cMax) {
				return (Vector3.zero);
			}
			if (lilB.min.x > bigB.max.x) {
				off.x = lilB.min.x - bigB.max.x;
			} else if (lilB.max.x < bigB.min.x) {
				off.x = lilB.max.x - bigB.min.x;
			}
			if (lilB.min.y > bigB.max.y) {
				off.y = lilB.min.y - bigB.max.y;
			} else if (lilB.max.y < bigB.min.y) {
				off.y = lilB.max.y - bigB.min.y;
			}
			if (lilB.min.z > bigB.max.z) {
				off.z = lilB.min.z - bigB.max.z;
			} else if (lilB.max.z < bigB.min.z) {
				off.z = lilB.max.z - bigB.min.z;
			}
			return (off);
		}
		return (Vector3.zero);
	}

	//============================ Transform Functions ===========================\\

	// This function will iteratively climb up the transform.parent tree
	//   until it either finds a parent with a tag != "Untagged" or no parent
	public static GameObject FindTaggedParent(GameObject go) {              // 1
		// If this gameObject has a tag
		if (go.tag != "Untagged") {                                         // 2
			// then return this gameObject
			return(go);
		}
		// If there is no parent of this Transform
		if (go.transform.parent == null) {                                  // 3
			// We've reached the top of the hierarchy with no interesting tag
			// So return null
			return( null );
		}
		// Otherwise, recursively climb up the tree
		return( FindTaggedParent( go.transform.parent.gameObject ) );       // 4
	}

	// This version of the function handles things if a Transform is passed in
	public static GameObject FindTaggedParent(Transform t) {                // 5
		return( FindTaggedParent( t.gameObject ) );
	} 

	//=========================== Materials Functions ============================\\

	//Returns a list of all Materials on this GameObject or its children
	static public Material[] GetAllMaterials(GameObject go){
		List<Material> mats = new List<Material> ();
		if (go.GetComponent<Renderer> () != null) {
			mats.Add (go.GetComponent<Renderer> ().material);
		}
		foreach (Transform t in go.transform) {
			mats.AddRange (GetAllMaterials (t.gameObject));
		}
		return(mats.ToArray ());
	}


}
