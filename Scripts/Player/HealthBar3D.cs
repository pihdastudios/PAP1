using Godot;
using System;

public class HealthBar3D : Sprite3D
{
  public HealthBar2D bar;

  public override void _Ready()
  {
	bar = GetNode<HealthBar2D>("Viewport/HealthBar2D");
	Texture = GetNode<Viewport>("Viewport").GetTexture();
  }

  public void Update(float amount, float full)
  {
	bar.UpdateBar(amount, full);
  }
}
