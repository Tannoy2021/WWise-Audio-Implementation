using System.Collections;
using System.Collections.Generic;
using AK;
using UnityEngine;

public class Room3Door : MonoBehaviour
{
	/// Used to move the door.
	public Transform doorTransform;

	public float doorTime = 0.5f;

    public AK.Wwise.Event doorOpen;
    public AK.Wwise.Event doorClose;

    /// True if the door should be open.
    private bool[] open;

	/// Set open correctly.
	void Start()
	{
		open = new bool[] {false, false};
	}
	
	/// Update is called once per frame
	void Update()
	{
		if((open[0] && open[1]) && (doorTransform.position.y > -2.1f))
		{
			doorTransform.position -= Vector3.up * 4.1f * (1.0f / doorTime) * Time.deltaTime;
		}
		else if((!open[0] || !open[1]) && (doorTransform.position.y < 2.0f))
		{
			doorTransform.position += Vector3.up * 4.1f * (1.0f / doorTime) * Time.deltaTime;
		}
	}

	/// Tells the door to open.
	public void Opendoor0()
	{
		open[0] = true;

		if(open[1])
		{
            if (doorOpen != null)
            {
                doorOpen.Post(gameObject);
            }
        }
	}

	/// Tells the door to close.
	public void Closedoor0()
	{
		if(open[1])
		{
            if (doorClose != null)
            {
                doorClose.Post(gameObject);
            }
        }

		open[0] = false;
	}

	/// Tells the door to open.
	public void Opendoor1()
	{
		open[1] = true;

		if(open[0])
		{
            if (doorOpen != null)
            {
                doorOpen.Post(gameObject);
            }
        }
	}

	/// Tells the door to close.
	public void Closedoor1(){
		if(open[0])
		{
            if (doorClose != null)
            {
                doorClose.Post(gameObject);
            }
        }

		open[1] = false;
	}
}
