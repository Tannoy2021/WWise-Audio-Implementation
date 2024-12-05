using System.Collections;
using System.Collections.Generic;
using AK;
using UnityEngine;

/// Class governing the motion of the harpoon projectile.
public class HarpoonProjectile : MonoBehaviour
{
	/// The GunController that fired this projectile.
	public GunController controller;
	/// The BoxPicker script we'll use to let the Player carry any received boxes.
	public BoxPicker boxPicker;

	/// Transform of the object we use to anchor the rope.
	public Transform anchorTransform;

	public AK.Wwise.Event HarpoonFired;
	public AK.Wwise.Event HarpoonReturns;

	/// True if the harpoon is firing away from the gun; false if it's returning to the gun.
	private bool firing = true;
	///	The speed to move the harpoon at.
	private float speed = 50.0f;

	///	True if we've triggered the return sound.
	private bool triggeredReturnSound = false;

	///	The direction to fire the harpoon in.
	private Vector3 direction;

	/// The key cube we're grabbing if we've grabbed one.
	private GameObject keyCube;
	///	The rigidbody of the key cube if we've grabbed one.
	private Rigidbody keyCubeBody;
	/// The key cube's original parent transform.
	private Transform originalCubeParent;

	/// Used to set our direction.
	void Start()
	{
		direction = transform.forward;

		if(HarpoonFired!= null) 
		{
			HarpoonFired.Post(gameObject);
		}
	}
	
	/// Update is called once per frame
	void Update()
	{
		if(firing)
		{
			//If we're too far from the player, return.
			if(Vector3.Distance(controller.gameObject.transform.position, transform.position) > 40.0f)
			{
				firing = false;

				if(!triggeredReturnSound)
				{
					//Trigger harpoon return sound/event?
					HarpoonReturns.Post(gameObject);
					triggeredReturnSound = true;
				}
			}

			//Move the projectile forward.
			transform.localPosition += direction * -speed * Time.deltaTime;
		}
		else
		{
			//We need to move towards the gun.
			transform.localPosition = Vector3.MoveTowards(transform.position,
														  anchorTransform.position,
														  speed * Time.deltaTime);

			//Update the cube if we have one.
			if (keyCube != null)
			{
				keyCubeBody.position = transform.position;
			}

			if (Vector3.Distance(anchorTransform.position, transform.position) < 0.001f)
			{
				//We've returned to the gun.
				controller.ProjectileReturned();

				

				//Update the cube if we have one.
				if(keyCube != null)
				{
					keyCubeBody.isKinematic = false;
					keyCubeBody.GetComponent<BoxCollider>().size = Vector3.one;

					keyCube.transform.parent = originalCubeParent;
					boxPicker.PickupBox(keyCube.GetComponent<Rigidbody>());

					controller.droppingBox = true;
				}
			}
		}

		//Rotate so we're always pointing away from the gun.
		transform.LookAt(transform.position);
	}

	/// Used to grab key cubes or return to the gun if we've collided with a wall.
	void OnTriggerEnter(Collider other)
	{
		if(!other.isTrigger)
		{
			if(firing)
			{
				if(other.name == "Key Cube")
				{
					//Grab cube.
					keyCube = other.gameObject;

					originalCubeParent = keyCube.transform.parent;
					keyCube.transform.parent = transform;

					keyCubeBody = keyCube.GetComponent<Rigidbody>();
					keyCubeBody.isKinematic = true;
					keyCubeBody.GetComponent<BoxCollider>().size = Vector3.zero;

                    if (HarpoonFired != null)
                    {
                        HarpoonFired.Post(gameObject);
                    }
                }
				else
				{
                    if (HarpoonFired != null)
                    {
                        HarpoonFired.Post(gameObject);
                    }
                }

				firing = false;

				
			}
		}
	}

	/// Called from GunController to return the harpoon when the player releases the mouse button.
	public void ReturnProjectile()
	{
		firing = false;

		if(!triggeredReturnSound)
		{
			
            if (HarpoonReturns != null)
            {
                HarpoonReturns.Post(gameObject);
            }

            triggeredReturnSound = true;
		}
	}
}
