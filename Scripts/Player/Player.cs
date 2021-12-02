using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using Pap1.Scripts;
using Pap1.Scripts.Attributes;

public class Player : KinematicBody
{
    public enum States
    {
        Idle,
        Walking,
        Sprinting
    }

    public bool IsPlayerOne = false;

    #region Physics properties

    private const int Gravity = -25;
    private const int MaxSpeed = 5;
    private const int MaxSprintSpeed = 6;
    private const int MaxCrouchSpeed = 5;
    private const int JumpSpeed = 6;
    private const int Accel = 5;
    private const int SprintAccel = 7;
    private const float CrouchAccel = 2.5f;
    private const int Deaccel = 5;
    private const int MaxSlopeAngle = 40;

    private const int MaxSprintTime = 2;
    private const int MaxSprintTimeout = 4;
    const float SprintFov = 95;

    const int MaxStairSlope = 20;
    const int StairJumpHeight = 4;

    #endregion

    private Camera camera;
    private float cameraAngle;
    private Vector2 cameraChange;

    private bool canJump = true;
    private bool canSprint = true;
    private Vector3 dir;
    private Spatial head;

    private float initialFov;
    private bool isCrouching;

    private bool isDead;
    private bool isLanded;
    private bool isLanding;

    private float sprintTimeout = MaxSprintTimeout;

    private States state;
    private RayCast floorCheck;

    private int timeInAir = 0;
    private float timeSprinting;

    private Vector3 vel;

    private RayCast interactionRc;
    private Hud hud;

    private Vector2 inputMovemnetVec;

    private List<States> currentStates;

    public enum KEY
    {
        FORWARD,
        BACKWARD,
        LEFT,
        RIGHT,
        JUMP,
        SPRINT
    };

    public bool[] Command = {false, false, false, false, false, false};

    public Player(bool isCrouching)
    {
        this.isCrouching = isCrouching;
    }

    public Player()
    {
    }

    public override void _Ready()
    {
        camera = GetNode<Camera>("Head/Camera");
//		Input.SetMouseMode(Input.MouseMode.Captured);
        hud = GetNode<Hud>("Hud");
        head = GetNode<Spatial>("Head");
        floorCheck = GetNode<RayCast>("FloorCheck");

        interactionRc = GetNode<RayCast>("Head/Camera/InteractionRay");
        initialFov = camera.Fov;
        vel = new Vector3();
        dir = new Vector3();

        currentStates = new List<States>();
    }

//	public override void _Input(InputEvent @event)
//	{
//		if (@event is InputEventMouseMotion motion)
//		{
//			cameraChange = motion.Relative;
//		}
//	}
    public override void _PhysicsProcess(float delta)
    {
        ProcessInput(delta);
//		ProcessAim();

        if (currentStates.Contains(States.Walking))
        {
            ProcessWalking(delta);
        }

        if (!IsOnFloor())
        {
            Fall(delta);
        }

        // if (IsNetworkMaster())
        // if (Attribute.GetCustomAttribute(head.GetType(), typeof(ActivePlayer)) != null)
        if (head.HasMethod("IsPlayer"))
        {
            RpcUnreliable("network_update", vel, MaxSlopeAngle);
        }

        ProcessFootsteps(delta);
        // ProcessCollisions();
        // ProcessSprinting(delta);
        // processCrouching(delta);
        // ProcessStairs(delta);
        // ProcessLanding(delta);

        ProcessInteraction();
    }

    [Sync]
    private void network_update(Vector3 updatedVel, int updatedMaxSlopeAngle)
    {
        vel = MoveAndSlide(updatedVel, new Vector3(0, 1, 0), true, 1, Mathf.Deg2Rad(updatedMaxSlopeAngle));
    }

    private void ProcessInteraction()
    {
        // if (((Node) interactionRc.GetCollider()) != null)
        // {
        //     if (((Node) interactionRc.GetCollider()).GetParent() is IInteractable inter)
        //     {
        //         var info = inter.GetInfo();
        //         // hud.SetInteractionText(info);
        //         if (Input.IsActionJustPressed("interact"))
        //         {
        //             inter.Interact();
        //         }
        //     }
        //     else if (((Node) interactionRc.GetCollider()).GetParent() is ISkipToParent)
        //     {
        //         if (((Node) interactionRc.GetCollider()).GetParent().GetParent() is IInteractable inter1)
        //         {
        //             var info = inter1.GetInfo();
        //             // hud.SetInteractionText(info);
        //             if (Input.IsActionJustPressed("interact"))
        //             {
        //                 inter1.Interact();
        //             }
        //         }
        //     }
        // }
        // else
        // {
        //     // hud.SetInteractionText("");
        // }
    }

//	private void ProcessAim()
//	{
//		if (cameraChange.Length() > 0)
//		{
//			this.RotateY(Mathf.Deg2Rad(-cameraChange.x * Settings.MouseSensitivity));
//
//			var change = -cameraChange.y * Settings.MouseSensitivity;
//			if (change + cameraAngle < 90 && change + cameraAngle > -90)
//			{
//				camera.RotateX(Mathf.Deg2Rad(change));
//				cameraAngle += change;
//			}
//
//			cameraChange = new Vector2();
//		}
//	}

    private void ProcessInput(float _)
    {
        inputMovemnetVec = new Vector2();
        dir = new Vector3();
        var camXform = GlobalTransform;
//
//		if (Input.IsActionPressed("move_forward"))
//			// GD.Print("maju");
//			inputMovemnetVec.y += 1;
//		if (Input.IsActionPressed("move_backward"))
//			inputMovemnetVec.y -= 1;
//		if (Input.IsActionPressed("move_left"))
//			inputMovemnetVec.x -= 1;
//		if (Input.IsActionPressed("move_right"))
//			inputMovemnetVec.x += 1;
//
        if (Command[(int) KEY.FORWARD])
            // GD.Print("maju");
            inputMovemnetVec.y += 1;
        if (Command[(int) KEY.BACKWARD])
            inputMovemnetVec.y -= 1;
        if (Command[(int) KEY.LEFT])
            inputMovemnetVec.x -= 1;
        if (Command[(int) KEY.RIGHT])
            inputMovemnetVec.x += 1;

        if (inputMovemnetVec.Length() != 0)
        {
            AddState(States.Walking);
        }
        else
        {
        }

        if (inputMovemnetVec.Length() != 0f)
            inputMovemnetVec = inputMovemnetVec.Normalized();

        dir += -camXform.basis.z.Normalized() * inputMovemnetVec.y;
        dir += camXform.basis.x.Normalized() * inputMovemnetVec.x;

        // Jump
        if (Command[(int) KEY.JUMP] && IsOnFloor() && !isDead && canJump)
        {
            vel.y = JumpSpeed;
            // Play jump audio
        }

        // Sprint
        // if (Input.IsActionPressed("sprint") && canSprint)
        // {
        // 	AddState(States.Sprinting);
        // }
        // else
        // {
        // 	RemoveState(States.Sprinting);
        // }
    }

    private void ProcessFootsteps(float delta)
    {
    }

    private static void ProcessCollisions()
    {
    }


    private void ProcessStairs(float delta)
    {
    }

    private void ProcessLanding(float delta)
    {
    }

    private void AddState(States state)
    {
        if (!currentStates.Contains(state))
        {
            currentStates.Add(state);
        }
    }

    private void RemoveState(States state)
    {
        if (currentStates.Contains(state))
        {
            currentStates.Remove(state);
        }
    }

    private void Fall(float delta)
    {
        vel.y += delta * Gravity;
        vel.y = Mathf.Clamp(vel.y, -10, 10);
    }

    private void ProcessSprinting(float delta)
    {
        if (currentStates.Contains(States.Sprinting))
        {
            /// Reduce remaining sprint time
            timeSprinting += delta;
            sprintTimeout -= delta;
            // hud.SprintBarValue = 100 * (MaxSprintTime - timeSprinting) / MaxSprintTime;
        }

        // recovery
        else if (canSprint && !Command[(int) KEY.SPRINT])
        {
            timeSprinting -= delta;
            timeSprinting = Mathf.Clamp(timeSprinting, 0, MaxSprintTime);

            if (timeSprinting > 0)
                sprintTimeout += delta;

            sprintTimeout = Mathf.Clamp(sprintTimeout, 0, MaxSprintTimeout);

            // hud.SprintBarValue = 100 * (MaxSprintTime - timeSprinting) / MaxSprintTime;
        }

        if (timeSprinting >= MaxSprintTime)
        {
            canSprint = false;
            sprintTimeout += delta;

            // hud.SprintBarValue = 100 - 100 * (MaxSprintTimeout - sprintTimeout) / MaxSprintTimeout;
        }

        if (sprintTimeout >= MaxSprintTimeout)
        {
            canSprint = true;
            sprintTimeout = 0;
            timeSprinting = 0;
        }

        // Sprint FOV
        camera.Fov += (initialFov - camera.Fov) * 5 * delta;
        if (currentStates.Contains(States.Sprinting) && dir.Dot(vel) > 0)
        {
            camera.Fov += (SprintFov - camera.Fov) * 5 * delta;
        }
    }

    private void ProcessWalking(float delta)
    {
        if (!isDead)
        {
            dir.y = 0;
            dir = dir.Normalized();

            var hvel = vel;
            hvel.y = 0;

            var target = dir;

            if (currentStates.Contains(States.Sprinting))
            {
                target *= MaxSprintSpeed;
            }
            else if (isCrouching)
            {
                target *= MaxCrouchSpeed;
            }
            else
            {
                target *= MaxSpeed;
            }

            float accel;
            if (dir.Dot(hvel) > 0)
            {
                if (currentStates.Contains(States.Sprinting))
                {
                    accel = SprintAccel;
                }
                else if (isCrouching)
                {
                    accel = CrouchAccel;
                }
                else
                {
                    accel = Accel;
                }
            }
            else
            {
                accel = Deaccel;
            }

            hvel = hvel.LinearInterpolate(target, accel * delta);


            vel.x = hvel.x;
            vel.z = hvel.z;

            if (new Vector2(vel.x, vel.z).Length() == 0f)
            {
                RemoveState(States.Walking);
            }
        }
    }
}