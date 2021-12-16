using Godot;
using System;
using Pap1.Scripts;

public class GameOver : Control
{
  private Label gameOverLabel;
  public override void _Ready()
  {
	gameOverLabel = GetNode<Label>("CenterContainer/Panel/VBoxContainer/CenterContainer/Label");
  }

  public void SetLabelText(string text)
  {
	gameOverLabel.Text = text;
  }

  private void OnQuitBtnPressed()
  {
	var mainMenu = GetTree().Root.GetChildren()[0] as MainMenu;
	mainMenu.EndGame();
  }
}
