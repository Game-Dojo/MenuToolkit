using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MenuSettings", menuName = "Menu Toolkit/Create Settings", order = 0)]
public class SettingsScriptable : ScriptableObject
{
    public List<string> resolutions = new()
    {
        "1920x1080",
        "1280x720",
        "640x360",
        "320x240"
    };

    public enum Languages 
    {
        English,
        Spanish,
        Chinese
    }
    
    [Header("Video")]
    public int resolution = 0;
    public FullScreenMode windowMode;
    public int quality;
    public int fieldOfView = 60;

    [Header("Audio")]
    public bool musicEnabled = true;
    public bool soundEnabled = true;
    
    public float musicVolume = 50f;
    public float soundVolume = 50f;

    [Header("Language")]
    public Languages language = Languages.English;
}
