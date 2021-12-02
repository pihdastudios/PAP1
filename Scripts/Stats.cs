using Godot;
using System;

public class Stats : Node{
  [Export]
  private float max_HP = 10;
  private float current_HP;
  public override void _Ready()
	{
		current_HP = max_HP;
	}
  public void take_hit(float damage)
	{
		current_HP -= damage;
	
    if(current_HP <= 0)
      die();
	}

  private void die(){
    // emit_signal("died_signal");
  }
}