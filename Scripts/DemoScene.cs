using Godot;
using System;

public partial class DemoScene : Spatial
{
	private Position3D player1Pos;
	private Position3D player2Pos;
	private enum Role {ATTACKER, DEFENDER};
	
	private PackedScene playerScn= GD.Load<PackedScene>("res://Scenes/Player/Player.tscn");
	private PackedScene controlledPlayerScn= GD.Load<PackedScene>("res://Scenes/Player/ControlledPlayer.tscn");
	private PackedScene nonControlledPlayerScn= GD.Load<PackedScene>("res://Scenes/Player/NonControlledPlayer.tscn");


	public override void _Ready()
	{
		var controlledPlayer = controlledPlayerScn.Instance<Spatial>();
		var nonControlledPlayer = nonControlledPlayerScn.Instance<Spatial>();

		var player1 = playerScn.Instance<KinematicBody>();
		player1.Name = GetTree().GetNetworkUniqueId().ToString();
		player1Pos = GetNode<Position3D>("Player1Pos");
		player1.GlobalTransform = player1Pos.GlobalTransform;

		var player2 = playerScn.Instance<KinematicBody>();
		player2.Name = Globals.Player2Id.ToString();
		player2Pos = GetNode<Position3D>("Player2Pos");
		player2.GlobalTransform = player2Pos.GlobalTransform;

		if(Globals.role == (int) Role.ATTACKER){
			player1.AddChild(controlledPlayer);			
			player2.AddChild(nonControlledPlayer);
		}else{
			player1.AddChild(nonControlledPlayer);
			player2.AddChild(controlledPlayer);
		}
		AddChild(player1);
		AddChild(player2);
	}
}
