using Godot;
using System;
using MechGame;
using static MechGame.HelperClass; //this allows us to freely use custom functions like AssignPlayerToMech()

public partial class MasterScript : Node {
    public override void _Ready() {
        Mech mech = new Mech(new PlaceholderTreads(), new PlaceholderHull());
        mech.Hull.AttachPart(new Cannon(), 1);
        mech.Hull.AttachPart(new Cannon(), 2);
        AssignPlayerToMech((Node2D)FindChild("LevelScene"), mech);
    }

    public override void _Process(double delta) {

    }
}
