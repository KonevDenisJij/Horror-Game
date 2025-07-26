using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class PointsHolder : Node3D
{
    public static PointsHolder Instance => _instance;
    private static PointsHolder _instance;

    public List<PointOfInterest> Points => _points;
    private List<PointOfInterest> _points;

    public override void _Ready()
    {
        if (_instance != null)
        {
            GD.PrintErr("We already have PointsHolder instance");
            Free();
            return;
        }

        _instance = this;
        _points = new List<PointOfInterest>();

        base._Ready();
    }

    public PointOfInterest GetPoint(string id)
    {
        return _points.Single(a => a.Id == id);
    }
}