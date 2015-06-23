using UnityEngine;
using System.Collections;

public class ncc3 : MonoBehaviour {

	public KeyCode keyLeft = KeyCode.LeftArrow;
	public KeyCode keyRight = KeyCode.RightArrow;
	public KeyCode keyRun = KeyCode.LeftShift;
	public KeyCode keyJump = KeyCode.Space;
	public KeyCode keyUp = KeyCode.UpArrow;
	public KeyCode keyDown = KeyCode.DownArrow;

	private Animator animator;


	void Awake(){
		animator = GetComponent<Animator>();
	}

	void Start () {
		//animator.SetTrigger("idle");
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Escape))
			Application.Quit();

		if (Input.GetKeyDown(keyUp)) {
			//print ("GetKeyDown(keyLeft)");
			animator.SetBool("idle",true);
		}

		if (Input.GetKeyDown(keyLeft)) {
			//print ("GetKeyDown(keyLeft)");
			//animator.SetTrigger("walk");
			animator.SetBool("walk",true);
		}

		if (Input.GetKeyUp(keyLeft)) {
			//print ("GetKeyUp(keyLeft)");
			//animator.SetTrigger("idle");
			animator.SetBool("walk",false);
		}

		if (Input.GetKeyDown(keyRight)) {
			//print ("GetKeyDown(keyRight)");
			//animator.SetTrigger("jump");
			animator.SetBool("jump",true);
		}
		
		if (Input.GetKeyUp(keyRight)) {
			//print ("GetKeyUp(keyRight)");
			//animator.SetTrigger("idle");
			animator.SetBool("walk",true);
		}
	}


}
