using Godot;
using System;

public partial class DemoScene : Spatial
{
	private Position3D player1Pos;
	private Position3D player2Pos;
	private Area finishArea;
	private enum Role {ATTACKER, DEFENDER};
	
	private PackedScene playerScn= GD.Load<PackedScene>("res://Scenes/Player/Player.tscn");
	private PackedScene controlledPlayerScn= GD.Load<PackedScene>("res://Scenes/Player/ControlledPlayer.tscn");
	private PackedScene nonControlledPlayerScn= GD.Load<PackedScene>("res://Scenes/Player/NonControlledPlayer.tscn");


	public override void _Ready()
	{
		var controlledPlayer = controlledPlayerScn.Instance<Spatial>();
		var nonControlledPlayer = nonControlledPlayerScn.Instance<Spatial>();

		var player1 = playerScn.Instance<KinematicBody>();
		player1Pos = GetNode<Position3D>("Player1Pos");
		player1.GlobalTransform = player1Pos.GlobalTransform;

		var player2 = playerScn.Instance<KinematicBody>();
		player2Pos = GetNode<Position3D>("Player2Pos");
		player2.GlobalTransform = player2Pos.GlobalTransform;

		if(Globals.role == (int) Role.ATTACKER){
			player1.Name = GetTree().GetNetworkUniqueId().ToString();
			player1.AddChild(controlledPlayer);
			
			player2.Name = Globals.PeerId.ToString();
			player2.AddChild(nonControlledPlayer);
		}else{
			player1.Name = Globals.PeerId.ToString();
			player1.AddChild(nonControlledPlayer);

			player2.Name = GetTree().GetNetworkUniqueId().ToString();
			player2.AddChild(controlledPlayer);
		}
		AddChild(player1);
		AddChild(player2);

		// finishArea = GetNode <Area>("Area");
	}

	public void OnAreaBodyEntered(Node body)
	{
		
	}
}
