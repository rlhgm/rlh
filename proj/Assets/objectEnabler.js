#pragma strict

var givenObjects : GameObject[];
var colliderTag : String = "Player";

function OnTriggerEnter2D (other:Collider2D){
	if(other.tag==colliderTag && givenObjects != null){

		for (var obj: GameObject in givenObjects) {
			obj.SetActive(!obj.activeSelf);		
		}
		Destroy (gameObject);
	}
}
