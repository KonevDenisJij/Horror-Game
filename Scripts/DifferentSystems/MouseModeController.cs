using Godot;
using System;

public partial class MouseModeController : Node3D
{
    private static MouseModeController _instance;
    public static MouseModeController Instance => _instance;

    public override void _Ready()
    {
        if (_instance != null)
        {
            GD.PrintErr("We already have instance of PauseMenu class");
            QueueFree();
            return;
        }

        _instance = this;

        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public void SetMouseMode()
    {
        if (PauseMenu.Instance.IsPaused || DialogueSystem.Instance.IsBusy)
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
        else
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
    }
}
