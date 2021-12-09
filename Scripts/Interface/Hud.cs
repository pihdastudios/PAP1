using Godot;
using System;

public class Hud : CanvasLayer
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

    private void HideInterface()
    {
        center.Hide();
        items.Hide();
        gameOver.Show();
    }
    public void Win()
    {
        gameOver.SetLabelText("Winner");
        HideInterface();
    }
    
    public void Lose()
    {
        gameOver.SetLabelText("Loser");
        HideInterface();
    }
}