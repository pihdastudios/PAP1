using Godot;
using System;
using GodotOnReady.Attributes;

public partial class DemoScene : Spatial
{
    [OnReadyGet("Player1Pos")] private Position3D player1Pos;
    [OnReadyGet("Player2Pos")] private Position3D player2Pos;
    
    private PackedScene player1Scn = GD.Load<PackedScene>("res://Scenes/Player/Player.tscn");
    private PackedScene player2Scn = GD.Load<PackedScene>("res://Scenes/Player/Player.tscn");


    public override void _Ready()
    {
        var player1 = player1Scn.Instance<KinematicBody>();
        player1.Name = GetTree().GetNetworkUniqueId().ToString();
        AddChild(player1);
        player1.GlobalTransform = player1Pos.GlobalTransform;
        
        var player2 = player2Scn.Instance<KinematicBody>();
        player2.Name = Globals.Player2Id.ToString();
        AddChild(player2);
        player2.GlobalTransform = player2Pos.GlobalTransform;
    }
}
