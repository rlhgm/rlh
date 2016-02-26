#pragma strict
private var childText : UI.Text;
private var thisText : UI.Text;

var checkScript : GameObject;

function Start () {
	childText = transform.GetChild(0).GetComponent(UI.Text);
	thisText = GetComponent(UI.Text);
}

function Update () {
	thisText.text = ""+ checkScript.gameObject.GetComponent(placeCheckpoints).checkpointsAmount;
	childText.text = thisText.text;
}