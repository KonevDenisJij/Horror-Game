using Godot;
using static Settings;

public partial class PauseMenu : Node3D
{
    private bool _isPaused;
    public bool IsPaused => _isPaused;

    private static PauseMenu _instance;
    public static PauseMenu Instance => _instance;

    public override void _Ready()
    {
        if (_instance != null)
        {
            GD.PrintErr("We already have instance of PauseMenu class");
            QueueFree();
            return;
        }

        _isPaused = false;
        _instance = this;

        var settings = new Settings();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("open_menu"))
        {
            if (_isPaused)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }
    }

    public void OpenMenu()
    {
        _isPaused = true;
        MouseModeController.Instance.SetMouseMode();
        PauseMenuUI.Instance.OpenMenu();
    }

    public void CloseMenu()
    {
        _isPaused = false;
        MouseModeController.Instance.SetMouseMode();
        PauseMenuUI.Instance.CloseMenu();
    }

    public void ExitGame()
    {
        _isPaused = false;
        GetTree().Quit();
    }
}