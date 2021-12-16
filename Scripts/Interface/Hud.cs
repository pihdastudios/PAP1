using Godot;
using System;

public class Hud : Control
{
  private Label interactionLbl;
  private CenterContainer center;
  private HBoxContainer items;
  private GameOver gameOver;
  private ProgressBar progressBar;
  private Label timer;
  private DemoScene demoScene;
  private float time = 120;

  public override void _Ready()
  {
	interactionLbl = GetNode<Label>("Center/VBoxContainer/InteractionLabel");
	center = GetNode<CenterContainer>("Center");
	items = GetNode<HBoxContainer>("Items");
	gameOver = GetNode<GameOver>("GameOver");
	demoScene = GetTree().Root.GetNode<DemoScene>("DemoScene");
	progressBar = GetNode<ProgressBar>("Items/HBoxContainer/ProgressBar");
	timer = GetNode<Label>("Label");
	if (Globals.CurrentRole == Globals.Role.Defender)
	{
	  progressBar.Hide();
	  GetNode<Label>("Items/HBoxContainer/Label").Hide();
	}
  }

  public override void _Process(float delta)
  {
	if (time <= 0 || demoScene.isGameOver)
	{
	  timer.Text = "";
	  if (GetParent().GetNode<Spatial>("Head").HasMethod("IsPlayer"))
		GetTree().Root.GetNode<DemoScene>("DemoScene").EmitSignal("GameOverSignal", Globals.Role.Defender);
	}
	else
	{
	  time -= delta;
	  timer.Text = Math.Floor(time).ToString();
	}
  }

  public void Win()
  {
	gameOver.SetLabelText("Winner");
	gameOver.Show();
	GetTree().CallGroup("HUDElement", "hide");
  }

  public void Lose()
  {
	gameOver.SetLabelText("Loser");
	gameOver.Show();
	GetTree().CallGroup("HUDElement", "hide");
  }

  public void updateHealth(double amount)
  {
	progressBar.Value = amount;
  }

  public void SetInteractionText(string text)
  {
	interactionLbl.Text = text;
  }
}
