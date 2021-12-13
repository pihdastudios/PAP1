using Godot;
using System;
using Pap1.Scripts;

public class SpellApple : Spatial
{
    [Export] public float Speed = 70, Damage = 20;
    private float KILL_TIME = 2, timer = 0;

    public override void _Ready()
    {
        Scale = new Vector3(2f, 1.696f, 2f);
    }

    public override void _PhysicsProcess(float delta)
    {
        var forwardDirection = GlobalTransform.basis.z.Normalized();
        GlobalTranslate(forwardDirection * Speed * delta);

        timer += delta;
        if (timer > KILL_TIME)
            QueueFree();
    }

    private void _on_Area_body_entered(Node body)
    {
        QueueFree();
        if (!body.HasNode("HealthBar3D")) return;
        var player = body as Player;
        player?.TakeDamage(Damage);
    }
}