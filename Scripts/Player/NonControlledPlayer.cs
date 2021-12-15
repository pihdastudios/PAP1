using Godot;
using System;
using Pap1.Scripts;

public class NonControlledPlayer : Spatial
{
	private Player character;
	private Hud hud;

	public override void _Ready()
	{
		character = GetParent<Player>();
		hud = character.GetNode<Hud>("Hud");
		hud.Hide();
	}

	public override void _PhysicsProcess(float delta)
	{
	}
}
