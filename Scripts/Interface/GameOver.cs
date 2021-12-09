using Godot;
using System;

public class GameOver : Control
{
    private Label gameOverLabel;
    public override void _Ready()
    {
        gameOverLabel = GetNode<Label>("CenterContainer/VBoxContainer/Label");
    }

    public void SetLabelText(string text)
    {
        gameOverLabel.Text = text;
    }
}
