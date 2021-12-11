using Godot;
using System;
using Pap1.Scripts;

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
