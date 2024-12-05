// PlayerMovement.cs - Simple first person movement script.
// -----------------------------------------------------------------------------
// Copyright (c) 2018-2023 Niall Moody
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// -----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using AK;
using UnityEngine;

/// Simple first person movement script, included so we don't need to include
/// any Standard Assets (they're far heavier than we need).
public class PlayerMovement : MonoBehaviour
{
	/// Used to implement mouselook on the vertical axis.
	public Camera playerCamera;

	/// The speed the player moves at.
	[Range(0.1f, 20.0f)]
	public float moveSpeed = 10.0f;
	/// The speed at which footstep sounds are triggered.
	[Range(0.01f, 1.0f)]
	public float footstepRate = 0.3f;
	///	The amount of force to apply when the player jumps.
	[Range(1.0f, 32.0f)]
	public float jumpForce = 10.0f;
	/// How much gravity to apply to the player.
	[Range(0.0f, 64.0f)]
	public float gravity = 32.0f;

	/// Used to disable collisions when the player moves.
	public bool noclip = false;

	

	public AK.Wwise.Event footstepsClip;
	private bool isPlaying = true;
	private float footstepLenght = 0.5f;
	private float nextFootstepLenght = 0f;

	public AK.Wwise.Event playerJumpClip;


	/// Used to set the player's rotation around the y-axis.
	private float playerRotation;
	/// Used to implement mouselook on the vertical axis.
	private float viewY;

	///	Used to let the player jump.
	private float jumpVelocity = 0.0f;

	///	Used to determine when to trigger footstep sounds.
	private bool walking = false;
	///	Used to determine when to trigger footstep sounds.
	private float walkCount = 0.0f;

	///	Used to ensure we play the Jump Land sound when we hit the ground.
	private bool inAir = false;
	///	Used to ensure we don't trigger a false Jump Land when the game starts.
	private int inAirCount = 16;

	/// We use this to hide the mouse cursor.
	void Start()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		
	}
	
	/// This is where we move the Player object and Camera.
	public void Update()
	{
		float speed = moveSpeed;

		//Get our current WASD speed.
		Vector3 strafe = new Vector3(Input.GetAxis("Horizontal") * speed, 0.0f, 0.0f);
		float forwardSpeed = Input.GetAxis("Vertical") * speed;

		if(Input.GetButtonDown("Noclip"))
			noclip = !noclip;

		if(((Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.0f) ||
			(Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.0f)))
		{
            walking = true;
            if (Time.time >= nextFootstepLenght && !inAir)
            {
                footstepsClip.Post(gameObject);
				
				nextFootstepLenght = Time.time + footstepLenght;
            }
          

	
		}
		else
		{
			walking = false;
            isPlaying = false;
			nextFootstepLenght = 0f;
            

			walkCount = footstepRate;
		}

		//Get our current mouse/camera rotation.
		playerRotation = Input.GetAxis("Mouse X") * 6.0f;

		playerCamera.transform.Rotate(new Vector3(viewY, 0.0f, 0.0f));

		viewY += (Input.GetAxis("Mouse Y") * 4.0f);

		//Don't let the player rotate the camera more than 90 degrees on the
		//y-axis.
		viewY = Mathf.Clamp(viewY, -90.0f, 90.0f);

		//Rotate the camera up/down.
		playerCamera.transform.Rotate(new Vector3(-viewY, 0.0f, 0.0f));

		//Rotate player (clamped so they can't move so fast they make themselves
		//sick).
		Mathf.Clamp(playerRotation, -5.0f, 5.0f);
		transform.Rotate(0.0f, playerRotation, 0.0f);

		//Jump player.
		CharacterController controller = GetComponent<CharacterController>();
		Vector3 jumpVector = Vector3.zero;

		if(!controller.isGrounded)
			inAir = true;
		else
		{
			if (inAir && (inAirCount < 1))
				playerJumpClip.Post(gameObject);
			inAir = false;
		}

		if(inAirCount > 0)
			--inAirCount;

		if(walking && !inAir)
		{
			walkCount += Time.deltaTime * (speed/10.0f);

			if(walkCount > footstepRate)
			{
				

				walkCount = 0.0f;
			}
		}

		//If the player is holding the jump button down, AND they're not yet
		//jumping AND on the ground, OR they are jumping but they've not reached
		//the top of the jump, increase their jumpAmount and move them
		//accordingly on the y-axis.
		if(Input.GetButton("Jump"))
		{
			if((jumpVelocity <= 0.0f) && controller.isGrounded)
			{
				
				
				jumpVelocity = jumpForce;
			}
		}

		//Move player.
		Vector3 moveDirection = Vector3.zero;

		//Set the player's direction based on the direction of the mouse and
		//which WASD keys they're pressing.
		moveDirection = transform.rotation * ((Vector3.forward * forwardSpeed) + strafe);

		if (noclip)
		{
			Vector3 pos = transform.localPosition;
			pos += moveDirection * Time.deltaTime;

			transform.localPosition = pos;
		}
		else
		{
			moveDirection.y = jumpVelocity;

			//Apply gravity.
			jumpVelocity -= gravity * Time.deltaTime;
			if(controller.isGrounded && (jumpVelocity < 1.0f))
				jumpVelocity = -1.0f;

			//Finally, apply the updated direction to the player's Controller (this
			//will figure out any collisions with the ground, other objects, etc.).
			controller.Move(moveDirection * Time.deltaTime);
		}

		//Quit if the player hits escape (in a build).
		if(Input.GetKey("escape"))
			Application.Quit();
	}
}
