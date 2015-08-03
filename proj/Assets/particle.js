#pragma strict

var particles : GameObject;
//var object : GameObject;

function Start () {

}

function OnTriggerEnter2D(coll: Collider2D) {
print("lol");
	if (coll.gameObject.tag == "Player"){
		//Instantiate(object,transform.position,Quaternion.EulerAngles(270,0,0));
		Instantiate(particles,Vector3(coll.gameObject.transform.position.x,transform.position.y),Quaternion.Euler(270,0,0));
		var audio: AudioSource = GetComponent.<AudioSource>();
		audio.Play();
	}
}