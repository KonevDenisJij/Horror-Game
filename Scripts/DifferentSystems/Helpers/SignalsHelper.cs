using Godot;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

public static class SignalsHelper
{
    public static bool ParseSignalName(string signalName)
    {
        var singleParam = string.Empty;
        var multiParam = new string[1];

        switch (signalName.Substring(0, signalName.IndexOf('(')))
        {
            case nameof(DialogueStart):
                singleParam = signalName.Substring(14, signalName.IndexOf(')') - 14);
                return DialogueStart(int.Parse(singleParam));
            case nameof(ReplyStart):
                singleParam = signalName.Substring(11, signalName.IndexOf(')') - 11);
                return ReplyStart(int.Parse(singleParam));
            case nameof(DialogueFinish):
                singleParam = signalName.Substring(15, signalName.IndexOf(')') - 15);
                return DialogueFinish(int.Parse(singleParam));
        }


        GD.Print("No such signal " + signalName);
        return false;
    }

    public static bool DialogueStart(int dialogueId)
    {
        try
        {
            var dialogue = DialogueSystem.Instance.GetById<Dialogue>(dialogueId);
            var npc = NpcsHolder.Instance.GetById(dialogue.CharacterId);

            Player.Instance.LookAtOverTime(npc.GlobalPosition);
            npc.Turn(Player.Instance.GlobalPosition);

            DialogueSystem.Instance.ShowDialogue(dialogue);
        }
        catch (Exception e)
        {
            GD.PrintErr("Error at DialogueStart signal " + e.Message, e.StackTrace);
            return false;
        }

        return true;
    }

    public static bool DialogueFinish(int nextDialogueId) //Id of the next dialogue of the character
    {
        DialogueSystem.Instance.HideDialogue();
        return true;
    }

    public static bool ReplyStart(int replyId)
    {
        try
        {
            var reply = DialogueSystem.Instance.GetById<Reply>(replyId);
            var npc = NpcsHolder.Instance.GetById(reply.CharacterId);

            Player.Instance.LookAtOverTime(npc.GlobalPosition);
            npc.Turn(Player.Instance.GlobalPosition);

            DialogueSystem.Instance.ShowReply(reply);
        }
        catch (Exception e)
        {
            GD.PrintErr("Error at DialogueStart signal " + e.Message, e.StackTrace);
            return false;
        }

        return true;
    }
}