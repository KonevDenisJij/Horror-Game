using Godot;
using System;

public partial class PlaceArea : Area3D
{
    [Export]
    private PlaceEnum _place;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is CharacterBase character)
        {
            character.SetCurrentPlace(_place);
        }
    }
}
