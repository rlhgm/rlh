#pragma strict

var searchType : String;
var icon : GameObject;

var openObjects : GameObject[];
var note : GameObject;
var ZapScript : MonoBehaviour;
var textObj : UI.Text;
var key : GameObject;

private var isReading : boolean = false;

function OnTriggerEnter2D (other:Collider2D){
	if(other.tag=="Player"){
		icon.SetActive(true);
	}
}

function OnTriggerStay2D (other:Collider2D){
	if(other.tag=="Player"){	
	
		if (Input.GetKeyDown ("w") && icon.activeSelf){
			if (searchType == "empty"){
				textObj.gameObject.GetComponent(textAppear).t = 3;
				textObj.text = "it's empty";
				//Destroy(gameObject);
				icon.SetActive(false);
			}	
			else if (searchType == "checkpoint"){
				if (openObjects != null){
					for (var obj: GameObject in openObjects) {
						obj.SetActive(!obj.activeSelf);		
					}
				}
				textObj.gameObject.GetComponent(textAppear).t = 3;
				textObj.text = "i've found an ankh cross";
				ZapScript.gameObject.GetComponent(placeCheckpoints).checkpointsAmount++;
				icon.SetActive(false);
				searchType = "empty";
			
			}
			
			if (searchType == "checkdrop"){
				if (openObjects != null){
					for (var obj: GameObject in openObjects) {
						obj.SetActive(!obj.activeSelf);		
					}
				}
				textObj.gameObject.GetComponent(textAppear).t = 3;
				textObj.text = "i've found an ankh cross";
				ZapScript.gameObject.GetComponent(placeCheckpoints).checkpointsAmount++;
				icon.SetActive(false);
				Destroy(gameObject);
			
			}			
			
			else if (searchType == "note"){
				isReading = !isReading;
				note.SetActive(isReading);
				ZapScript.enabled = !isReading;
			}
			
			else if (searchType == "open1"){
				for (var obj: GameObject in openObjects) {
					obj.SetActive(!obj.activeSelf);		
				}
				textObj.gameObject.GetComponent(textAppear).t = 3;		
				textObj.text = "it's empty";
				searchType = "empty";
				//Destroy(gameObject);
				icon.SetActive(false);
			}			
			
			else if (searchType == "key"){
				for (var obj: GameObject in openObjects) {
					obj.SetActive(!obj.activeSelf);		
				}		
				textObj.gameObject.GetComponent(textAppear).t = 3;
				textObj.text = "i've found a key";
				Destroy(gameObject);
				icon.SetActive(false);
			}
			
			else if (searchType == "lockeddoor"){
				
				if (!key.activeSelf){
					textObj.gameObject.GetComponent(textAppear).t = 3;
					textObj.text = "i need a key";				
				}	
				
				else if (key.activeSelf){
					for (var obj: GameObject in openObjects) {
						obj.SetActive(!obj.activeSelf);		
					}
					key.SetActive(false);
					Destroy(gameObject);
				}		

				icon.SetActive(false);
			}
			
			else if (searchType == "switchdoor"){
				
					textObj.gameObject.GetComponent(textAppear).t = 3;
					textObj.text = "it's a stone with a lever sign";
					icon.SetActive(false);
			}			
																									
			
		
			
			
		}
	}
}

function OnTriggerExit2D (other:Collider2D){
	if(other.tag=="Player"){
			icon.SetActive(false);
	}
}
