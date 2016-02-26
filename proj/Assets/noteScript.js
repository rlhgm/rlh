#pragma strict

var note : GameObject;

var icon : GameObject;
var ZapScript : MonoBehaviour;
private var isReading : boolean = false;
private var canRead : boolean = false;

function OnTriggerEnter2D (other:Collider2D){
	if(other.tag=="Player"){
		icon.SetActive(true);
	}
}

function OnTriggerStay2D (other:Collider2D){
	if(other.tag=="Player"){
		canRead=true;
	}
}

function Update(){ //bo inaczej w jednej klatce getkeydown pare razy sie odpalal
	if (Input.GetKeyDown ("w") && canRead){
		isReading = !isReading;
		note.SetActive(isReading);
		ZapScript.enabled = !isReading;
	}
}

function OnTriggerExit2D (other:Collider2D){
	if(other.tag=="Player"){
			icon.SetActive(false);
			canRead = false;
	}
}
