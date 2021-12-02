using Godot;
using System;

public class Spell : Spatial
{
	private PackedScene Projectile = GD.Load<PackedScene>("res://Scenes/Spells/spell_fireball.tscn");
	[Export]
	public float projectile_speed = 20, millis_between_shots = 200;
	private Timer rof_timer;
	private bool can_shoot = true;
	public override void _Ready()
	{
		rof_timer = GetNode<Timer>("Timer");
		rof_timer.WaitTime = millis_between_shots / 1000;
	}

	public void shoot(){
		if(can_shoot){
			// ScSound.get_node("FireSound").play()
			var new_projectile = Projectile.Instance<spell_fireball>();
			new_projectile.GlobalTransform = GetNode<Spatial>("Muzzle").GlobalTransform;
			new_projectile.speed = projectile_speed;
			var scene_root = (Node) GetTree().Root.GetChildren()[0];
			scene_root.AddChild(new_projectile);
			can_shoot = false;
			rof_timer.Start();
		}
	}
    private void _on_Timer_timeout()
    {
        can_shoot = true;
    }
}
