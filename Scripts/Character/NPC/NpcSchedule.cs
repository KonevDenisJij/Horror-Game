using System.Linq;
using System.Text.Json.Serialization;

public class NpcSchedule
{
    [JsonPropertyName("characterName")]
    public string CharacterName { get; set; }

    [JsonPropertyName("time")]
    public Time[] TimeArray { get; set; }

    public class Time
    {        
        [JsonPropertyName("timeOfDay")]
        public string TimeOfDay { get; set; }

        [JsonPropertyName("performingAction")]
        public PerformingAction Action { get; set; }

        [JsonPropertyName("triggers")]
        public Trigger[] Triggers { get; set; }

        public class PerformingAction
        {
            [JsonPropertyName("action")]
            public string Name { get; set; }

            [JsonPropertyName("delay")]
            public int Delay { get; set; }

            [JsonPropertyName("actionParams")]
            public Param[] ActionParams { get; set; }

            public class Param
            {
                [JsonPropertyName("name")]
                public string Name { get; set; }
            }
        }

        public class Trigger
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("action")]
            public string Action { get; set; }
        }
    }
}
