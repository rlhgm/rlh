#pragma strict

var givenObjects : GameObject[];
var icon : GameObject;

function OnTriggerEnter2D (other:Collider2D){
	if(other.tag=="Player"){
		icon.SetActive(true);
	}
}

function OnTriggerStay2D(other:Collider2D){
	if(icon.activeSelf==true){ //zeby w jednej klatce nie przechodzil 2 razy
		if (Input.GetKeyDown ("w") && givenObjects != null){
			for (var obj: GameObject in givenObjects) {
				obj.SetActive(!obj.activeSelf);		
			}
			transform.GetChild(1).transform.localScale.y = -transform.GetChild(1).transform.localScale.y;
			icon.SetActive(false);
			var audio: AudioSource = GetComponent.<AudioSource>();
			audio.Play();	
		}
	}
}

function OnTriggerExit2D (other:Collider2D){
	if(other.tag=="Player"){
			icon.SetActive(false);
	}
}
