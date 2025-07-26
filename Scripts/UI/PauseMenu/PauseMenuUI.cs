using Godot;

public partial class PauseMenuUI : Control
{
    private static PauseMenuUI _instance;
    public static PauseMenuUI Instance => _instance;

    [Export]
    private Control _menuBaseUI;
    [Export]
    private Control _settingsUI;

    public override void _Ready()
    {
        if (_instance != null)
        {
            GD.PrintErr("We already have instance of PauseMenu class");
            QueueFree();
            return;
        }

        _instance = this;
    }

    public void OpenMenu()
    {
        _settingsUI.Hide();
        _menuBaseUI.Show();
    }

    public void CloseMenu()
    {
        _settingsUI.Hide();
        _menuBaseUI.Hide();
    }

    private void ContinueButtonClick()
    {
        PauseMenu.Instance.CloseMenu();
    }

    private void OptionsButtonClick()
    {
        _settingsUI.Show();
        _menuBaseUI.Hide();
    }

    private void ExitButtonClick()
    {
        PauseMenu.Instance.ExitGame();
    }
}
