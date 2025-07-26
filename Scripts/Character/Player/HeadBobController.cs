using Godot;
using System;

public partial class HeadBobController : Node3D
{
    /*private bool _enabled = true;

    [Export]
    private float _amplitude = 0.01f;
    [Export]
    private float _frequency = 12f;
    [Export]
    private Camera3D _camera;

    private float _toggleSpeed = 3f;
    private float _resetTime;
    private Vector3 _startPos;
    private Player _player;

    public override void _Ready()
    {
        _player = GetParent().GetParent<Player>();
        _startPos = _camera.Position;
    }

    public override void _Process(double delta)
    {
        if (!_enabled)
        {
            return;
        }

        CheckMotion();
        ResetPosition((float)delta);
    }

    private void CheckMotion()
    {
        if (_player.Velocity.Length() < _toggleSpeed || !_player.IsOnFloor())
        {
            return;
        }

        PlayMotion(FootStepMotion());
    }

    private Vector3 FootStepMotion()
    {
        _resetTime = 0f;

        var pos = Vector3.Zero;
        var time = Time.GetTicksMsec() / 100 * _frequency;
        pos.Y = Mathf.Sin(time) * _amplitude;
        return pos;
    }

    private void PlayMotion(Vector3 motion)
    {
        _camera.GlobalPosition += motion;
    }

    private void ResetPosition(float delta)
    {
        if (_camera.Position == _startPos)
        {
            return;
        }
        _resetTime += delta * 2f;

        _camera.Position = _camera.Position.Lerp(_startPos, _resetTime);
    }*/
}
