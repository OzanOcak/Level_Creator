using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	
	private bool _isFacingRight;
	private CharecterController2D _controller;
	private float _normalizedHorizontalSpeed=0;//private

	public float MaxSpeed;
	public float SpeedAccelaritionOnGround=10f;
	public float SpeedAccelaritionOnAir=5f;

		public void Start()
	{
		_controller=GetComponent<CharecterController2D>();
		_isFacingRight=transform.localScale.x>0;
	}
	public void Update()
	{
		HandleInput();

		var movementFactor= _controller.State.IsGrounded ? SpeedAccelaritionOnGround : SpeedAccelaritionOnAir;
		_controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x,_normalizedHorizontalSpeed*MaxSpeed,movementFactor*Time.deltaTime));
	}
	private void HandleInput()//private
	{
		if(Input.GetKey(KeyCode.A))
		{
			_normalizedHorizontalSpeed=-1;
			if (!_isFacingRight)
				Flip();
		}
		else if(Input.GetKey(KeyCode.D))
		{
			_normalizedHorizontalSpeed=1;
			if (_isFacingRight)
				Flip();
		}
		else
			_normalizedHorizontalSpeed=0;

		if(_controller.CanJump && Input.GetKey(KeyCode.Space))
		{
			_controller.Jump();
		}

	}
	private void Flip()
	{
		transform.localScale=new Vector3(-transform.localScale.x,transform.localScale.y,transform.localScale.z);
		_isFacingRight=transform.localScale.x>0;
	}

}
