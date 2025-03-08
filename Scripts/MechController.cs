using Godot;
using System;
using System.ComponentModel.Design.Serialization;

public partial class MechController : CharacterBody2D
{
	public Mech MechObject;
    private float MoveSpeed = 300f;
	private float RotateSpeed = 180f;
	private float RotorSpeed = 100f;
	private Action MovementMethod;
	//configuration
	private const float Movement_Lerp = 0.25f;
	private const float Backpedal_Multiplier = 0.4f;
	//internal vars
	private float currentHullRotation = 0f;
	private float linearVelocity = 0f;

	public override void _Ready() {
		MoveSpeed = this.MechObject.Chassis.MoveSpeed;
        RotateSpeed = this.MechObject.Chassis.RotateSpeed;
        RotorSpeed = this.MechObject.Chassis.RotorSpeed + this.MechObject.Hull.RotorSpeed;
		switch(this.MechObject.Chassis.Type) {
			case ChassisType.TwoLegs:
				MovementMethod = TwoLegsMovement;
				break;
            case ChassisType.FourLegs:
				MovementMethod = FourLegsMovement;
				break;
			case ChassisType.Treads:
                MovementMethod = TreadsMovement;
                break;
            default:
                MovementMethod = FourLegsMovement;
                break;
        }
	}

	public override void _PhysicsProcess(double delta) {
		//Chassis position
		MovementMethod();

		//Hull orientation
		Vector2 mousePos = GetGlobalMousePosition();
		float targetRotation = Convert.ToSingle(Math.Atan2(mousePos.Y - GlobalPosition.Y, mousePos.X - GlobalPosition.X) + Math.PI*2);
		if (targetRotation > Mathf.Pi*2) targetRotation -= Mathf.Pi*2;
		sbyte rotateDirection;
		if (currentHullRotation > targetRotation) {
			rotateDirection = -1;
		} else {
			rotateDirection = 1;
		}
		float difference = Mathf.Abs(currentHullRotation - targetRotation);
		float increment = Mathf.DegToRad(RotorSpeed) / 60f;
		if (difference > Mathf.Pi) rotateDirection *= -1;
		if (difference < increment) increment = difference;
		currentHullRotation = currentHullRotation + (increment * rotateDirection);

		if (currentHullRotation > Mathf.Pi * 2) {
			currentHullRotation -= Mathf.Pi * 2;
		}
		else if (currentHullRotation < 0) {
			currentHullRotation += Mathf.Pi * 2;
		}
		GetNode<Sprite2D>("HullSprite").Rotation = currentHullRotation - Mathf.Pi/2; //90 deg correction
	}

    // Movement Functions //
	private void TwoLegsMovement() {
		Vector2 moveDirection = Input.GetVector("Move_Left", "Move_Right", "Move_Up", "Move_Down");
        Sprite2D chassisNode = MechObject.CharacterBody.GetNode<Sprite2D>("ChassisSprite");
		//fwd-focused movement
        if (moveDirection.Y != 0) {
			float mult = 1f;
			if (moveDirection.Y > 0) mult = -Backpedal_Multiplier;
            linearVelocity = GlobalHelper.Lerpf(linearVelocity, MoveSpeed * mult, Movement_Lerp);
            Velocity = new Vector2(Mathf.Cos(chassisNode.GlobalRotation), Mathf.Sin(chassisNode.GlobalRotation)) * linearVelocity;
        } else {
            linearVelocity = GlobalHelper.Lerpf(linearVelocity, 0f, Movement_Lerp);
            Velocity = new Vector2(Mathf.Cos(chassisNode.GlobalRotation), Mathf.Sin(chassisNode.GlobalRotation)) * linearVelocity;
        }
		//rotation
		float increment = Mathf.DegToRad(RotateSpeed) / 60f;
		float rotateTo = chassisNode.GlobalRotation + (increment * moveDirection.X);
        chassisNode.GlobalRotation = rotateTo;

        MoveAndSlide();
    }
    private void FourLegsMovement() {
        Vector2 moveDirection = Input.GetVector("Move_Left", "Move_Right", "Move_Up", "Move_Down");
        Velocity = Velocity.Lerp(moveDirection * MoveSpeed, Movement_Lerp);
        MoveAndSlide();
    }
    private void TreadsMovementOld() {
        Vector2 moveDirection = Input.GetVector("Move_Left", "Move_Right", "Move_Up", "Move_Down");
		Sprite2D chassisNode = MechObject.CharacterBody.GetNode<Sprite2D>("ChassisSprite");

        if (moveDirection.Length() > 0) {
			//forward movement TODO: backwards too
			linearVelocity = GlobalHelper.Lerpf(linearVelocity, MoveSpeed, Movement_Lerp);
			Velocity = new Vector2(Mathf.Cos(chassisNode.GlobalRotation), Mathf.Sin(chassisNode.GlobalRotation)) * linearVelocity;
			//rotation
			float targetRotation = Mathf.Atan2(moveDirection.Y, moveDirection.X);
			sbyte rotateDirection;
            if (chassisNode.GlobalRotation > targetRotation) {
                rotateDirection = -1;
            } else {
                rotateDirection = 1;
            }
            float difference = Mathf.Abs(chassisNode.GlobalRotation - targetRotation);
			float increment = Mathf.DegToRad(RotateSpeed) / 60f;
			if (difference > Mathf.Pi) rotateDirection *= -1;
			if (difference < increment) increment = difference;
			float rotateTo = chassisNode.GlobalRotation + (increment * rotateDirection);

			chassisNode.GlobalRotation = rotateTo;
        } else {
			//slow to a stop
            linearVelocity = GlobalHelper.Lerpf(linearVelocity, 0f, Movement_Lerp);
            Velocity = new Vector2(Mathf.Cos(chassisNode.GlobalRotation), Mathf.Sin(chassisNode.GlobalRotation)) * linearVelocity;
        }
			MoveAndSlide();
    }
    private void TreadsMovement() {
        Vector2 moveDirection = Input.GetVector("Move_Left", "Move_Right", "Move_Up", "Move_Down");
        Sprite2D chassisNode = MechObject.CharacterBody.GetNode<Sprite2D>("ChassisSprite");

        if (moveDirection.Length() > 0) {
            float targetRotation = Mathf.Atan2(moveDirection.Y, moveDirection.X);
            sbyte rotateDirection;

            //check if reversing direction is more efficient
            float differencePlus = Mathf.Abs(chassisNode.GlobalRotation + Mathf.Pi - targetRotation);
            float differenceMinus = Mathf.Abs(chassisNode.GlobalRotation - Mathf.Pi - targetRotation);
            if (differencePlus < Mathf.Pi / 2 || differenceMinus < Mathf.Pi / 2) { //checks if reversed is less than 45 deg.
                targetRotation += Mathf.Pi;
                linearVelocity = GlobalHelper.Lerpf(linearVelocity, -MoveSpeed, Movement_Lerp);
                //todo: fix logical error regarding 45 deg (Pi/2)
            }
            else {
                linearVelocity = GlobalHelper.Lerpf(linearVelocity, MoveSpeed, Movement_Lerp);
            }
            //linear movement
            Velocity = new Vector2(Mathf.Cos(chassisNode.GlobalRotation), Mathf.Sin(chassisNode.GlobalRotation)) * linearVelocity;

            //rotation
            if (chassisNode.GlobalRotation > targetRotation) {
                rotateDirection = -1;
            }
            else {
                rotateDirection = 1;
            }
            float difference = Mathf.Abs(chassisNode.GlobalRotation - targetRotation);
            float increment = Mathf.DegToRad(RotateSpeed) / 60f;
            if (difference > Mathf.Pi) rotateDirection *= -1;
            if (difference < increment) increment = difference;
            float rotateTo = chassisNode.GlobalRotation + (increment * rotateDirection);

            chassisNode.GlobalRotation = rotateTo;

            GD.Print(Mathf.Round(chassisNode.GlobalRotationDegrees) + " -> " + Mathf.RadToDeg(targetRotation));
        }
        else {
            //slow to a stop
            linearVelocity = GlobalHelper.Lerpf(linearVelocity, 0f, Movement_Lerp);
            Velocity = new Vector2(Mathf.Cos(chassisNode.GlobalRotation), Mathf.Sin(chassisNode.GlobalRotation)) * linearVelocity;
        }
        MoveAndSlide();
    }
}