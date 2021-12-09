using Godot;

public interface IInteractable
{
    [Export] bool Disabled { set; get; }

    void Interact();
    string GetInfo();
}