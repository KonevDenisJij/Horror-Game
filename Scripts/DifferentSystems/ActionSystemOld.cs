using Godot;

public partial class ActionSystem : Node3D
{
    /*public static ActionSystem Instance => _instance;
    private static ActionSystem _instance;

    private Dictionary<string, List<TriggerStruct>> _triggerList;

    public override void _Ready()
    {
        if (_instance != null)
        {
            GD.PrintErr("We already have ActionSystem instance");
            Free();
            return;
        }

        _instance = this;
        _triggerList = new Dictionary<string, List<TriggerStruct>>();

        base._Ready();
    }

    public void FillNpcTriggers(NpcSchedule.Time schedule, NPC npc)
    {
        foreach (var trigger in schedule.Triggers)
        {
            if (_triggerList.ContainsKey(trigger.Id))
            {
                _triggerList[trigger.Id].Add(new TriggerStruct(npc, trigger.Action));
                continue;
            }

            _triggerList.Add(trigger.Id, new List<TriggerStruct>
            {
                new TriggerStruct(npc, trigger.Action)
            });
        }
    }

    public void OnInteractionHappened(string interactionId)
    {
        if (_triggerList.ContainsKey(interactionId))
        {
            _triggerList[interactionId].ForEach(a => a.Npc.ProcessCustomSignal(a.Action));
        }
    }

    private class TriggerStruct
    {
        public TriggerStruct(NPC npc, string action)
        {
            _npc = npc;
            _action = action;
        }

        public NPC Npc => _npc;
        public NPC _npc;

        public string Action => _action;
        public string _action;
    }*/
}
