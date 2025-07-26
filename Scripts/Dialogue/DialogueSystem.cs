using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public partial class DialogueSystem : Node3D
{
    private static DialogueSystem _instance;
    public static DialogueSystem Instance => _instance;

    private IDialogueComponent _currentDialogue;

    private bool _isBusy;
    public bool IsBusy => _isBusy;

    public Dialogue[] AllDialogues;

    public Reply[] AllReplies;

    private DialogueSystemUI UI => DialogueSystemUI.Instance;

    public override void _Ready()
    {
        if (_instance != null)
        {
            GD.PrintErr("We already have instance of DialogueSystem!");
            QueueFree();
            return;
        }

        _isBusy = false;
        _instance = this;

        var dialoguesFile = FileAccess.Open($"res://Files/Dialogues/Dialogues.json", FileAccess.ModeFlags.Read);
        AllDialogues = JsonSerializer.Deserialize<List<Dialogue>>(dialoguesFile.GetAsText()).OrderBy(a => a.Id).ToArray();
        dialoguesFile.Close();

        var repliesFile = FileAccess.Open($"res://Files/Dialogues/Replies.json", FileAccess.ModeFlags.Read);
        AllReplies = JsonSerializer.Deserialize<List<Reply>>(repliesFile.GetAsText()).OrderBy(a => a.Id).ToArray();
        repliesFile.Close();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isBusy || PauseMenu.Instance.IsPaused)
        {
            return;
        }

        if (Input.IsActionJustPressed("next_dialogue_line") && _currentDialogue is Dialogue)
        {
            CallDialogueSignal(_currentDialogue as Dialogue);
        }
    }

    public T GetById<T>(int id) where T : IDialogueComponent
    {
        if (typeof(T) == typeof(Dialogue))
        {
            var min = 0;
            var max = AllDialogues.Length - 1;

            while (min <= max)
            {
                var mid = (min + max) / 2;
                if (id == AllDialogues[mid].Id)
                {
                    return (T)Convert.ChangeType(AllDialogues[mid], typeof(T));
                }
                else if (id < AllDialogues[mid].Id)
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }
        }
        else if (typeof(T) == typeof(Reply))
        {
            var min = 0;
            var max = AllReplies.Length - 1;

            while (min <= max)
            {
                var mid = (min + max) / 2;
                if (id == AllReplies[mid].Id)
                {
                    return (T)Convert.ChangeType(AllReplies[mid], typeof(T));
                }
                else if (id < AllReplies[mid].Id)
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }
        }

        GD.Print("WrongType");
        return (T)(object)null;
    }

    public void ShowDialogue(Dialogue dialogue)
    {
        _currentDialogue = dialogue;
        _isBusy = true;
        MouseModeController.Instance.SetMouseMode();
        UI.ShowDialogue(dialogue);
    }

    public void ShowReply(Reply reply)
    {
        _currentDialogue = reply;
        _isBusy = true;
        MouseModeController.Instance.SetMouseMode();
        UI.ShowReply(reply);
    }

    public void HideDialogue()
    {
        _currentDialogue = null;
        _isBusy = false;
        MouseModeController.Instance.SetMouseMode();
        UI.HideUi();
    }

    private void CallDialogueSignal(Dialogue dialogue)
    {
        SignalsHelper.ParseSignalName(dialogue.SignalName);
    }
}
