using Godot;
using System;

public partial class SettingsUI : Control
{
    [Export]
    private Control _menuBaseUI;

    private void OnBackButtonUp()
    {
        _menuBaseUI.Show();
        Hide();
    }
}
