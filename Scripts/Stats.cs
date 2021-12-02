using Godot;
using System;

public class Stats : Node{
  [Export]
  private float maxHp = 10;
  private float currentHp;
  public override void _Ready()
	{
		currentHp = maxHp;
	}
  public void take_hit(float damage)
	{
		currentHp -= damage;
	
    if(currentHp <= 0)
      Die();
	}

  public static void Die(){
    // emit_signal("died_signal");
  }
}