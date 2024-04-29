using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour {

	[SerializeField] private InputGameActions input;

	[SerializeField] private float walkSpeed;
	[SerializeField] private float walkAcceleration;
	[Space]
	[SerializeField] private float runSpeed;
	[SerializeField] private float runAcceleration;
	[Space]
	[SerializeField] private float jumpUpSpeed;
	[SerializeField] private float jumpThrustSpeed;
	[SerializeField] private float gravityAcceleration;
	[SerializeField] private float stepDownSpeed;
	[Space]
	[SerializeField] private float groundFriction;
	[SerializeField] private float airFriction;
	[Space]
	[SerializeField][Range(0.0F, 1.0F)] private float airControl;
	[SerializeField][Range(1.0F, 3.0F)] private float turnAroundMultiplier;

	private CharacterController controller;
	private new Camera camera;
	private bool wasGrounded;
	private Vector3 desiredVelocity;
	private float yaw;
	private float pitch;

	private void Awake() {
		controller = GetComponent<CharacterController>();
		camera = GetComponentInChildren<Camera>();
		input = new InputGameActions();
		input.Enable();
	}

	private void Update() {
		if(Game.Instance.IsPaused) return;

		Move();
		Rotate();
	}

	private void Move() {
		Vector3 inputVector;
		Vector3 targetVelocity;
		bool isGrounded;
		bool stop;
		bool run;
		float turnAround;
		float speed;
		float acceleration;
		
		
		inputVector = input.FindAction("Player/Move").ReadValue<Vector2>();
		inputVector = transform.TransformDirection(
			Vector3.ClampMagnitude(
				new Vector3(inputVector.x, 0.0F, inputVector.y),
				1.0F
			)
		);

		targetVelocity = Vector3.ProjectOnPlane(controller.velocity, Vector3.up);

		isGrounded = controller.isGrounded;
		stop = inputVector.magnitude < 0.001F;
		run = input.FindAction("Player/Run").IsPressed();
		turnAround = Mathf.Clamp01(Vector3.Dot(targetVelocity.normalized, -inputVector));

		if(stop) {
			speed = 0.0F;
			acceleration = groundFriction;
		} else if(run) {
			speed = runSpeed;
			acceleration = runAcceleration;
		} else {
			speed = walkSpeed;
			acceleration = walkAcceleration;
		}

		if(speed <= targetVelocity.magnitude && turnAround < 0.5F) {
			acceleration = isGrounded ? groundFriction : airFriction;
		} else if(isGrounded) {
			acceleration *= Mathf.Lerp(
				1.0F,
				turnAroundMultiplier,
				turnAround
			);
		} else {
			acceleration *= airControl;
		}

		targetVelocity = Vector3.MoveTowards(targetVelocity, speed * inputVector, acceleration * Time.deltaTime);
		targetVelocity.y = desiredVelocity.y;

		if(isGrounded) {
			if(!wasGrounded && targetVelocity.y <= 0.0F) {
				targetVelocity.y = -1.0F;
			} else if(input.FindAction("Player/Jump").IsPressed()) {
				targetVelocity.y = jumpUpSpeed;
				targetVelocity += inputVector * jumpThrustSpeed;
			}
		} else {
			if(wasGrounded && targetVelocity.y <= 0.0F) {
				targetVelocity.y = -stepDownSpeed;
			} else {
				targetVelocity.y -= gravityAcceleration * Time.deltaTime;
			}
		}

		// TODO: Ceiling bump.

		wasGrounded = isGrounded;
		desiredVelocity = targetVelocity;
		controller.Move(desiredVelocity * Time.deltaTime);
	}

	private void Rotate() {
		Vector2 rotate = input.FindAction("Player/Rotate").ReadValue<Vector2>() / 10.0F;

		yaw += rotate.x * Options.MouseSensitivity;
		pitch += -rotate.y * Options.MouseSensitivity;
		pitch = Mathf.Clamp(pitch, -80.0F, 80.0F);

		transform.eulerAngles = Vector3.up * yaw;
		camera.transform.localEulerAngles = Vector3.right * pitch;
	}

}
