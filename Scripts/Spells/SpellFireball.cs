using Godot;
using System;
using Pap1.Scripts;

public class SpellFireball : Spatial
{
	[Export]
	public float Speed = 70, Damage = 1;
	private float KILL_TIME = 2, timer = 0;

	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(float delta)
	{
		var forwardDirection = GlobalTransform.basis.z.Normalized();
		GlobalTranslate(forwardDirection * Speed * delta);
		
		timer += delta;
		if(timer > KILL_TIME)
			QueueFree();
	}

	private void _on_Area_body_entered(Node body)
	{
		QueueFree();
		if (!body.HasNode("Stats")) return;
		var statsNode = body.FindNode("Stats") as Stats;
		statsNode?.TakeHit(Damage);
	}
}
