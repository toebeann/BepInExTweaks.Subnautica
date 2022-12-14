using BepInEx.Configuration;

namespace Tobey.BepInExTweaks.Subnautica;
public static class Config
{
    internal static ConfigFile Cfg => BepInExTweaks.Instance.Config;

    public static class General
    {
        //public static ConfigEntry<KeyboardShortcut> ShowConfigManager { get; } = 
    }
}
