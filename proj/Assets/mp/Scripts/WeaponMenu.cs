using UnityEngine;
using System.Collections;

public class WeaponMenu : MonoBehaviour {

	public WeaponMenuItem itemKnife;
	public WeaponMenuItem itemGravityGun;

	// Use this for initialization
	void Awake () {
		itemKnife = transform.Find ("weaponicon_machete").GetComponent<WeaponMenuItem>();
		itemGravityGun = transform.Find ("weaponicon_gravitygun").GetComponent<WeaponMenuItem>();
		
		//print (itemKnife);
		//print (itemGravityGun);
		
		//itemKnife.setState (WeaponMenuItem.State.OFF);
		//itemGravityGun.setState (WeaponMenuItem.State.OFF);
		
		//itemGravityGun.setState (WeaponMenuItem.State.BLINK);
	}

	// Use this for initialization
	void Start () {
		//itemKnife = transform.Find ("weaponicon_machete").GetComponent<WeaponMenuItem>();
		//itemGravityGun = transform.Find ("weaponicon_gravitygun").GetComponent<WeaponMenuItem>();

		//print (itemKnife);
		//print (itemGravityGun);

		itemKnife.setState (WeaponMenuItem.State.OFF);
		itemGravityGun.setState (WeaponMenuItem.State.OFF);

		//itemGravityGun.setState (WeaponMenuItem.State.BLINK);
	}
	
	// Update is called once per frame
	void Update () {
		//print("weapon menu update");

	}
}
