using Godot;
using System;
using Pap1.Scripts;

public class ControlledPlayer : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private Camera camera;
    private float cameraAngle;
    private Vector2 cameraChange;
    private Player character;
    private SpellController spellController;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Input.SetMouseMode(Input.MouseMode.Captured);
        camera = GetNode<Camera>("Camera");
        character = GetParent<Player>();
        spellController = GetNode<SpellController>("SpellController");
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
        if (!(@event is InputEventMouseButton mouseEvent) || !mouseEvent.Pressed) return;
        switch ((ButtonList) mouseEvent.ButtonIndex)
        {
            case ButtonList.Left:
                spellController.Cast();
                break;
            case ButtonList.Right:
                break;
            case ButtonList.Middle:
                break;
            case ButtonList.Xbutton1:
                break;
            case ButtonList.Xbutton2:
                break;
            case ButtonList.WheelUp:
                break;
            case ButtonList.WheelDown:
                break;
            case ButtonList.WheelLeft:
                break;
            case ButtonList.WheelRight:
                break;
            case ButtonList.MaskXbutton1:
                break;
            case ButtonList.MaskXbutton2:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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

    public void IsPlayer()
    {
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}