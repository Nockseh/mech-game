using Godot;
using System;

public partial class MechMovement : CharacterBody2D
{
	//config
	public const float Move_Speed = 300.0f;
	public const float Movement_Lerp = 0.25f;

	public override void _PhysicsProcess(double delta) {

		Vector2 direction = Input.GetVector("Move_Left", "Move_Right", "Move_Up", "Move_Down");

		Velocity = Velocity.Lerp(direction * Move_Speed, Movement_Lerp);

		MoveAndSlide();
	}
}