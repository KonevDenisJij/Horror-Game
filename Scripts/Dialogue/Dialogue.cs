using System.Text.Json.Serialization;

public class Dialogue : IDialogueComponent
{
    [JsonPropertyName("CharacterId")]
    public string CharacterId { get; set; }

    [JsonPropertyName("DialogueId")]
    public int Id { get; set; }

    [JsonPropertyName("TextKey")]
    public string TextKey { get; set; }

    [JsonPropertyName("SignalName")]
    public string SignalName { get; set; }

    [JsonPropertyName("TextAppearenceSpeed")]
    public float TextAppearenceSpeed { get; set; }

    [JsonPropertyName("CharacterSpeechMood")]
    public string CharacterSpeechMood { get; set; }
}
