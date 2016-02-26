#pragma strict

var checkpoint : GameObject;
var textObj : UI.Text;

var checkpointsAmount : int = 10;

var checkObj : GameObject;

var inCheck : boolean = false;

function Start(){
	textObj.text = ""+ checkpointsAmount;
}

function Update () {
	if(Input.GetMouseButtonDown(2)){
		if (!inCheck  && checkpointsAmount>0){
			Instantiate(checkpoint, transform.position, transform.rotation);
			checkpointsAmount--;
			textObj.text = ""+ checkpointsAmount;
		}
		else if (inCheck){
			if(checkObj!=null)
			Destroy(checkObj);
			
			checkObj = null;
			checkpointsAmount++;
			inCheck = false;
			textObj.text = ""+ checkpointsAmount;
		}
	}
}

function OnTriggerEnter2D (other:Collider2D){
	if(other.tag=="CheckPoint"){
		inCheck = true;
		checkObj = other.gameObject.transform.parent.gameObject;
	}
}

function OnTriggerExit2D (other:Collider2D){
	if(other.tag=="CheckPoint"){
		inCheck = false;
	}
}