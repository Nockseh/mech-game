using Godot;
using System;
//doesn't use our custom namespace yet

public partial class CameraScript : Camera2D {

	private CharacterBody2D MechBody;

	public override void _Ready() {
		MechBody = GetParent<CharacterBody2D>();
	}

	public override void _Process(double delta) {
		GlobalPosition = MechBody.Position;
	}
}
