#pragma strict

private var img : UI.Image;
private var rect : RectTransform;
private var t : float;

var activePos : float = 0;
var inactivePos : float;

var speed: float = 150;

function Start () {
	img = GetComponent(UI.Image);
	rect = GetComponent(RectTransform);
}

function Update () {
var step = speed * Time.deltaTime;

	if (img.color.a < .9){
		rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition, Vector2(inactivePos,rect.anchoredPosition.y), step);
	}
	else{
		rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition, Vector2(activePos,rect.anchoredPosition.y), step);
	}
}