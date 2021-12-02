using Godot;
using System;

public class SpellController : Node
{
	private PackedScene startingSpell = GD.Load<PackedScene>("res://Scenes/Spells/Spell.tscn");
	private Spatial hand;
	private Spell equipped_weapon;

	public override void _Ready()
	{
		hand = GetParent().GetNode<Spatial>("Camera/ItemHolder");
		if (startingSpell!=null)
			equip_weapon(startingSpell);
	}
	private void equip_weapon(PackedScene weapon_to_equip){
		if (equipped_weapon!=null)
			equipped_weapon.QueueFree();
		else{
			equipped_weapon = weapon_to_equip.Instance<Spell>();
			hand.AddChild(equipped_weapon);
		}
	}

	public void cast(){
		if(equipped_weapon!=null)
			equipped_weapon.shoot();
	}
}
