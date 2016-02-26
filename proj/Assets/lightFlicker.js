var duration : float = 1.0;
var originalRange : float;


var lt: Light;


function Start() {
	lt = GetComponent.<Light>();
	originalRange = lt.range;
}

	
function Update() {
	var amplitude : float = Mathf.PingPong(Time.time, duration);
	
	amplitude = amplitude / duration * 0.95 + 0.95;

	lt.range = originalRange * amplitude;
}