﻿using UnityEngine;
using System.Collections;

public class GroundMoveable : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseOver(){
		if( Input.GetMouseButtonDown(0) ){
			print("GetMouseButtonDown");
		}
	}
}