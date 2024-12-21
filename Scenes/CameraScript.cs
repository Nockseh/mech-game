using Godot;
using System;

public partial class CameraScript : Camera2D {

	private CharacterBody2D MechBody;

	public override void _Ready() {
		MechBody = GetNode<CharacterBody2D>("%MechBody");
	}

	public override void _Process(double delta) {
		Position = MechBody.Position;
	}
}
