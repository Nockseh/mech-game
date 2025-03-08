using Godot;
using System;

public partial class MasterScript : Node {
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        Mech mech = new Mech(new Chassis("PlaceholderTreads"), new Hull());
        mech.Hull.AttachPart(new Primary(), 1);
        mech.Hull.AttachPart(new Primary(), 2);
        GlobalHelper.AssignPlayerToMech((Node2D)FindChild("LevelScene"), mech);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {

    }
}
