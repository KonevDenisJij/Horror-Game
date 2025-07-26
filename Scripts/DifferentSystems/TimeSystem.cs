using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public partial class TimeSystem : Node3D
{
    /*const string SCHEDULE_PATH = "Schedules/";
    private string _actualPath;
    private int _currentDay;
    public static TimeSystem Instance => _instance;
    private static TimeSystem _instance;

    private TimeEnum _currentTime;

    public override void _Ready()
    {
        if (_instance != null)
        {
            GD.PrintErr("We already have TimeSystem instance");
            Free();
            return;
        }

        _instance = this;
        _currentDay = 1;
        _actualPath = SCHEDULE_PATH + "day" + _currentDay + "/";
        _currentTime = TimeEnum.Morning;

        base._Ready();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey key && key.IsPressed() && key.Keycode == Key.T)
        {
            GD.Print($"Current time {_currentTime}");
        }

        else if (@event is InputEventKey key2 && key2.IsPressed() && key2.Keycode == Key.Y)
        {
            MoveCurrentTime();
        }

        else if (@event is InputEventKey key3 && key3.IsPressed() && key3.Keycode == Key.U)
        {
            ProcessTimeChange();
        }
    }

    public void MoveCurrentTime()
    {
        switch (_currentTime)
        {
            case TimeEnum.Morning:
                _currentTime = TimeEnum.Day;
                break;
            case TimeEnum.Day:
                _currentTime = TimeEnum.Evening;
                break;
            case TimeEnum.Evening:
                _currentTime = TimeEnum.Morning;
                ProcessDayChange();
                break;
        }

        ProcessTimeChange();
    }

    public static TimeEnum StringToTime(string str)
    {
        switch (str)
        {
            case "morning":
                return TimeEnum.Morning;
            case "day":
                return TimeEnum.Day;
            case "evening":
                return TimeEnum.Evening;
            default:
                return TimeEnum.Morning;
        }
    }

    private void ProcessDayChange()
    {
        _actualPath = SCHEDULE_PATH + "day" + ++_currentDay + "/";
    }

    private void ProcessTimeChange()
    {
        DirectoryInfo d = new DirectoryInfo(_actualPath);
        FileInfo[] files = d.GetFiles("*.json");

        foreach (FileInfo file in files)
        {
            NpcSchedule schedule;
            using (FileStream fs = new FileStream(file.FullName, FileMode.Open))
            {
                schedule = JsonSerializer.Deserialize<NpcSchedule>(fs);
            }

            var currentTime = schedule.TimeArray.SingleOrDefault(a => StringToTime(a.TimeOfDay) == _currentTime);
            if (currentTime == null)
            {
                return;
            }

            var npc = NpcsHolder.Instance.GetById(schedule.CharacterName);
            ActionSystem.Instance.FillNpcTriggers(currentTime, npc);
            npc.ProcessCustomSignal(currentTime.Action.Name, currentTime.Action.ActionParams.Select(a => a.Name).ToArray());
        }
    }

    public enum TimeEnum
    {
        Morning = 1,
        Day = 2,
        Evening = 4
    }*/
}
