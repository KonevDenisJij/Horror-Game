using Godot;

public partial class Door : Node3D
{
    [Export]
    private NavigationRegion3D _navRegionParent;
    [Export]
    private NavigationRegion3D _navRegionBottom;
    [Export]
    private PointOfInterest _entrance;
    [Export]
    private PointOfInterest _exit;

    public DoorHandle Handle => _handle;
    private DoorHandle _handle;
    public PointOfInterest[] Places => new[] { _entrance, _exit };

    public bool IsOpen => Handle.IsOpen;

    public override void _Ready()
    {
        if (_navRegionParent == null || _navRegionBottom == null)
        {
            GD.PrintErr($"Select NavRegions for door {Name}");
            return;
        }

        _handle = GetChild(1) as DoorHandle;

        PathFindHelper.Doors.Add(this);
    }

    public void Open()
    {
        if (IsOpen)
        {
            return;
        }

        _handle.Interact();
    }

    public void OnDoorStateChange(string animationName)
    {
        if (animationName == "Open")
        {
            _navRegionBottom.Enabled = true;
        }
        else
        {
            _navRegionBottom.Enabled = false;
        }

        _navRegionParent.BakeNavigationMesh();
    }
}
