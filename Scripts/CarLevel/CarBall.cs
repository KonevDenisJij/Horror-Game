using Godot;
using System;

public partial class CarBall : Node3D
{
    [Export]
    private RigidBody3D _ball;
    [Export]
    private Node3D _carMesh;
    [Export]
    private RayCast3D _groundRay;
    [Export]
    private Node3D _cameraPivot;
    [Export]
    private Camera3D _camera;

    [Export]
    private Vector3 _sphereOffset = new Vector3(0, -1, 0);
    [Export]
    private float _acceleration = 50;
    [Export]
    private float _steering = 0.4f;
    [Export]
    private float _turnSpeed = 5;
    [Export]
    private float _turnStopLimit = 0.75f;

    private Vector3 _cameraLookAt;
    private float _speedInput;
    private float _rotateInput;

    public override void _Ready()
    {
        _groundRay.AddException(_ball);
        _cameraLookAt = GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        _carMesh.Position = _ball.Position + _sphereOffset;
        _ball.ApplyCentralForce(-_carMesh.GlobalTransform.Basis.Z * _speedInput);

        _cameraPivot.GlobalPosition = _cameraPivot.GlobalPosition.Lerp(_carMesh.GlobalPosition, (float)delta * 20);
        _cameraPivot.Rotation = _cameraPivot.Rotation.Lerp(_carMesh.Rotation, (float)delta * _turnSpeed);
        _cameraLookAt = _cameraLookAt.Lerp(_ball.GlobalPosition + _ball.LinearVelocity, (float)delta * _turnSpeed);
        _camera.LookAt(_cameraLookAt);
    }

    public override void _Process(double delta)
    {
        if (!_groundRay.IsColliding())
        {
            GD.Print("Not on Ground");
            return;
        }

        _rotateInput = Input.GetAxis("move_right", "move_left") * _steering;
        _speedInput = Input.GetAxis("move_backward", "move_forward") * _acceleration;

        if (_ball.LinearVelocity.Length() > _turnStopLimit)
        {
            var newBasis = _carMesh.GlobalTransform.Basis.Rotated(_carMesh.GlobalTransform.Basis.Y, _rotateInput).Orthonormalized();
            _carMesh.Basis = _carMesh.Basis.Orthonormalized().Slerp(newBasis, _turnSpeed * (float)delta);

            _carMesh.GlobalTransform = _carMesh.GlobalTransform.Orthonormalized();
        }
    }
}
