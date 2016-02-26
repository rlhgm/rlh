#pragma strict

var zapLight : Light;
var lightSize : float = 117;

function OnTriggerEnter2D (other:Collider2D){
	if(other.tag=="Player"){
		zapLight.spotAngle = lightSize;
	}
}
