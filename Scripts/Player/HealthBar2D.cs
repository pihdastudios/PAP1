using Godot;
using System;

public class HealthBar2D : TextureProgress
{
    private readonly Texture barRed = GD.Load<Texture>("res://Assets/Sprites/Player/healthBar_red.png");
    private readonly Texture barGreen = GD.Load<Texture>("res://Assets/Sprites/Player/healthBar_green.png");
    private readonly Texture barYellow = GD.Load<Texture>("res://Assets/Sprites/Player/healthBar_yellow.png");

    public void UpdateBar(float amount, float full)
    {
        if (amount < full)
            Show();
        TextureProgress_ = barGreen;
        if (amount < 0.75 * full)
            TextureProgress_ = barYellow;
        if (amount < 0.45 * full)
            TextureProgress_ = barRed;
        Value = amount;
    }
}