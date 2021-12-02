using Godot;
using System;
using Pap1.Scripts;

public class ControlledPlayer : KinematicBody
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	private Camera camera;
	private float cameraAngle;
	private Vector2 cameraChange;
	private Player character;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.SetMouseMode(Input.MouseMode.Captured);
		camera = GetNode<Camera>("Camera");
		character = GetParent<Player>();
		camera.Current = true;
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motion)
		{
			cameraChange = motion.Relative;
		}
		character.Command[(int) Player.KEY.FORWARD] = Input.IsActionPressed("move_forward");
		character.Command[(int) Player.KEY.BACKWARD] = Input.IsActionPressed("move_backward");
		character.Command[(int) Player.KEY.LEFT] = Input.IsActionPressed("move_left");
		character.Command[(int) Player.KEY.RIGHT] = Input.IsActionPressed("move_right");
		character.Command[(int) Player.KEY.JUMP] = Input.IsActionPressed("jump");
//		character.Command[(int) Player.KEY.SPRINT] = Input.IsActionPressed("sprint");
		
	}
	
	public override void _PhysicsProcess(float delta)
	{
//		ProcessInput(delta);
		ProcessAim();
	}
	
	private void ProcessAim()
	{
		if (cameraChange.Length() > 0)
		{
			character.RotateY(Mathf.Deg2Rad(-cameraChange.x * Settings.MouseSensitivity));

			var change = -cameraChange.y * Settings.MouseSensitivity;
			if (change + cameraAngle < 90 && change + cameraAngle > -90)
			{
				camera.RotateX(Mathf.Deg2Rad(change));
				cameraAngle += change;
			}

			cameraChange = new Vector2();
		}
	}
	
	public bool is_player()
	{
		return true;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
