using Godot;
using System;
using System.Drawing;
using System.Linq;

public partial class CarCustom : RigidBody3D
{
    #region Refrences

    [ExportGroup("References")]
    [Export]
    private RayCast3D[] _rayPoints;
    [Export]
    private uint _collisionMask = 1;
    [Export]
    private Node3D _accelerationPoint;
    [Export]
    private Node3D _cameraPivot;
    [Export]
    private Camera3D _camera;
    [Export]
    private Camera3D _reverseCamera;
    [Export]
    private Node3D[] _tires;
    [Export]
    private Node3D[] _frontTiresParent;
    [Export]
    private GpuParticles3D[] _smokeParticles;
    [Export]
    private GpuParticles3D[] _trailParticles;

    #endregion

    #region Suspension Settings

    [ExportGroup("Suspension Settings")]
    [Export]
    private float _springStiffness;
    [Export]
    private float _damperStiffness;
    [Export]
    private float _restLength;
    [Export]
    private float _springTravel;
    [Export]
    private float _wheelRadius;

    #endregion

    #region Car Settings

    [ExportGroup("Car Settings")]
    [Export]
    private float _acceleration = 25f;
    [Export]
    private float _deceleration = 10f;
    [Export]
    private float _maxSpeed = 100f;
    [Export]
    private float _steerStrength = 15f;
    [Export]
    private Curve _turningCurve;
    [Export]
    private float _dragCoefficient = 1f;
    [Export]
    private float _breakingDeceleration = 100f;
    [Export]
    private float _breakingDragCoefficient = 0.5f;

    #endregion

    #region Visuals Settings

    [Export]
    private float _tireRotSpeed = 53f;
    [Export]
    private float _maxSteeringAngle = 30f;

    #endregion

    private Vector3 _currentCarLocalVelocity = Vector3.Zero;
    private float _carVelocityRatio = 0;

    private bool[] _wheelIsGrounded = new bool[4];
    private bool _isGrounded = false;

    private float _moveInput = 0;
    private float _steerInput = 0;

    private Vector3 _cameraLookAt;

    public override void _Ready()
    {
        _cameraLookAt = GlobalPosition;

        foreach (var rayPoint in _rayPoints)
        {
            rayPoint.TargetPosition = new Vector3(0, -(_restLength + _wheelRadius), 0);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
        Movement();
        CameraFollow((float)delta);
        Visuals((float)delta);
    }

    public override void _Process(double delta)
    {
        GetPlayerInput();
    }

    #region Suspension

    private void Suspension()
    {
        for (int i = 0; i <_rayPoints.Length; i++)
        {
            var rayPoint = _rayPoints[i];

            if (rayPoint.GetCollider() != null)
            {
                _wheelIsGrounded[i] = true;
                rayPoint.ForceRaycastUpdate();
                var collisionPoint = rayPoint.GetCollisionPoint();
                var hitDistance = collisionPoint.DistanceTo(rayPoint.GlobalPosition);

                var currentSpringLength = hitDistance - _wheelRadius;
                var springCompression = (_restLength - currentSpringLength) / _springTravel;

                var springVelocity = GetPointVelocity(rayPoint.GlobalPosition).Dot(rayPoint.GlobalBasis.Y);
                var dampForce = _damperStiffness * springVelocity;
                var springForce = _springStiffness * springCompression;

                var netForce = springForce - dampForce;

                /*if (i == 0)
                {
                    GD.Print("Wheel: " + rayPoint.Name);
                    GD.Print("hitDistance: " + hitDistance);
                    GD.Print("upVector: " + rayPoint.GlobalBasis.Y);
                    GD.Print("currentSpringLength: " + currentSpringLength);
                    GD.Print("springCompression: " + springCompression);
                    GD.Print("springVelocity: " + springVelocity);
                    GD.Print("dampForce: " + dampForce);
                    GD.Print("springForce: " + springForce);
                    GD.Print("netForce: " + netForce);
                    GD.Print("ForceDir: " + netForce * rayPoint.GlobalBasis.Y);
                    GD.Print("----------------");
                }*/

                ApplyForce(netForce * rayPoint.GlobalBasis.Y, rayPoint.GlobalPosition - ToGlobal(CenterOfMass));

                SetTirePosition(_tires[i], collisionPoint + rayPoint.Basis.Y * _wheelRadius);
            }
            else
            {
                _wheelIsGrounded[i] = false;
                SetTirePosition(_tires[i], rayPoint.GlobalPosition - rayPoint.Basis.Y * _restLength);
            }
        }
    }
    private Vector3 GetPointVelocity(Vector3 point) => LinearVelocity + AngularVelocity.Cross(point - ToGlobal(CenterOfMass));

    #endregion

    #region CarStatusCheck

    private void GroundCheck() => _isGrounded = _wheelIsGrounded.Where(a => a).Count() > 1;

    #endregion

    #region Calculations

    private void CalculateCarVelocity()
    {
        _currentCarLocalVelocity = GlobalBasis.Inverse() * LinearVelocity;
        _carVelocityRatio = _currentCarLocalVelocity.Z / _maxSpeed;
    }

    #endregion

    #region Input

    private void GetPlayerInput()
    {
        _steerInput = Input.GetAxis("move_right", "move_left");
        _moveInput = Input.GetAxis("move_backward", "move_forward");
    }

    #endregion

    #region Movement

    private void Movement()
    {
        if (_isGrounded)
        {
            Acceleration();
            Deceleration();
            Turn();
            SidewaysDrag();
        }
    }

    private void Acceleration()
    {
        //GD.Print("Acceleration force: " + _acceleration * _moveInput * -Basis.Z * Mass);
        ApplyForce(_acceleration * _moveInput * -Basis.Z * Mass, _accelerationPoint.Position);
    }

    private void Deceleration()
    {
        //GD.Print("Deceleration force: " + (Input.GetActionStrength("car_break") > 0 ? _breakingDeceleration : _deceleration) * Mathf.Abs(_carVelocityRatio) * Basis.Z * Mass);
        ApplyForce((Input.GetActionStrength("car_break") > 0 ? _breakingDeceleration : _deceleration) * Mathf.Abs(_carVelocityRatio) * Basis.Z * Mass, _accelerationPoint.Position);
    }

    private void Turn()
    {
        ApplyTorque(_steerStrength * _steerInput * _turningCurve.Sample(Mathf.Abs(_carVelocityRatio)) * Mathf.Sign(-_carVelocityRatio) * Basis.Y * Mass);
    }

    private void SidewaysDrag()
    {
        if (Mathf.Abs(_currentCarLocalVelocity.X) < 1f)
        {
            return;
        }

        var currentSidewaysSpeed = _currentCarLocalVelocity.X;
        var dragMagnitude = -currentSidewaysSpeed * Input.GetActionStrength("car_break") > 0 ? _breakingDragCoefficient : _dragCoefficient;
        var dragForce = Basis.X * dragMagnitude;

        ApplyCentralForce(dragForce * Mass);
    }

    #endregion

    #region Camera

    private void CameraFollow(float delta)
    {
        _cameraPivot.GlobalPosition = _cameraPivot.GlobalPosition.Lerp(GlobalPosition, delta * 20);
        _cameraPivot.Transform = _cameraPivot.Transform.InterpolateWith(Transform, delta * 5);
        _cameraLookAt = _cameraLookAt.Lerp(GlobalPosition + LinearVelocity, delta * 5);
        _camera.LookAt(_cameraLookAt);
        _reverseCamera.LookAt(_cameraLookAt);
        CheckCameraSwitch();
    }

    private void CheckCameraSwitch()
    {
        if (LinearVelocity.Dot(Basis.Z) < 5)
        {
            _camera.Current = true;
        }
        else
        {
            _reverseCamera.Current = true;
        }
    }

    #endregion

    #region Visuals

    private void Visuals(float delta)
    {
        TireVisuals(delta);
    }

    private void SetTirePosition(Node3D tire, Vector3 targetPosition)
    {
        tire.GlobalPosition = targetPosition;
    }

    private void TireVisuals(float delta)
    {
        var steeringAngle = _maxSteeringAngle * _steerInput;

        for (var i = 0; i < _tires.Length;  i++)
        {
            if (i < 2)
            {
                _tires[i].RotateX(_carVelocityRatio * _tireRotSpeed * 2.5f * delta);// 2.5 - additional spinning force (without it, it's too slow)

                _frontTiresParent[i].RotationDegrees = new Vector3(_frontTiresParent[i].RotationDegrees.X, (steeringAngle), _frontTiresParent[i].RotationDegrees.Z);
            }
            else
            {
                _tires[i].RotateX(_moveInput * _tireRotSpeed * delta);
            }
        }
    }

    #endregion
}
