using System.Collections;
using System.Collections.Generic;
using AK;
using UnityEngine;

public class BoxSoundTrigger : MonoBehaviour
{
	public AK.Wwise.Event pickupSound;
	public AK.Wwise.Event dropSound;

	public Rigidbody boxRigidbody;


    /// Used to trigger a sound when the box collides with something after falling.
    private bool falling = false;
	private bool isMoving = false;


	private void Start()
	{
		boxRigidbody = GetComponent<Rigidbody>();
	}

    private void Update()
    {
        if (boxRigidbody)
        {
            if (boxRigidbody.velocity.magnitude > 0.03f)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }

        if (isMoving && falling && dropSound != null)
        {
            dropSound.Post(gameObject);
        }

        falling = false;
    }

    ///	Called when the box is picked up.
    public void PickupBox()
	{
		if(pickupSound!= null)
		{
			pickupSound.Post(gameObject);
		}
	}

	///	Called when the box is picked up.
	public void DropBox()
	{
        if (dropSound != null)
        {
            dropSound.Post(gameObject);
        }
    }

	/// Used to set our falling variable.
	void OnCollisionStay(Collision collision)
	{
		falling = false;
	}

	/// Used to set our falling variable.
	void OnCollisionExit(Collision collision)
	{
		falling = true;
	}
}
