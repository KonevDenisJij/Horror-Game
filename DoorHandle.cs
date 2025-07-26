using Godot;

public partial class DoorHandle : StaticBody3D, IInteractableObject
{
    public string InteractText => "Open";
    public string InteractionSignalName { get; set; }

    private bool _isOpen;
    public bool IsOpen => _isOpen;

    [Export]
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _isOpen = false;
        _animationPlayer = _animationPlayer ?? GetParent().GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void Interact()
    {
        if (_animationPlayer.IsPlaying())
        {
            return;
        }

        if (!_isOpen)
        {
            _animationPlayer.Play("Open");
            _isOpen = true;
            return;
        }

        _animationPlayer.Play("Close");
        _isOpen = false;
    }
}
