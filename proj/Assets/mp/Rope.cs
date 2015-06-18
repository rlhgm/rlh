using UnityEngine;
using System.Collections;

public class Rope : MonoBehaviour {

	float force;
	float tm;

	public float CycleTime = 1.0f;
	public float PutOutFactor = 5.0f;
	public float MaxAngle = 45.0f;

	// Use this for initialization
	void Start () {
		force = 0.0f;
		tm = Mathf.PI * 0.5f;

		CycleTime = 1.0f;
		PutOutFactor = 5.0f;
		MaxAngle = 45.0f;
	}

	void addForce(float newForce){
		force += newForce;
		checkForce ();
	}

	void checkForce(){
		force = Mathf.Min (force, MaxAngle);
		force = Mathf.Max (force, 0.0f);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.O)) {
			addForce(-3.0f);
		}

		if (Input.GetKeyDown (KeyCode.P)) {
			addForce(3.0f);
		}

		if (force == 0.0f)
			return;

		float mkv = (tm * Mathf.PI) / CycleTime;
		float ckv = Mathf.Cos (mkv);
		ckv *= force;
		tm += Time.deltaTime;

		force -= (PutOutFactor * Time.deltaTime);
		checkForce ();

		Vector3 oldRotation = transform.rotation.eulerAngles;
		oldRotation.z = ckv;
		Quaternion newRotation = new Quaternion();
		newRotation.eulerAngles = oldRotation;
		transform.rotation = newRotation;
	}
}
