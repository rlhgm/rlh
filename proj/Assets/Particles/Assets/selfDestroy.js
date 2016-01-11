#pragma strict

var timeToDestroy : float = 2.0;

function Start () {
	Destroy (gameObject, timeToDestroy);
}