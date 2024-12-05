using System.Collections;
using System.Collections.Generic;
using AK;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
	/// The HarpoonGun prefab we're going to give to the player.
	public GameObject gunPrefab;
	/// The Harpoon prefab we're going to give to the player.
	public GameObject harpoonPrefab;
	/// The Harpoon projectile prefab we're going to give to the player.
	public GameObject harpoonProjectilePrefab;
	/// The rope prefab we're going to give to the player.
	public GameObject ropePrefab;

	public AK.Wwise.Event WeaponPickup;


	void OnTriggerEnter(Collider other)
	{
		if(other.name == "Player")
		{
			GameObject camera = other.transform.Find("Main Camera").gameObject;

			//Add Harpoon Gun.
			GameObject gun = Instantiate(gunPrefab, camera.transform);
			
			gun.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
			gun.transform.localPosition = new Vector3(0.25f, -0.33f, 0.0f);

			//Add Harpoon.
			GameObject harpoon = Instantiate(harpoonPrefab, camera.transform);
			
			harpoon.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
			harpoon.transform.localPosition = new Vector3(0.25f, -0.33f, 0.9f);
			

			//Add the GunController Component to our Player.
			GunController controller = other.gameObject.AddComponent<GunController>() as GunController;

			controller.unfiredHarpoon = harpoon;
			controller.harpoonPrefab = harpoonProjectilePrefab;
			controller.ropePrefab = ropePrefab;


			if(WeaponPickup != null)
			{
				WeaponPickup.Post(gameObject);
			}

			Destroy(gameObject);
		}
	}
}
