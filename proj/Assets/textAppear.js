#pragma strict

var t : float = 5;

private var textObj : UI.Text;
private var child : GameObject;

function Start(){
	textObj = GetComponent(UI.Text);
	child = transform.GetChild(0).gameObject;
}
function Update () {
	t -= Time.deltaTime;
 
	if (t <= 0){
		textObj.text = "";
		child.SetActive(false);
	}
	else if(!child.activeSelf){
		child.SetActive(true);		
	}
}
