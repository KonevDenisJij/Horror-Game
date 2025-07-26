using Godot;

public partial class Player : CharacterBase
{
    [Export]
    private float _speed = 5.0f;
    [Export]
    private float _jumpVelocity = 1f;
    [Export]
    private Camera3D _camera;
    [Export]
    private Node3D _rotationHelper;
    [Export]
    private float _mouseSensitivity = 0.1f;
    [Export]
    private RayCast3D _rayCast;
    [Export]
    private float _pushForce = 10f;

    private IInteractableObject _currentInterObject;

    private static Player _instance;
    public static Player Instance => _instance;

    public Camera3D Camera => _camera;

    public override void _Ready()
    {
        if (_instance != null)
        {
            GD.PrintErr("We already have instance of Player!");
            QueueFree();
            return;
        }

        _instance = this;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (PauseMenu.Instance.IsPaused)
        {
            return;
        }

        if (DialogueSystem.Instance.IsBusy)
        {
            if (_currentInterObject != null)
            {
                ClearCurrentInterObject();
            }

            ProcessAutomaticRotationXY(delta, _rotationHelper);
            return;
        }

        ProcessMovement(delta);

        if (_pickedItem != null)
        {
            ProcessPickedItem(delta);
            return;
        }

        ProcessInteractions();
    }

    public override void _Input(InputEvent @event)
    {
        if (PauseMenu.Instance.IsPaused)
        {
            return;
        }

        if (@event is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            InputEventMouseMotion mouseEvent = @event as InputEventMouseMotion;
            _rotationHelper.RotateX(Mathf.DegToRad(-mouseEvent.Relative.Y * _mouseSensitivity));
            RotateY(Mathf.DegToRad(-mouseEvent.Relative.X * _mouseSensitivity));

            Vector3 cameraRot = _rotationHelper.Rotation;
            cameraRot.X = Mathf.Clamp(cameraRot.X, -1.2217305f, 1.2217305f);
            _rotationHelper.Rotation = cameraRot;
        }

        if (@event is InputEventKey key && key.Keycode == Key.P)
        {
            InputEventMouseMotion mouseEvent = @event as InputEventMouseMotion;
            _rotationHelper.RotateX(Mathf.DegToRad(-mouseEvent.Relative.Y * _mouseSensitivity));
            RotateY(Mathf.DegToRad(-mouseEvent.Relative.X * _mouseSensitivity));

            Vector3 cameraRot = _rotationHelper.Rotation;
            cameraRot.X = Mathf.Clamp(cameraRot.X, -1.2217305f, 1.2217305f);
            _rotationHelper.Rotation = cameraRot;
        }
    }

    protected override void ClearCurrentInterObject()
    {
        _currentInterObject = null;
        PopUpUI.Instance.HidePopUp();
    }

    private void ProcessPickedItem(double delta)
    {
        if (Input.IsActionJustPressed("use_item"))
        {
            UseItem(-_camera.GlobalTransform.Basis.Z);
        }
        else if (Input.IsActionJustPressed("use_item_alter"))
        {
            UseItemAlter();
        }
    }

    private void ProcessMovement(double delta)
    {
        Vector3 velocity = Velocity;
        var sprinting = Input.IsActionPressed("sprint");

        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity.Y -= Gravity * (float)delta;
        }
        else if (Input.IsActionJustPressed("jump"))
        {
            velocity.Y = _jumpVelocity;
        }

        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * _speed * (sprinting ? 2 : 1);
            velocity.Z = direction.Z * _speed * (sprinting ? 2 : 1);
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, _speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, _speed);
        }

        Velocity = velocity;
        MoveAndSlide();

        for(var i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);
            if (collision.GetCollider() is RigidBody3D collider)
            {
                collider.ApplyCentralImpulse(-collision.GetNormal() * (velocity.Length() + 0.1f)/ _speed);
            }
        }
    }

    private void ProcessInteractions()
    {
        var interObject = _rayCast.GetCollider() as IInteractableObject;

        if (interObject != null)
        {
            if (_currentInterObject != interObject)
            {
                _currentInterObject = interObject;
                PopUpUI.Instance.ShowPopUp(interObject.InteractText);
                return;
            }
        }
        else
        {
            if (_currentInterObject != null)
            {
                ClearCurrentInterObject();
            }
        }

        if (_currentInterObject == null)
        {
            return;
        }

        if (Input.IsActionJustPressed("interact"))
        {
            _currentInterObject.Interact();
        }
    }
}