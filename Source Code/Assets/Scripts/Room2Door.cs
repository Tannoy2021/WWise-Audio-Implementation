using System.Collections;
using System.Collections.Generic;
using AK;
using UnityEngine;

public class Room2Door : MonoBehaviour
{
	/// Used to move the door.
	public Transform doorTransform;

	public float doorTime = 0.5f;

	public AK.Wwise.Event doorOpen;
	public AK.Wwise.Event doorClose;

    /// True if the door should be open.
    private bool open = false;
	
	/// Update is called once per frame
	void Update()
	{
		if(open && (doorTransform.position.y > -3.1f))
		{
			doorTransform.position -= Vector3.up * 6.1f * (1.0f/doorTime) * Time.deltaTime;
		}
		else if(!open && (doorTransform.position.y < 3.0f))
		{
			doorTransform.position += Vector3.up * 6.1f * (1.0f / doorTime) * Time.deltaTime;
		}
	}

	/// Tells the door to open.
	public void OpenDoor()
	{
		open = true;

		if (doorOpen != null) 
		{
			doorOpen.Post(gameObject);
		}
	}

	/// Tells the door to close.
	public void CloseDoor()
	{
		open = false;
		
		if (doorClose != null) 
		{
			doorClose.Post(gameObject);
		}
	}
}
