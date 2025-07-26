using System.Text.Json.Serialization;

public class Reply : IDialogueComponent
{
    [JsonPropertyName("CharacterId")]
    public string CharacterId { get; set; }

    [JsonPropertyName("ReplyId")]
    public int Id { get; set; }

    [JsonPropertyName("CharacterTextKey")]
    public string CharacterTextKey { get; set; }

    [JsonPropertyName("RepliesMoveSpeed")]
    public float RepliesMoveSpeed { get; set; }

    [JsonPropertyName("RepliesMovePattern")]
    public int RepliesMovePattern { get; set; }

    [JsonPropertyName("ReplyOptions")]
    public ReplyOptions[] RepliesArray { get; set; }

    public class ReplyOptions
    {
        [JsonPropertyName("Order")]
        public int Order { get; set; }

        [JsonPropertyName("TextKey")]
        public string TextKey { get; set; }

        [JsonPropertyName("SignalName")]
        public string SignalName { get; set; }

        [JsonPropertyName("ReplyMood")]
        public int ReplyMood { get; set; }
    }
}