using Godot;
using System.Collections.Generic;

public partial class DialogueSystemUI : Control
{
    [Export]
    private PackedScene _button;
    [Export]
    private Label _characterNameLabel;
    [Export]
    private RichTextLabel _dialogueTextLabel;
    [Export]
    private Control _replyWindow;
    [Export]
    private Control _dialogueWindow;

    [Export]
    private float _speed = 0.1f;

    public static DialogueSystemUI Instance => _instance;
    private static DialogueSystemUI _instance;

    private List<Button> _buttons;

    public override void _Ready()
    {
        if (_instance != null)
        {
            GD.PrintErr("We already have instance of DialogueSystemUINew!");
            QueueFree();
            return;
        }

        _instance = this;
        _buttons = new List<Button>();
    }

    public void ShowDialogue(Dialogue dialogue)
    {
        _replyWindow.Hide();
        _dialogueWindow.Show();

        _characterNameLabel.Text = dialogue.CharacterId;
        _dialogueTextLabel.Text = dialogue.TextKey;
    }

    public void ShowReply(Reply reply)
    {
        for (var i = 0; i < reply.RepliesArray.Length; i++)
        {
            var replyOption = reply.RepliesArray[i];
            var newButton = _button.Instantiate<ReplyOptionButton>();

            _replyWindow.AddChild(newButton);
            newButton.SetReply(replyOption, reply.RepliesMoveSpeed, 1f / reply.RepliesArray.Length * i);

            _buttons.Add(newButton);
        }

        _dialogueWindow.Hide();
        _replyWindow.Show();
    }

    public void HideUi()
    {
        _replyWindow.Hide();
        _dialogueWindow.Hide();

        _buttons.ForEach(button => button.QueueFree());
        _buttons = new List<Button>();
    }
}