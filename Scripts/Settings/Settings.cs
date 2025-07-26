using Godot;
using System;

public partial class Settings
{
    public Settings Instance => _instance;
    private Settings _instance;

    private string _settingsPath = "res://Files/Settings/Settings.txt";

    private TextureQuality _textureQuality;
    private float _musicVolume;
    private float _soundVolume;

    public Settings()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        using (var file = FileAccess.Open(_settingsPath, FileAccess.ModeFlags.ReadWrite))
        {
            var graphicsSring = file.GetLine();
            graphicsSring = graphicsSring.Substring(graphicsSring.IndexOf(':') + 2);
            var musicVolumeSring = file.GetLine();
            musicVolumeSring = musicVolumeSring.Substring(musicVolumeSring.IndexOf(':') + 2);
            var soundVolumeSring = file.GetLine();
            soundVolumeSring = soundVolumeSring.Substring(soundVolumeSring.IndexOf(':') + 2);

            if (graphicsSring == string.Empty || musicVolumeSring == string.Empty || soundVolumeSring == string.Empty)
            {
                SetDefaultSettings(file);
                return;
            }

            _textureQuality = graphicsSring == "Good" 
                ? TextureQuality.Good 
                : graphicsSring == "Normal" 
                ? TextureQuality.Normal 
                : graphicsSring == "Bad" 
                ? TextureQuality.Bad 
                : TextureQuality.Good;
            //_musicVolume = musicVolumeSring
        }
    }

    public void SetDefaultSettings(FileAccess file)
    {
        _textureQuality = TextureQuality.Good;
        _soundVolume = 1.0f;
        _musicVolume = 1.0f;

        var defaultSettingsString = $"textureQuality: {_textureQuality} \n" +
            $"musicVolume: {_musicVolume} \n" +
            $"soundVolume: {_soundVolume}";

        file.StoreString(defaultSettingsString);

    }

    public enum TextureQuality
    {
        Good,
        Normal,
        Bad,
    }
}
