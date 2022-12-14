using BepInEx;
using System;
using UnityEngine;

namespace Tobey.BepInExTweaks.Subnautica;
using static Config;

[BepInDependency("com.bepis.bepinex.configurationmanager", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[DisallowMultipleComponent]
public class BepInExTweaks : BaseUnityPlugin
{
    public static BepInExTweaks Instance { get; private set; }

    private static Lazy<Component> configurationManagerPlugin = new(() => Instance.GetPlugin("com.bepis.bepinex.configurationmanager"));
    public static Component ConfigurationManagerPlugin => configurationManagerPlugin.Value;

    public static bool HasConfigurationManager => ConfigurationManagerPlugin != null;

    public ConfigurationManagerTweaks ConfigurationManagerTweaks { get; private set; }
    public SceneCleanerTweaks SceneCleanerTweaks { get; private set; }

    private void Awake()
    {
        // enforce singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
            return;
        }
    }

    private void OnEnable()
    {
        AddTweaks();
        Bind();
    }

    private void AddTweaks()
    {
        AddConfigurationManagerTweaks();
        AddSceneCleanerTweaks();
    }

    private void RemoveTweaks()
    {
        Destroy(ConfigurationManagerTweaks);
        Destroy(SceneCleanerTweaks);
    }

    private void Bind() => General.AddSceneCleanerPreserve.SettingChanged += AddSceneCleanerPreserve_SettingChanged;

    private void Unbind() => General.AddSceneCleanerPreserve.SettingChanged -= AddSceneCleanerPreserve_SettingChanged;

    private void AddConfigurationManagerTweaks()
    {
        if (HasConfigurationManager)
        {
            ConfigurationManagerTweaks = gameObject.AddComponent<ConfigurationManagerTweaks>();
        }
    }

    private void AddSceneCleanerTweaks()
    {
        if (General.AddSceneCleanerPreserve.Value)
        {
            SceneCleanerTweaks = gameObject.AddComponent<SceneCleanerTweaks>();
        }
        else
        {
            Destroy(SceneCleanerTweaks);
        }
    }

    private void AddSceneCleanerPreserve_SettingChanged(object _, EventArgs __) => AddSceneCleanerTweaks();

    private void OnDisable()
    {
        Unbind();
        RemoveTweaks();
    }
}
