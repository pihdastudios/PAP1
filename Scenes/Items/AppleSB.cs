using Godot;
using System;

public class AppleSB : StaticBody, IInteractable
{
    public override void _Ready()
    {
    }

    public bool Disabled { get; set; }

    public void Interact()
    {
        return;
    }

    public string GetInfo()
    {
        return "Get Apple";
    }
}