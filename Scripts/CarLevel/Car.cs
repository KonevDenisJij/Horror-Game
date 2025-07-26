using Godot;
using System;

public partial class Car : VehicleBody3D
{
    private float MAX_STEER = 0.5f;
    [Export]
    private float MAX_RPM = 500f;
    [Export]
    private float ENGINE_POWER = 300f;

    [Export]
    private Node3D _cameraPivot;
    [Export]
    private Camera3D _camera;

    [Export]
    private VehicleWheel3D _backLeftWheel;
    [Export]
    private VehicleWheel3D _backRightWheel;

    private Vector3 _cameraLookAt;
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        _cameraLookAt = GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        Steering = Mathf.Lerp(Steering, Input.GetAxis("move_right", "move_left") * MAX_STEER, (float)delta * 5);
        var acceleration = Input.GetAxis("move_backward", "move_forward");
        var rpm = _backLeftWheel.GetRpm();
        _backLeftWheel.EngineForce = acceleration * ENGINE_POWER * (1 - rpm / MAX_RPM);
        rpm = _backRightWheel.GetRpm();
        _backRightWheel.EngineForce = acceleration * ENGINE_POWER * (1 - rpm / MAX_RPM);

        _cameraPivot.GlobalPosition = _cameraPivot.GlobalPosition.Lerp(GlobalPosition, (float)delta * 20);
        _cameraPivot.Transform = _cameraPivot.Transform.InterpolateWith(Transform, (float)delta * 5);
        _cameraLookAt = _cameraLookAt.Lerp(GlobalPosition + LinearVelocity, (float)delta * 5);
        _camera.LookAt(_cameraLookAt);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("open_menu"))
        {
            MouseModeController.Instance.SetMouseMode();
        }
    }
}
