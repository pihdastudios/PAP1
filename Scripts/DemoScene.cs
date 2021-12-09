using Godot;
using System;

public partial class DemoScene : Spatial
{
    private Position3D player1Pos;
    private Position3D player2Pos;

    private Player player1;
    private Player player2;

    [Signal]
    public delegate void GameOverSignal(Role winner);

    public enum Role
    {
        Attacker,
        Defender
    };

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

        // TODO: Add role changer
        // if (Globals.role == (int) Role.Attacker)
        // {
        player1.Name = GetTree().GetNetworkUniqueId().ToString();
        player1.AddChild(controlledPlayer);

        player2.Name = Globals.PeerId.ToString();
        player2.AddChild(nonControlledPlayer);
        // }
        // else
        // {
        //     player1.Name = Globals.PeerId.ToString();
        //     player1.AddChild(nonControlledPlayer);
        //
        //     player2.Name = GetTree().GetNetworkUniqueId().ToString();
        //     player2.AddChild(controlledPlayer);
        // }

        AddChild(player1);
        AddChild(player2);
    }

    public void OnAreaBodyEntered(Node body)
    {
        if (body?.Get("IsPlayerOne") != null && (bool) body.Get("IsPlayerOne"))
        {
            EmitSignal(nameof(GameOverSignal), Role.Attacker);
        }
    }

    public void OnGameOver(Role winner)
    {
        if (winner == Role.Attacker)
        {
            player1.Win();
            player2.Lose();
        }
        else
        {
            player1.Lose();
            player2.Win();
        }
    }
}