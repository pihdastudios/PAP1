using Godot;
using System;

public class Hud : Control
{
    private Label interactionLbl;
    private CenterContainer center;
    private HBoxContainer items;
    private GameOver gameOver;
    
    public override void _Ready()
    {
        interactionLbl = GetNode<Label>("Center/VBoxContainer/InteractionLabel");
        center = GetNode<CenterContainer>("Center");
        items = GetNode<HBoxContainer>("Items");
        gameOver = GetNode<GameOver>("GameOver");
    }
    
    public void Win()
    {
        GD.Print("Win");
        gameOver.SetLabelText("Winner");
        gameOver.Show();
        GetTree().CallGroup("HUDElement", "hide");
    }
    
    public void Lose()
    {
        GD.Print("Lose");
        gameOver.SetLabelText("Loser");
        gameOver.Show();
        GetTree().CallGroup("HUDElement", "hide");
    }
}