using Godot;
using System;
using System.Linq;

public partial class Mirror : Node3D
{
    [Export]
    private Color _mirrorColor = new Color(0.9f, 0.97f, 0.94f);
    [Export]
    private float _mirrorDistortion = 1;

    [Export]
    private Camera3D _playerCam;

    private Camera3D _cam;
    private MeshInstance3D _mirror;
    private Viewport _viewport;
    private VisibleOnScreenNotifier3D _visibleOnScreenNotifier;

    private bool isVisible = false;

    public override void _Ready()
    {
        _visibleOnScreenNotifier = GetChild<VisibleOnScreenNotifier3D>(0);
        _mirror = _visibleOnScreenNotifier.GetChild<MeshInstance3D>(0);
        _viewport = _mirror.GetChild<Viewport>(0);

        _cam = _viewport.GetChild<Camera3D>(0);
        var shaderMat = (_mirror.GetActiveMaterial(0) as ShaderMaterial);
        shaderMat.SetShaderParameter("tint", _mirrorColor);
        shaderMat.SetShaderParameter("mirror_tex", _viewport.GetTexture());
        shaderMat.SetShaderParameter("distort_strength", _mirrorDistortion);
    }

    public override void _PhysicsProcess(double delta)
    {
        /*if (!isVisible)
        {
            return;
        }

        var spaceState = GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(GlobalPosition, Player.Instance.GlobalPosition + Vector3.Up, 1);
        var result = spaceState.IntersectRay(query);

        isVisible = result.Count == 0;*/
    }

    public override void _Process(double delta)
    {
        // Skip rendering the view from this portal if player is not looking at the linked portal
        if (!isVisible)
        {
            //_visibleOnScreenNotifier.Visible = false;
            return;
        }

        _mirror.Visible = true;

        var MirrorNormal = _mirror.GlobalTransform.Basis.Z;

        var MirrorTransform = Mirror_transform(MirrorNormal, _mirror.GlobalTransform.Origin);

        _cam.GlobalTransform = MirrorTransform * _playerCam.GlobalTransform;

        _cam.GlobalTransform = _cam.GlobalTransform.LookingAt(
                _cam.GlobalTransform.Origin / 2 + _playerCam.GlobalTransform.Origin / 2,
                _mirror.GlobalTransform.Basis.Y
            ); ;

        var cam2mirror_offset = _mirror.GlobalTransform.Origin - _cam.GlobalTransform.Origin;

        var near = Mathf.Abs((cam2mirror_offset).Dot(MirrorNormal));
        near += 0.05f;

        var cam2mirror_camlocal = _cam.GlobalTransform.Basis.Inverse() * cam2mirror_offset;

        var frostum_offset = new Vector2(cam2mirror_camlocal.X, cam2mirror_camlocal.Y);

        _cam.SetFrustum(Scale.X, frostum_offset, near, 10000);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey key && key.IsPressed() && key.Keycode == Key.P)
        {
            GD.Print($"Portal: {Name}; Cam Pos: {_cam.GlobalTransform}; Player cam: {_playerCam.GlobalPosition}");
        }
    }

    private Transform3D Mirror_transform(Vector3 n, Vector3 d)
    {
        var basisX = new Vector3(1.0f, 0, 0) - 2 * new Vector3(n.X * n.X, n.X * n.Y, n.X * n.Z);
        var basisY = new Vector3(0, 1.0f, 0) - 2 * new Vector3(n.Y * n.X, n.Y * n.Y, n.Y * n.Z);
        var basisZ = new Vector3(0, 0, 1.0f) - 2 * new Vector3(n.Z * n.X, n.Z * n.Y, n.Z * n.Z);
        var offset = 2 * n.Dot(d) * n;

        return new Transform3D(new Basis(basisX, basisY, basisZ), offset);
    }

    private void OnScreenEntered()
    {
        isVisible = true;
    }

    private void OnScreenExited()
    {
        isVisible = false;
    }
}