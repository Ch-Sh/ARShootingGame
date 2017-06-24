using UnityEngine;
using System.Collections;

public class DualTouchPlayerController : MonoBehaviour
{
	public LeftJoystick leftJoystick; // the game object containing the LeftJoystick script
	public RightJoystick rightJoystick; // the game object containing the RightJoystick script
	public float moveSpeed = 6.0f; // movement speed of the player character
	public float rotationSpeed = 8; // rotation speed of the player character
	public Transform rotationTarget; // the game object that will rotate to face the input direction
	public Animator animator; // the animator controller of the player character

	private Vector3 leftJoystickInput; // holds the input of the Left Joystick
	private Vector3 rightJoystickInput; // hold the input of the Right Joystick
	private Rigidbody rigidBody; // rigid body component of the player character
	private Vector3 movement;

	void Start()
	{
		if (transform.GetComponent<Rigidbody>() == null)
		{
			Debug.LogError("A RigidBody component is required on this game object.");
		}
		else
		{
			rigidBody = transform.GetComponent<Rigidbody>();
		}

		if (leftJoystick == null)
		{
			Debug.LogError("The left joystick is not attached.");
		}

		if (rightJoystick == null)
		{
			Debug.LogError("The right joystick is not attached.");
		}

		if (rotationTarget == null)
		{
			Debug.LogError("The target rotation game object is not attached.");
		}
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		// get input from both joysticks
		leftJoystickInput = leftJoystick.GetInputDirection();
		//leftJoystickInput.z = 0;

		rightJoystickInput = rightJoystick.GetInputDirection();
		//rightJoystickInput.y = 0;
			
		if (leftJoystickInput != Vector3.zero)
			Move ();
		else
			animator.SetBool ("IsWalking",false);

		if (rightJoystickInput != Vector3.zero)
			Turn ();
		
	}

	void Move()
	{
		movement.Set (leftJoystickInput.x,0,leftJoystickInput.y);

		movement = movement.normalized * moveSpeed * Time.fixedDeltaTime;

		rigidBody.MovePosition (transform.position + movement);

		animator.SetBool ("IsWalking",true);

	}

	void Turn()
	{

		float xMovementRightJoystick = rightJoystickInput.x; // The horizontal movement from joystick 02
		float zMovementRightJoystick = rightJoystickInput.y; 

		float tempAngle = Mathf.Atan2(zMovementRightJoystick, xMovementRightJoystick);
		xMovementRightJoystick *= Mathf.Abs(Mathf.Cos(tempAngle));
		zMovementRightJoystick *= Mathf.Abs(Mathf.Sin(tempAngle));

		// rotate the player to face the direction of input
		Vector3 temp = transform.position;
		temp.x += xMovementRightJoystick;
		temp.z += zMovementRightJoystick;
		Vector3 lookDirection = temp - transform.position;
		if (lookDirection != Vector3.zero)
		{
			rotationTarget.localRotation = Quaternion.Slerp(rotationTarget.localRotation, 
				Quaternion.LookRotation(lookDirection) * Quaternion.Euler(0, 45f, 0), 
				rotationSpeed * Time.deltaTime);
		}

	}

}

