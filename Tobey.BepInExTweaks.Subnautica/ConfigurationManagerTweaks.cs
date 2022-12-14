using HarmonyLib;
using System;
using UnityEngine;

namespace Tobey.BepInExTweaks.Subnautica;

[DisallowMultipleComponent]
public class ConfigurationManagerTweaks : MonoBehaviour
{
    private static Lazy<Traverse> configurationManager = new(() => Traverse.Create(BepInExTweaks.ConfigurationManagerPlugin));
    public static Traverse ConfigurationManager => configurationManager.Value;

    private static Lazy<Traverse> configurationManager_overrideHotkey = new(() => ConfigurationManager.Field("OverrideHotkey"));
    public static Traverse ConfigurationManager_OverrideHotkey => configurationManager_overrideHotkey.Value;

    private static Lazy<Traverse> configurationMAnager_displayingWindow = new(() => ConfigurationManager.Property("DisplayingWindow"));
    public static Traverse ConfigurationManager_DisplayingWindow => configurationMAnager_displayingWindow.Value;

    private void OnEnable()
    {
        ConfigurationManager_OverrideHotkey.SetValue(true);
    }

    private void Update()
    {
        if (Config.ConfigurationManager.ShowConfigManager.Value.IsDown())
        {
            ConfigurationManager_DisplayingWindow.SetValue(!ConfigurationManager_DisplayingWindow.GetValue<bool>());
        }
    }

    private void OnDisable()
    {
        ConfigurationManager_OverrideHotkey.SetValue(false);
    }
}
