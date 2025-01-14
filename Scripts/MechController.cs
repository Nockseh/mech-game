using Godot;
using System;

public partial class MechController : CharacterBody2D
{
	public Mech MechObject;
	//config
	public const float Move_Speed = 300.0f;
	public const float Movement_Lerp = 0.25f;
	//vars


    public override void _Ready() {

    }

    public override void _PhysicsProcess(double delta) {
		//Chassis position
		Vector2 direction = Input.GetVector("Move_Left", "Move_Right", "Move_Up", "Move_Down");
		Velocity = Velocity.Lerp(direction * Move_Speed, Movement_Lerp);
		MoveAndSlide();

		//Hull orientation
		Vector2 mousePos = GetGlobalMousePosition();
		float targetRotation = Convert.ToSingle(Math.Atan2(mousePos.Y - GlobalPosition.Y, mousePos.X - GlobalPosition.X) - Math.PI / 2);
		//TODO: linearly interpolate based on hull's rotor speed
        GetNode<Sprite2D>("HullSprite").Rotation = targetRotation;
    }
}