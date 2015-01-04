﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BunnyHopMovement : Movement
{
	private bool applyFriction = false;

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
		if(applyFriction && !getJumpKeyPressed())
		{
			frictionTemp *= frictionMultiplier;
			velocity = new Vector3(frictionTemp.x, velocity.y, frictionTemp.y);
		}

		return velocity;
	}

	private Vector3 getJumpVelocity(float yVelocity)
	{
		bool onGround = checkGround();
		Vector3 jumpVelocity = Vector3.zero;

		//Calculate jump
		if(Time.time < lastJumpPress + jumpPressDuration && yVelocity < jumpForce && onGround)
		{
			lastJumpPress = -1f;
			GameInfo.info.playSound("jump");
			jumpVelocity = new Vector3(0f, jumpForce - yVelocity, 0f);
		}

		return jumpVelocity;
	}

	public override void FixedMoveUpdate()
	{
		if(checkGround()) { applyFriction = true; }
		else { applyFriction = false; }
	}
}
