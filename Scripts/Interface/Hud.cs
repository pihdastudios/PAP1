using Godot;
using System;

public class Hud : CanvasLayer
{
    private Label interactionLbl;
    public override void _Ready()
    {
        interactionLbl = GetNode<Label>("CenterContainer/InteractionLabel");
    }
}