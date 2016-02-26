#pragma strict

var intensity : float = 20;

var lt: Light;

function Start () {
	lt = GetComponent.<Light>();
}

function Update () {
	lt.intensity = intensity;
}