using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireUp : MonoBehaviour {

//	public LineRenderer gunRenderer;
//	public PlayerShooting playerShootig;
	BoxCollider mCollider;

	// Use this for initialization
	void Start () {
		mCollider = GetComponent<BoxCollider> ();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") {
			CompleteProject.PlayerShooting shooting = 
				other.gameObject.GetComponentInChildren<CompleteProject.PlayerShooting> ();
			if (shooting) {
				shooting.fireUpdate ();
				mCollider.isTrigger = false;
				transform.parent.gameObject.SetActive (false);
//				gameObject.SetActive (false);
			}
		}
	}
		
}
