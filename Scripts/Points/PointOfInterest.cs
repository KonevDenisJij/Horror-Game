using Godot;

public partial class PointOfInterest : Marker3D
{
    [Export]
    private string _id;
    [Export]
    private PlaceEnum _place;

    public string Id => _id;
    public PlaceEnum Place => _place;

    public override void _Ready()
    {
        if (_id == string.Empty || _id == null)
        {
            _id = GetParent().Name + Name;
        }

        PointsHolder.Instance.Points.Add(this);
    }
}
