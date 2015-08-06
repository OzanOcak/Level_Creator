using UnityEngine;
using System.Collections;

public class CharecterController2D : MonoBehaviour
{
	private const float SkinWidth=.02f;
	private const int TotalHorizontalRay=8;
	private const int TotalVerticalRay=4;

	private static readonly float SlopeLimitTangent=Mathf.Tan(75f*Mathf.Deg2Rad);

	public LayerMask PlatformMask;
	public ControllerParameter2D DefaultParameters;

	public ControllerState2D State{get;private set;}
	public Vector2 Velocity{get{return _velocity;}}
	public bool CanJump
	{
		get
		{
			if(Parameters.JumpRestrictions==ControllerParameter2D.JumpBehaviour.CanJumpAnyWhere)
			    return _jumpIn <= 0;
			if(Parameters.JumpRestrictions==ControllerParameter2D.JumpBehaviour.CanJumpOnGround)
				return State.IsGrounded;

			return false;
		}
	}

	public bool HandleCollisions{get;set;}
	public ControllerParameter2D Parameters{ get {return _overrideParameters ?? DefaultParameters;}}
	public GameObject StandingOn{get;private set;}

	private Vector2 _velocity; //I cant manipulate properties but i can modify on fields
	private Transform _transform;
	private Vector3 _localScale;
	private BoxCollider2D _boxCollider;
	private ControllerParameter2D _overrideParameters;
	private float _jumpIn;

	private Vector3
		_raycastTopLeft,
		_raycastBottomRight,
		_raycastBottomLeft;

	private float
		_verticalDistanceBetweenRays,
		_horizontalDistanceBetweenRays;

	public void Awake()
	{
		HandleCollisions=true;
		State=new ControllerState2D();
		_transform=transform;
		_localScale=transform.localScale;
		_boxCollider=GetComponent<BoxCollider2D>();

		var colliderWidth=_boxCollider.size.x * Mathf.Abs (transform.localScale.x)-(2*SkinWidth);
		_horizontalDistanceBetweenRays=colliderWidth/(TotalVerticalRay-1);

		var colliderHeight= _boxCollider.size.y* Mathf.Abs(transform.localScale.y)-(2*SkinWidth);
		_verticalDistanceBetweenRays=colliderHeight/(TotalVerticalRay-1);
	}

	public void AddForce(Vector2 force)
	{
		_velocity=force;
	}

	public void SetForce(Vector2 force)
	{
		_velocity+=force;
	}

	public void SetHorizontalForce(float x)
	{
		_velocity.x=x;
	}

	public void SetVerticalForce(float y)
	{
		_velocity.y=y;
	}

	public void Jump()
	{
		AddForce(new Vector2(0,Parameters.JumpMagnitude));
		_jumpIn=Parameters.JumpFrequency;
	}

	public void LateUpdate()
	{
		_jumpIn -= Time.deltaTime;

		_velocity.y+=Parameters.Gravity*Time.deltaTime;
		Move (Velocity*Time.deltaTime);
	}

	private void Move(Vector2 deltaMovement)
	{
		var wasGrounded=State.IsCollidingBelow;
		State.Reset();

		if(HandleCollisions)
		{
			HandlePlatforms ();
			CalculateRayOrigins();

			if (deltaMovement.y>0 && wasGrounded )
				HandleVerticalSlope(ref deltaMovement);

			if(Mathf.Abs(deltaMovement.x)>.001f)
				MoveHorizontally(ref deltaMovement);

			MoveVertically(ref deltaMovement);
		}
		_transform.Translate(deltaMovement,Space.World);

		//TODO : Addittional moving platform code
		if(Time.deltaTime>0)
			_velocity=deltaMovement/Time.deltaTime;

		_velocity.x=Mathf.Min (_velocity.x,Parameters.MaxVelocity.x);
		_velocity.y=Mathf.Min(_velocity.y,Parameters.MaxVelocity.y);

		if(State.IsMovingUpSlope)
			_velocity.y=0;
	}

	private void HandlePlatforms()
	{}

	private void CalculateRayOrigins()
	{
		var size= new Vector2(_boxCollider.size.x*Mathf.Abs(_localScale.x),_boxCollider.size.y*Mathf.Abs(_localScale.y))/2;
		var center= new Vector2(_boxCollider.center.x * _localScale.x,_boxCollider.center.y*_localScale.y);

		_raycastTopLeft=_transform.position+ new Vector3(center.x-size.x + SkinWidth, center.y + size.y -SkinWidth);
		_raycastBottomRight=_transform.position+new Vector3(center.x+size.x - SkinWidth, center.y - size.y +SkinWidth);
		_raycastTopLeft=_transform.position+new Vector3(center.x-size.x + SkinWidth, center.y - size.y + SkinWidth);
	}

	private void MoveHorizontally(ref Vector2 deltaMovement)
	{
		var IsGoingRight = deltaMovement.x > 0;
		var rayDistance=Mathf.Abs(deltaMovement.x)+SkinWidth;
		var rayDirection = IsGoingRight ? Vector2.right : -Vector2.right;
		var rayOrigin = IsGoingRight ? _raycastBottomRight : _raycastBottomLeft;

		for(var i=0;i<TotalHorizontalRay; i++)
		{
			var rayVector=new Vector2(rayOrigin.x,rayOrigin.y+(i*_verticalDistanceBetweenRays));
			Debug.DrawRay(rayVector,rayDirection*rayDistance,Color.red);

			var rayCastHit=Physics2D.Raycast (rayVector,rayDirection,rayDistance,PlatformMask);

			if(!rayCastHit)
				continue;

			if( i == 0 && HandleHorizontalSlope(ref deltaMovement,Vector2.Angle(rayCastHit.normal,Vector2.up),IsGoingRight))
				break;

			deltaMovement.x= rayCastHit.point.x-rayVector.x;
			rayDistance=Mathf.Abs (deltaMovement.x);

			if(IsGoingRight)
			{
				deltaMovement.x-=SkinWidth;
				State.IsCollidingRight=true;
			}
			else
			{
				deltaMovement.x+=SkinWidth;
				State.IsCollidingLeft=true;
			}
			if(rayDistance<SkinWidth+.0001f)
				break;

		}
	}

	private void MoveVertically(ref Vector2 deltaMovement)
	{
		var IsgoingUp=deltaMovement.y>0;
		var rayDistance=Mathf.Abs (deltaMovement.y)+SkinWidth;
		var rayDirection=IsgoingUp? Vector2.up : -Vector2.up;
		var rayOrigin = IsgoingUp ? _raycastBottomLeft :_raycastBottomRight;

		rayOrigin.x+=deltaMovement.x;

		var standingOnDistance=float.MaxValue;

		for (var i=0;i<TotalVerticalRay;i++)
		{
			var rayVector=new Vector2(rayOrigin.x+(i*_horizontalDistanceBetweenRays),rayOrigin.y);
			Debug.DrawRay(rayVector,rayDirection*rayDistance,Color.red);

			var raycastHit=Physics2D.Raycast(rayVector,rayDirection,rayDistance,PlatformMask);

			if(!raycastHit)
				continue;
			if(!IsgoingUp)
			{
				var verticalDistanceToHit=_transform.position.y-raycastHit.point.y;
				if(verticalDistanceToHit<standingOnDistance)
				{
					standingOnDistance = verticalDistanceToHit;
					StandingOn = raycastHit.collider.gameObject;
				}
			}
			deltaMovement.y=raycastHit.point.y-rayVector.y;
			rayDistance=Mathf.Abs(deltaMovement.y);

			if(IsgoingUp)
			{
				deltaMovement.y -= SkinWidth;
				State.IsCollidingAbove=true;
			}
			else
			{
				deltaMovement.y+=SkinWidth;
				State.IsMovingUpSlope=true;
			}
			if(!IsgoingUp && deltaMovement.y> .0001f)
				State.IsMovingDownSlope=true;

			if( rayDistance< SkinWidth + .0001f)
				break;

		}

	}

	private void HandleVerticalSlope(ref Vector2 deltaMovement )
	{
		var center =(_raycastBottomLeft.x + _raycastBottomRight.x)/2;
		var direction = -Vector2.up;

		var slopeDistance=SlopeLimitTangent*(_raycastBottomRight.x - center);
		var slopeRayVector = new Vector2(center, _raycastBottomLeft.y);

		Debug.DrawRay(slopeRayVector,direction*slopeDistance,Color.yellow);

		var raycastHit=Physics2D.Raycast(slopeRayVector,direction,slopeDistance,PlatformMask);

		if(!raycastHit)
			return;

		var isMovingDownSlope= Mathf.Sign(raycastHit.normal.x)==Mathf.Sign(deltaMovement.x);
		if(!isMovingDownSlope)
			return;

		var angle=Vector2.Angle(raycastHit.normal,Vector2.up);
		if(Mathf.Abs(angle)<.001f)
			return;

		State.IsMovingDownSlope =true;
		State.SlopeAngle =angle;
		deltaMovement.y = raycastHit.point.y - slopeRayVector.y;
	}

	private bool  HandleHorizontalSlope(ref Vector2 deltaMovement,float angle, bool IsGoingRight ) //return boolian
	{
		if(Mathf.RoundToInt(angle)==90)
		return false;

		if(angle>Parameters.SlopeLimit)
		{
			deltaMovement.x=0;
			return true;
		}
		if(deltaMovement.y> .07f)
			return true;
		deltaMovement.x += IsGoingRight ? -SkinWidth: SkinWidth ;
		deltaMovement.y = Mathf.Abs (Mathf.Tan(angle*Mathf.Deg2Rad)*deltaMovement.x);
		State.IsMovingUpSlope=true;
		State.IsCollidingBelow=true;
		return true;
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		var parameters= other.gameObject.GetComponent<ControllerPhysicsVolume2D>();
		if(parameters==null)
			return;

		_overrideParameters=parameters.Parameters;
	}

	public void OnTriggerExit2D(Collider2D other)
	{
		var parameters= other.gameObject.GetComponent<ControllerPhysicsVolume2D>();
		if (parameters==null)
			return; 

			_overrideParameters=null;
	}

}
