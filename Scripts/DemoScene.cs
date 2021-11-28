using Godot;
using System;

public partial class DemoScene : Spatial
{
    private Position3D player1Pos;
    private Position3D player2Pos;
    
    private PackedScene playerScn= GD.Load<PackedScene>("res://Scenes/Player/Player.tscn");


    public override void _Ready()
    {
        player1Pos = GetNode<Position3D>("Player1Pos");
        player2Pos = GetNode<Position3D>("Player2Pos");
        var player1 = playerScn.Instance<KinematicBody>();
        player1.Name = GetTree().GetNetworkUniqueId().ToString();
        AddChild(player1);
        player1.GlobalTransform = player1Pos.GlobalTransform;
        
        var player2 = playerScn.Instance<KinematicBody>();
        player2.Name = Globals.Player2Id.ToString();
        AddChild(player2);
        player2.GlobalTransform = player2Pos.GlobalTransform;
    }
}
