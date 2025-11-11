using System.Collections.Generic;
using UnityEngine;

public enum GamepadType
{
    Generic,
    Xbox,
    PlayStation,
    Nintendo
}

[CreateAssetMenu(fileName = "GamepadIconMap", menuName = "Input/Icon Map")]
public class GamepadIconMap : ScriptableObject
{
    public GamepadType gamepadType;

    [System.Serializable]
    public class IconEntry
    {
        public string displayName;
        public string controlPath;
        public string spriteTag;
    }

    public List<IconEntry> entries = new();

    public string GetIcon(string path)
    {
        IconEntry entry = entries.Find(e => e.controlPath == path);
        return entry != null ? entry.spriteTag : path;
    }

    public string GetName(string path)
    {
        IconEntry entry = entries.Find(e => e.controlPath == path);
        return entry != null ? entry.displayName : path;
    }
}
