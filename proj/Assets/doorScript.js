#pragma strict

var destination : Transform;
var Zap : Transform;
var icon : GameObject;

function OnTriggerEnter2D (other:Collider2D){
	if(other.tag=="Player"){
		yield WaitForSeconds (0.05); //zeby w jednej klatce nie przechodzil 2 razy
		icon.SetActive(true);
	}
}

function OnTriggerStay2D(other:Collider2D){
	if(icon.activeSelf==true && other.tag=="Player"){ //zeby w jednej klatce nie przechodzil 2 razy
		if (Input.GetKeyDown ("w")){
			Zap.position = destination.position;
		}
	}
}

function OnTriggerExit2D (other:Collider2D){
	if(other.tag=="Player"){
			icon.gameObject.SetActive(false);
	}
}
