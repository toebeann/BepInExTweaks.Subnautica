using BepInEx.Configuration;
using UnityEngine;

namespace Tobey.BepInExTweaks.Subnautica;
public static class Config
{
    internal static ConfigFile Cfg => BepInExTweaks.Instance.Config;

    public static class ConfigurationManager
    {
        public static ConfigEntry<KeyboardShortcut> ShowConfigManager { get; } = BepInExTweaks.HasConfigurationManager switch
        {
            true => Cfg.Bind(
                            section: nameof(ConfigurationManager),
                            key: "Show Configuration Manager",
                            defaultValue: new KeyboardShortcut(KeyCode.F2),
                            description: "Shortcut to open the Configuration Manager overlay."
                        ),
            false => null
        };
    }

    public static class General
    {
        public static ConfigEntry<bool> AddSceneCleanerPreserve { get; } =
            Cfg.Bind(
                section: nameof(General),
                key: "Preserve BepInEx runtime GameObjects",
                defaultValue: true,
                configDescription: new(
                    description: "Prevents important BepInEx GameObjects from being destroyed when the game cleans the scene.",
                    tags: new[] { new ConfigurationManagerAttributes { IsAdvanced = true } }
                )
            );
    }
}
