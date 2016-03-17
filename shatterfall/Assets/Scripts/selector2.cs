﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class selector2 : MonoBehaviour {
	
	public static int option;
	public List<GameObject> uiChoices;
	
	// Use this for initialization
	void Start () {
		option = 0;
		transform.localScale = uiChoices [0].transform.localScale * 9f;
	}
	
	bool ready;
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetAxis("MoveVertical1") == 0)
			ready = true;
		
		if (Input.GetKeyDown (KeyCode.UpArrow) || Input.GetAxis("MoveVertical1") > 0 && ready) {
			ready = false;
			if (option == 0) {
				option = uiChoices.Count - 1;
			} else {
				option--;
			}
		}
		if (Input.GetKeyDown (KeyCode.DownArrow) || Input.GetAxis("MoveVertical1") < 0 && ready) {
			ready = false;
			if (option == uiChoices.Count - 1) {
				option = 0;
			} else {
				option++;
			}
		}
		
		transform.position = uiChoices[option].transform.position + new Vector3(-uiChoices[option].transform.localScale.x * 0.55f, uiChoices[option].transform.localScale.x * 0.2f, 0);
		
		if (Input.GetKeyDown (KeyCode.Space) || Input.GetAxis("Jump1") > 0) {
			if(option == 0){
				Application.LoadLevel ("main");
			}
			else {
				Application.LoadLevel ("MainMenu");
			}
		}
	}
}