using Godot;
using System;
using System.Collections.Generic;
using Pap1.Scripts;

public class Player : KinematicBody
{
    public enum States
    {
        Idle,
        Walking,
        Sprinting
    }

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

    private Camera _camera;
    private float _cameraAngle;
    private Vector2 _cameraChange;

    private bool _canJump = true;
    private bool _canSprint = true;
    private Vector3 _dir;
    private Spatial _head;

    private float _initialFov;
    private bool _isCrouching;

    private bool _isDead;
    private bool _isLanded;
    private bool _isLanding;

    private float _sprintTimeout = MaxSprintTimeout;

    private States _state;
    private RayCast _floorCheck;

    private int _timeInAir = 0;
    private float _timeSprinting;

    private Vector3 _vel;

    private RayCast _interactionRc;
    private Hud hud;

    private Vector2 _inputMovemnetVec;

    private List<States> _currentStates;

    public Player()
    {
    }

    public override void _Ready()
    {
        _camera = GetNode<Camera>("Head/Camera");
        Input.SetMouseMode(Input.MouseMode.Captured);
        hud = GetNode<Hud>("Hud");
        _head = GetNode<Spatial>("Head");
        _floorCheck = GetNode<RayCast>("FloorCheck");

        _interactionRc = GetNode<RayCast>("Head/Camera/InteractionRay");
        _initialFov = _camera.Fov;
        _vel = new Vector3();
        _dir = new Vector3();

        _currentStates = new List<States>();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
        {
            _cameraChange = motion.Relative;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        ProcessInput(delta);

        if (_currentStates.Contains(States.Walking))
        {
            ProcessWalking(delta);
        }

        if (!IsOnFloor())
        {
            Fall(delta);
        }

        _vel = MoveAndSlide(_vel, new Vector3(0, 1, 0), true, 1, Mathf.Deg2Rad(MaxSlopeAngle));


        ProcessAim();
        ProcessFootsteps(delta);
        ProcessCollisions();
        // ProcessSprinting(delta);
        // processCrouching(delta);
        ProcessStairs(delta);
        ProcessLanding(delta);

        ProcessInteraction();
    }

    private void ProcessInteraction()
    {
        if (((Node) _interactionRc.GetCollider()) != null)
        {
            if (((Node) _interactionRc.GetCollider()).GetParent() is IInteractable inter)
            {
                var info = inter.GetInfo();
                // hud.SetInteractionText(info);
                if (Input.IsActionJustPressed("interact"))
                {
                    inter.Interact();
                }
            }
            else if (((Node) _interactionRc.GetCollider()).GetParent() is ISkipToParent)
            {
                if (((Node) _interactionRc.GetCollider()).GetParent().GetParent() is IInteractable inter1)
                {
                    var info = inter1.GetInfo();
                    // hud.SetInteractionText(info);
                    if (Input.IsActionJustPressed("interact"))
                    {
                        inter1.Interact();
                    }
                }
            }
        }
        else
        {
            // hud.SetInteractionText("");
        }
    }

    private void ProcessAim()
    {
        if (_cameraChange.Length() > 0)
        {
            this.RotateY(Mathf.Deg2Rad(-_cameraChange.x * Settings.MouseSensitivity));

            var change = -_cameraChange.y * Settings.MouseSensitivity;
            if (change + _cameraAngle < 90 && change + _cameraAngle > -90)
            {
                _camera.RotateX(Mathf.Deg2Rad(change));
                _cameraAngle += change;
            }

            _cameraChange = new Vector2();
        }
    }

    private void ProcessInput(float _)
    {
        _inputMovemnetVec = new Vector2();
        _dir = new Vector3();
        var camXform = GlobalTransform;

        if (Input.IsActionPressed("move_forward"))
            _inputMovemnetVec.y += 1;
        if (Input.IsActionPressed("move_backward"))
            _inputMovemnetVec.y -= 1;
        if (Input.IsActionPressed("move_left"))
            _inputMovemnetVec.x -= 1;
        if (Input.IsActionPressed("move_right"))
            _inputMovemnetVec.x += 1;

        if (_inputMovemnetVec.Length() != 0)
        {
            AddState(States.Walking);
        }
        else
        {
        }

        if (_inputMovemnetVec.Length() != 0f)
            _inputMovemnetVec = _inputMovemnetVec.Normalized();

        _dir += -camXform.basis.z.Normalized() * _inputMovemnetVec.y;
        _dir += camXform.basis.x.Normalized() * _inputMovemnetVec.x;

        // Jump
        if (Input.IsActionPressed("jump") && IsOnFloor() && !_isDead && _canJump)
        {
            _vel.y = JumpSpeed;
            // Play jump audio
        }

        // Sprint
        if (Input.IsActionPressed("sprint") && _canSprint)
        {
            AddState(States.Sprinting);
        }
        else
        {
            RemoveState(States.Sprinting);
        }
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
        if (!_currentStates.Contains(state))
        {
            _currentStates.Add(state);
        }
    }

    private void RemoveState(States state)
    {
        if (_currentStates.Contains(state))
        {
            _currentStates.Remove(state);
        }
    }

    private void Fall(float delta)
    {
        _vel.y += delta * Gravity;
        _vel.y = Mathf.Clamp(_vel.y, -10, 10);
    }

    private void ProcessSprinting(float delta)
    {
        if (_currentStates.Contains(States.Sprinting))
        {
            /// Reduce remaining sprint time
            _timeSprinting += delta;
            _sprintTimeout -= delta;
            // hud.SprintBarValue = 100 * (MaxSprintTime - timeSprinting) / MaxSprintTime;
        }

        // recovery
        else if (_canSprint && !Input.IsActionPressed("sprint"))
        {
            _timeSprinting -= delta;
            _timeSprinting = Mathf.Clamp(_timeSprinting, 0, MaxSprintTime);

            if (_timeSprinting > 0)
                _sprintTimeout += delta;

            _sprintTimeout = Mathf.Clamp(_sprintTimeout, 0, MaxSprintTimeout);

            // hud.SprintBarValue = 100 * (MaxSprintTime - timeSprinting) / MaxSprintTime;
        }

        if (_timeSprinting >= MaxSprintTime)
        {
            _canSprint = false;
            _sprintTimeout += delta;

            // hud.SprintBarValue = 100 - 100 * (MaxSprintTimeout - sprintTimeout) / MaxSprintTimeout;
        }

        if (_sprintTimeout >= MaxSprintTimeout)
        {
            _canSprint = true;
            _sprintTimeout = 0;
            _timeSprinting = 0;
        }

        // Sprint FOV
        _camera.Fov += (_initialFov - _camera.Fov) * 5 * delta;
        if (_currentStates.Contains(States.Sprinting) && _dir.Dot(_vel) > 0)
        {
            _camera.Fov += (SprintFov - _camera.Fov) * 5 * delta;
        }
    }

    private void ProcessWalking(float delta)
    {
        if (!_isDead)
        {
            _dir.y = 0;
            _dir = _dir.Normalized();

            var hvel = _vel;
            hvel.y = 0;

            var target = _dir;

            if (_currentStates.Contains(States.Sprinting))
            {
                target *= MaxSprintSpeed;
            }
            else if (_isCrouching)
            {
                target *= MaxCrouchSpeed;
            }
            else
            {
                target *= MaxSpeed;
            }

            float accel;
            if (_dir.Dot(hvel) > 0)
            {
                if (_currentStates.Contains(States.Sprinting))
                {
                    accel = SprintAccel;
                }
                else if (_isCrouching)
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


            _vel.x = hvel.x;
            _vel.z = hvel.z;

            if (new Vector2(_vel.x, _vel.z).Length() == 0f)
            {
                RemoveState(States.Walking);
            }
        }
    }
}