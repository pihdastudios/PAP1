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
  private const int JumpSpeed = 4;
  private const int Accel = 5;
  private const int SprintAccel = 7;
  private const float CrouchAccel = 2.5f;
  private const int Deaccel = 5;
  private const int MaxSlopeAngle = 40;
  public float SlowEffect = 0;

  private const int MaxSprintTime = 2;
  private const int MaxSprintTimeout = 4;
  private float maxHealth = 100;
  private float health = 100;
  private HealthBar3D healthBar;
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

  private RayCast interactionRc;

  private States state;
  private RayCast floorCheck;

  private int timeInAir = 0;
  private float timeSprinting;

  private Vector3 vel;

  private Hud hud;

  private Vector2 inputMovemnetVec;

  private List<States> currentStates;

  public enum Key
  {
	Forward,
	Backward,
	Left,
	Right,
	Jump,
	Sprint
  };

  public bool[] Command = { false, false, false, false, false, false };

  public Player(bool isCrouching)
  {
	this.isCrouching = isCrouching;
  }

  public Player()
  {
  }

  public override void _Ready()
  {
	healthBar = GetNode<HealthBar3D>("HealthBar3D");
	healthBar.Update(health, maxHealth);
	camera = GetNode<Camera>("Head/Camera");
	//		Input.SetMouseMode(Input.MouseMode.Captured);
	hud = GetNode<Hud>("Hud");
	hud.updateHealth(health);
	head = GetNode<Spatial>("Head");
	floorCheck = GetNode<RayCast>("FloorCheck");

	initialFov = camera.Fov;
	vel = new Vector3();
	dir = new Vector3();

	currentStates = new List<States>();
	interactionRc = GetNode<RayCast>("Head/Camera/InteractionRay");
  }

  private void ProcessInteraction()
  {
	if (((Node)interactionRc.GetCollider()) != null)
	{
	  if (((Node)interactionRc.GetCollider()) is IInteractable inter)
	  {
		var info = inter.GetInfo();
		hud.SetInteractionText(info);
		if (Input.IsActionJustPressed("interact"))
		{
		  inter.Interact();
		}
	  }
	}
	else
	{
	  hud.SetInteractionText("");
	}
  }

  public void TakeDamage(float damage)
  {
	health -= damage;
	healthBar.Update(health, maxHealth);
	hud.updateHealth(health);
	SlowEffect += 0.3f;
	if (SlowEffect > 0.9f)
	  SlowEffect = 0.9f;
	if (health <= 0 && Globals.CurrentRole == Globals.Role.Attacker)
	{
	  Rpc("ResetPosition");
	}

	CancelEffect();
  }

  private async void CancelEffect()
  {
	var t = new Timer();
	t.WaitTime = 5;
	t.OneShot = true;
	AddChild(t);
	t.Start();
	await ToSignal(t, "timeout");
	t.QueueFree();
	SlowEffect -= 0.3f;
	if (SlowEffect < 0)
	  SlowEffect = 0f;
  }

  [Sync]
  private void ResetPosition()
  {
	GlobalTransform = GetTree().Root.GetNode<Position3D>("DemoScene/Player1Pos").GlobalTransform;
	health = maxHealth;
	healthBar.Update(health, maxHealth);
	hud.updateHealth(health);
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
	  RpcUnreliable("NetworkUpdate", vel, MaxSlopeAngle);
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
  private void NetworkUpdate(Vector3 updatedVel, int updatedMaxSlopeAngle)
  {
	vel = MoveAndSlide(updatedVel, new Vector3(0, 1, 0), true, 1, Mathf.Deg2Rad(updatedMaxSlopeAngle));
  }

  public void Fire()
  {
	Transform pos = GetNode<Spatial>("Head/Camera/ItemHolder/Spell/Muzzle").GlobalTransform;
	RpcUnreliable("NetworkUpdate2", pos);
  }

  [Sync]
  private void NetworkUpdate2(Transform pos)
  {
	var currentProjectile = GD.Load<PackedScene>("res://Scenes/Spells/SpellApple.tscn");
	var newProjectile = currentProjectile.Instance<SpellApple>();
	newProjectile.GlobalTransform = pos;
	var sceneRoot = GetTree().Root.GetChildren()[1] as Node;
	sceneRoot?.AddChild(newProjectile);
  }


  private void ProcessInput(float _)
  {
	inputMovemnetVec = new Vector2();
	dir = new Vector3();
	var camXform = GlobalTransform;

	if (Command[(int)Key.Forward])
	  inputMovemnetVec.y += 1;
	if (Command[(int)Key.Backward])
	  inputMovemnetVec.y -= 1;
	if (Command[(int)Key.Left])
	  inputMovemnetVec.x -= 1;
	if (Command[(int)Key.Right])
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
	if (Command[(int)Key.Jump] && IsOnFloor() && !isDead && canJump)
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
	  // Reduce remaining sprint time
	  timeSprinting += delta;
	  sprintTimeout -= delta;
	  // hud.SprintBarValue = 100 * (MaxSprintTime - timeSprinting) / MaxSprintTime;
	}

	// recovery
	else if (canSprint && !Command[(int)Key.Sprint])
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
	if (isDead) return;
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

	hvel = hvel.LinearInterpolate(target, accel * delta * (1 - SlowEffect));


	vel.x = hvel.x;
	vel.z = hvel.z;

	if (new Vector2(vel.x, vel.z).Length() == 0f)
	{
	  RemoveState(States.Walking);
	}
  }

  public void Win()
  {
	hud.Win();
	PauseMode = PauseModeEnum.Stop;
  }

  public void Lose()
  {
	hud.Lose();
	PauseMode = PauseModeEnum.Stop;
  }

  public void StopControl()
  {
	var headPlayer = head as ControlledPlayer;
	headPlayer.freeze = true;
	Input.SetMouseMode(Input.MouseMode.Visible);
  }
}
