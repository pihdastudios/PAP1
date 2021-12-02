using Godot;
using System;
using Pap1.Scripts;

public class spell_fireball : Spatial
{
	[Export]
	public float speed = 70, damage = 1;
	private float KILL_TIME = 2, timer = 0;

	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(float delta)
	{
		var forward_direction = GlobalTransform.basis.z.Normalized();
		GlobalTranslate(forward_direction * speed * delta);
		
		timer += delta;
		if(timer > KILL_TIME)
			QueueFree();
	}

	private void _on_Area_body_entered(Node body)
	{
		QueueFree();
		if(body.HasNode("Stats")){
			Stats stats_node = (Stats) body.FindNode("Stats");
			stats_node.take_hit(damage);
		}
	}
}
