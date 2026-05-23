using Godot;
using System;

public partial class Player : CharacterBody3D
{
	private const float Speed = 7.0f;
	private const float JumpVelocity = 5f;
	private const float CameraSensitivity = 0.005f;

	private Node3D head;
	private Camera3D camera;

	public override void _Ready()
	{
		head = GetNode<Node3D>("Head");
		camera = GetNode<Camera3D>("Head/Camera3D");
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		//Manage control of the mouse cursor
		if (@event is InputEventMouseButton)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		else if (@event.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		
		//Actual camera movement
		if (@event is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			InputEventMouseMotion motion = (InputEventMouseMotion)@event;
			
			head.RotateY(-motion.Relative.X * CameraSensitivity);
			camera.RotateX(-motion.Relative.Y * CameraSensitivity);

			Vector3 camRot = new Vector3(Mathf.Clamp(camera.Rotation.X, Mathf.DegToRad(-80), Mathf.DegToRad(80)), 0, 0);
			camera.Rotation = camRot;
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		if (Input.IsActionJustPressed("Jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}
		
		Vector2 inputDir = Input.GetVector("Left", "Right", "Forward", "Backward");
		Vector3 direction = (head.GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
