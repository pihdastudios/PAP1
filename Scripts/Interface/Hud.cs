using Godot;
using System;

public class Hud : CanvasLayer
{
    private Label interactionLbl;
    private Label gameOverLbl;
    public override void _Ready()
    {
        interactionLbl = GetNode<Label>("CenterContainer/VBoxContainer/InteractionLabel");
        gameOverLbl = GetNode<Label>("CenterContainer/VBoxContainer/GameOverLabel");
    }

    public void Win()
    {
        gameOverLbl.Text = "You Win";
    }
    
    public void Loose()
    {
        gameOverLbl.Text = "You Lost";
    }
}