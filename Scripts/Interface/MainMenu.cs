using Godot;
using System;

public class MainMenu : Control
{
    public MainMenu()
    {
    }

    public void OnJoinBtnPressed()
    {
        GetTree().ChangeScene("res://Scenes/DemoScene.tscn");
    }
}
