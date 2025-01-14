using Godot;
using System;

public partial class MasterScript : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Mech mech = new Mech();
		mech.Hull.AttachPart(new Primary(), 1);
		mech.Hull.AttachPart(new Primary(), 2);
        AddChild(mech.Controller);
		mech.Controller.AddChild(ResourceLoader.Load<PackedScene>("res://Scenes/MechCamera.tscn").Instantiate());

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {

	}
}
