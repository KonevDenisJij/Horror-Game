using Godot;
using System;

public partial class PopUpUI : Control
{
    private const string DEFAULT_TEXT = "Interact";

    [Export]
    private Label _label;

    public static PopUpUI Instance;
    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.PrintErr("More then one instance of PopUpUI class");
            QueueFree();
            return;
        }

        Instance = this;
    }

    public void ShowPopUp(string text)
    {
        if (text == null || text.Length == 0)
        {
            text = DEFAULT_TEXT;
        }

        _label.Text = $"E to {text}";

        GetChild<Control>(0).Show();
    }

    public void HidePopUp()
    {
        GetChild<Control>(0).Hide();
    }
}
