using Godot;
using System;

public partial class DemoScene : Spatial
{
  private Position3D player1Pos;
  private Position3D player2Pos;

  private Player player1;
  private Player player2;

  [Signal]
  public delegate void GameOverSignal(Globals.Role winner);

  private readonly PackedScene playerScn = GD.Load<PackedScene>("res://Scenes/Player/Player.tscn");

  private readonly PackedScene controlledPlayerScn =
  GD.Load<PackedScene>("res://Scenes/Player/ControlledPlayer.tscn");

  private readonly PackedScene nonControlledPlayerScn =
  GD.Load<PackedScene>("res://Scenes/Player/NonControlledPlayer.tscn");


  public override void _Ready()
  {
	var controlledPlayer = controlledPlayerScn.Instance<Spatial>();
	var nonControlledPlayer = nonControlledPlayerScn.Instance<Spatial>();

	player1 = playerScn.Instance<Player>();
	player1.IsPlayerOne = true;
	player1Pos = GetNode<Position3D>("Player1Pos");
	player1.GlobalTransform = player1Pos.GlobalTransform;

	player2 = playerScn.Instance<Player>();
	player2Pos = GetNode<Position3D>("Player2Pos");
	player2.GlobalTransform = player2Pos.GlobalTransform;
	player2.GetNode<HealthBar3D>("HealthBar3D").Visible = false;

	// TODO: Add role changer
	if (Globals.CurrentRole == Globals.Role.Attacker)
	{
	  player1.Name = GetTree().GetNetworkUniqueId().ToString();
	  player1.AddChild(controlledPlayer);

	  player2.Name = Globals.PeerId.ToString();
	  player2.AddChild(nonControlledPlayer);
	}
	else
	{
	  player1.Name = Globals.PeerId.ToString();
	  player1.AddChild(nonControlledPlayer);

	  player2.Name = GetTree().GetNetworkUniqueId().ToString();
	  player2.AddChild(controlledPlayer);
	}

	AddChild(player1);
	AddChild(player2);
  }

  public void OnAreaBodyEntered(Node body)
  {
	if (body?.Get("IsPlayerOne") != null && (bool)body.Get("IsPlayerOne"))
	{
	  EmitSignal(nameof(GameOverSignal), Globals.Role.Attacker);
	}
  }

  public void OnGameOver(Globals.Role winner)
  {
	switch (winner)
	{
	  case Globals.Role.Attacker:
		player1.Win();
		player2.Lose();
		break;
	  case Globals.Role.Defender:
		player1.Lose();
		player2.Win();
		break;
	  default:
		throw new ArgumentOutOfRangeException(nameof(winner), winner, null);
	}

	switch (Globals.CurrentRole)
	{
	  case Globals.Role.Attacker:
		player1.StopControl();
		break;
	  case Globals.Role.Defender:
		player2.StopControl();
		break;
	  default:
		throw new ArgumentOutOfRangeException();
	}
  }
}
