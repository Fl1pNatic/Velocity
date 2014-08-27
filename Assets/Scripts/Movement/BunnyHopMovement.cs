﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BunnyHopMovement : Movement
{
	private List<int> collidingObjects = new List<int>();
	private bool applyFriction = false;
	private bool collidedLastFrame = false;
	private int frameCounter = 0;
	private Vector3 acceleratorForce = Vector3.zero;
	private Vector3 jumpPadForce = Vector3.zero;
	private bool usePadX, usePadY, usePadZ;

	public override Vector3 calculateAdditionalVelocity(Vector2 input)
	{
		//Get input and make it a vector
		Vector3 camRotation = new Vector3(0f, camObj.transform.rotation.eulerAngles.y, camObj.transform.rotation.eulerAngles.z);
		Vector3 inputVelocity = Quaternion.Euler(camRotation) * new Vector3(input.x * speed, 0f, input.y * speed);

		//Ignore vertical component of rotated input
		Vector3 alignedInputVelocity = new Vector3(inputVelocity.x, 0f, inputVelocity.z) * Time.deltaTime * airSpeed;
		
		//Get current velocity
		Vector3 currentVelocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);

		//How close the current speed to max velocity is (0 = not moving, 1 = at max speed)
		float max = 1 - (currentVelocity.magnitude / maxSpeed);

		//How perpendicular the input to the current velocity is (0 = 90°)
		float velocityDot = Vector3.Dot(currentVelocity, alignedInputVelocity);

		//Scale the input to the max speed
		Vector3 modifiedVelocity = alignedInputVelocity * max;

		//The more perpendicular the input is, the more the input velocity will be applied
		Vector3 correctVelocity = Vector3.Lerp(alignedInputVelocity, modifiedVelocity, velocityDot);

		//Apply accelerator
		correctVelocity += acceleratorForce;
		acceleratorForce = Vector3.zero;

		//Apply jump
		correctVelocity += getJumpVelocity(rigidbody.velocity.y);

		//Return
		return correctVelocity;
	}

	public override Vector3 overrideVelocity(Vector3 input)
	{
		Vector3 velocity = input;
		Vector2 frictionTemp = new Vector2(input.x, input.z);

		//Friction
		if(applyFriction)
		{
			frictionTemp *= frictionMultiplier;
			velocity = new Vector3(frictionTemp.x, velocity.y, frictionTemp.y);
		}

		//Apply jumppad
		if(jumpPadForce != Vector3.zero)
		{
			float tempX = velocity.x, tempY = velocity.y, tempZ = velocity.z;
			if(usePadX) { tempX = jumpPadForce.x; }
			if(usePadY) { tempY = jumpPadForce.y; }
			if(usePadZ) { tempZ = jumpPadForce.z; }
			velocity = new Vector3(tempX, tempY, tempZ);
			jumpPadForce = Vector3.zero;
		}

		return velocity;
	}

	public void clearCollisionList()
	{
		collidingObjects.Clear();
	}

	private Vector3 getJumpVelocity(float yVelocity)
	{
		bool onGround = checkGround();
		Vector3 jumpVelocity = Vector3.zero;

		//Calculate jump
		if(Time.time < lastJumpPress + jumpPressDuration && yVelocity < jumpForce && onGround)
		{
			lastJumpPress = -1f;
			frameCounter = 0;
			GameInfo.info.playSound("jump");
			jumpVelocity = collisionAverageNormal * (jumpForce - yVelocity);
		}

		return jumpVelocity;
	}

	public override void FixedMoveUpdate()
	{
		if(collidedLastFrame)
		{
			frameCounter++;
		}
		else
		{
			frameCounter = 0;
		}

		if(collidingObjects.Count == 0)
		{
			collidedLastFrame = false;
		}
		else
		{
			collidedLastFrame = true;
		}

		if(frameCounter > 3)
		{
			applyFriction = true;
		}
		else
		{
			applyFriction = false;
		}
	}

	void OnCollisionEnter(Collision col)
	{
		foreach(ContactPoint contact in col)
		{
			if(contact.normal.y > 0.7f && !collidingObjects.Contains(contact.otherCollider.gameObject.GetInstanceID()))
			{
				collidingObjects.Add(contact.otherCollider.gameObject.GetInstanceID());
			}
		}
	}

	void OnCollisionExit(Collision col)
	{
		foreach(ContactPoint contact in col)
		{
			if(collidingObjects.Contains(contact.otherCollider.gameObject.GetInstanceID()))
			{
				collidingObjects.Remove(contact.otherCollider.gameObject.GetInstanceID());
			}
		}
	}

	void OnCollisionStay(Collision col)
	{
		foreach(ContactPoint contact in col)
		{
			if(contact.otherCollider.gameObject.tag.Equals("Accelerator"))
			{
				acceleratorForce = contact.otherCollider.gameObject.GetComponent<Accelerator>().accelerationVector;
			}
		}
	}

	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag.Equals("JumpPad"))
		{
			JumpPad pad = other.gameObject.GetComponent<JumpPad>();
			usePadX = pad.useX;
			usePadY = pad.useY;
			usePadZ = pad.useZ;
			jumpPadForce = pad.jumpVector;
		}
	}
}
