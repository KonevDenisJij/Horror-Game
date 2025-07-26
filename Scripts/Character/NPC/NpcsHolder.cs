using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class NpcsHolder : Node3D
{
    public static NpcsHolder Instance => _instance;
    private static NpcsHolder _instance;

    public List<NPC> NPCs => _npcs;
    private List<NPC> _npcs;

    public override void _Ready()
    {
        if (_instance != null)
        {
            GD.PrintErr("We already have NpcsHolder instance");
            Free();
            return;
        }

        _instance = this;
        _npcs = new List<NPC>();

        base._Ready();
    }

    public NPC GetById(string name)
    {
        return _npcs.Single(a => a.SysName == name);
    }
}
