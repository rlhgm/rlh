 private var animator : Animator;
 private var originalSpeed : float;
 
 function Start () 
 { 
     animator = GetComponent(Animator);     
     originalSpeed = animator.speed;
     animator.speed = Random.Range(0,2000);     
     yield WaitForSeconds(0.1);     
     animator.speed = originalSpeed; 
 }