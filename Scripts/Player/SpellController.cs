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
            equip_weapon(startingSpell);
    }

    private void equip_weapon(PackedScene weaponToEquip)
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