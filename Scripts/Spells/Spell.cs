using Godot;
using System;

public class Spell : Spatial
{
    private readonly PackedScene projectile = GD.Load<PackedScene>("res://Scenes/Spells/SpellFireball.tscn");
    [Export] public float ProjectileSpeed = 20, MillisBetweenShots = 200;
    private Timer rofTimer;
    private bool canShoot = true;

    public override void _Ready()
    {
        rofTimer = GetNode<Timer>("Timer");
        rofTimer.WaitTime = MillisBetweenShots / 1000;
    }

    public void Shoot()
    {
        if (!canShoot) return;
        // ScSound.get_node("FireSound").play()
        var newProjectile = projectile.Instance<SpellFireball>();
        newProjectile.GlobalTransform = GetNode<Spatial>("Muzzle").GlobalTransform;
        newProjectile.Speed = ProjectileSpeed;
        var sceneRoot = GetTree().Root.GetChildren()[0] as Node;
        sceneRoot?.AddChild(newProjectile);
        canShoot = false;
        rofTimer.Start();
    }

    private void _on_Timer_timeout()
    {
        canShoot = true;
    }
}