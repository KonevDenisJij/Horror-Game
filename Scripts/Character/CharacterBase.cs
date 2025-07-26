using Godot;
public partial class CharacterBase : CharacterBody3D
{
    public PlaceEnum CurrentPlace => _currentPlace;
    protected PlaceEnum _currentPlace;

    private Vector3 _targetRotation;
    protected float _rotationProcess;

    protected Ball _pickedItem;

    public float Gravity => ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public void SetCurrentPlace(PlaceEnum place)
    {
        _currentPlace = place;
    }

    public void LookAtOverTime(Vector3 lookAtPoint)
    {
        var pos = lookAtPoint;

        var rotation = Transform
            .LookingAt(pos)
            .Basis
            .GetEuler();

        _rotationProcess = 0f;
        _targetRotation = rotation;
    }

    public void PickUpObject(Ball interObject)
    {
        if (interObject.GetParent() != null)
        {
            interObject.GetParent().RemoveChild(interObject);
        }

        AddChild(interObject);
        interObject.Position = new Vector3(0f, 0.5f, -0.4f);
        _pickedItem = interObject;
        ClearCurrentInterObject();
    }

    protected virtual void ClearCurrentInterObject()
    {
    }

    public void UseItem(Vector3 direction)
    {
        if (_pickedItem == null)
        {
            return;
        }

        _pickedItem.ThrowItem(direction, 2f, GetParent());
        _pickedItem = null;
    }

    public void UseItemAlter()
    {
        if (_pickedItem == null)
        {
            return;
        }

        _pickedItem.PlaceItem(GetParent());
        _pickedItem = null;
    }

    protected void ProcessAutomaticRotationXY(double delta, Node3D rotationHelper)
    {
        if (_rotationProcess > 1f)
        {
            if (Rotation.Y != _targetRotation.Y || rotationHelper.Rotation.X != _targetRotation.X)
            {
                Rotation = new Vector3(Rotation.X, _targetRotation.Y, Rotation.Z);
                rotationHelper.Rotation = new Vector3(_targetRotation.X, rotationHelper.Rotation.Y, rotationHelper.Rotation.Z);
            }

            return;
        }

        _rotationProcess += (float)delta;
        var yRotation = Mathf.LerpAngle(Rotation.Y, _targetRotation.Y, _rotationProcess) - Rotation.Y;
        var xRotation = Mathf.LerpAngle(rotationHelper.Rotation.X, _targetRotation.X, _rotationProcess) - rotationHelper.Rotation.X;

        RotateY(yRotation);
        rotationHelper.RotateX(xRotation);
    }
}