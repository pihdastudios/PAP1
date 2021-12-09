using Godot;
using System;

public class SpellController : Node
{
    private readonly PackedScene startingSpell = GD.Load<PackedScene>("res://Scenes/Spells/Spell.tscn");
    private Spatial hand;
    private Spell equippedWeapon;

    public override void _Ready()
    {
        hand = GetParent().GetNode<Spatial>("Camera/ItemHolder");
        if (startingSpell != null)
            EquipWeapon(startingSpell);
    }

    private void EquipWeapon(PackedScene weaponToEquip)
    {
        if (equippedWeapon != null)
            equippedWeapon.QueueFree();
        else
        {
            equippedWeapon = weaponToEquip.Instance<Spell>();
            hand.AddChild(equippedWeapon);
        }
    }

    public void Cast()
    {
        equippedWeapon?.Shoot();
    }
}