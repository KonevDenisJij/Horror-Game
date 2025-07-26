using Godot;

public partial class Ball : RigidBody3D, IInteractableObject
{
    [Signal]
    public delegate void OnBallPickupEventHandler();

    public string InteractText => "Pick Up";
    public string InteractionSignalName { get; set; }

    public override void _PhysicsProcess(double delta)
    {
        if (PauseMenu.Instance.IsPaused)
        {
            return;
        }

        base._PhysicsProcess(delta);
    }

    public void Interact()
    {
        Freeze = true;
        CollisionLayer = (uint)16;
        CollisionMask = (uint)1;
        Player.Instance.PickUpObject(this);
        EmitSignal(SignalName.OnBallPickup);
    }

    public void ThrowItem(Vector3 direction, float power, Node parent)
    {
        PlaceItem(parent);
        ApplyCentralImpulse(direction * power);
    }

    public void PlaceItem(Node parent)
    {
        Position = new Vector3(0f, 0.6f, -0.61f);
        var prevGlobalPosition = GlobalPosition;
        GetParent().RemoveChild(this);
        parent.AddChild(this);
        Position = prevGlobalPosition;
        Freeze = false;
        CollisionLayer = (uint)4;
        CollisionMask = (uint)15;
    }
}
