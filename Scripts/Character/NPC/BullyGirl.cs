using Godot;
using System;

public partial class BullyGirl : CharacterBody3D
{
    [Export]
    private AnimationTree _animTree;
    [Export]
    private PointOfInterest _pointFrom;
    [Export]
    private PointOfInterest _pointTo;

    public override void _Ready()
    {
        _animTree = _animTree ?? GetChild<AnimationTree>(2);
    }

    public override void _PhysicsProcess(double delta)
    {
        
    }
}
