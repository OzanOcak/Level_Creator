using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ControllerParameter2D 
{
	public enum JumpBehaviour
	{
		CanJumpOnGround,
		CanJumpAnyWhere,
		CantJump
	}
	public Vector2 MaxVelocity=new Vector2(float.MaxValue,float.MaxValue);

	[RangeAttribute(0,90)]
	public float SlopeLimit=30;

	public float Gravity=-25f;

	public JumpBehaviour JumpRestrictions;

	public float JumpFrequency=.25f;

	public float JumpMagnitude=12;
}

