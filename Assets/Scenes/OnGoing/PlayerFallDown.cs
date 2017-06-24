using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using CompleteProject;

public class PlayerFallDown : MonoBehaviour {


	MeshCollider surfaceCollider;
	// Use this for initialization
	void Start () {
//		surfaceCollider = GetComponent<MeshCollider> ();
	}

	void OnTriggerExit(Collider other){
		if (other.tag == "Player")
			other.GetComponent<CompleteProject.PlayerHealth> ().FallDown();
//			Debug.Log("player exit");
	}
//
//	void OnTriggerEnter(Collider other)
//	{
//		if (other.tag == "Player")
//			Debug.Log ("Player enter");
//	}
//
//	void OnTiggerStay(Collider other)
//	{
//		if (other.tag == "Player")
//			Debug.Log ("PlayerStay");
//	}
}
