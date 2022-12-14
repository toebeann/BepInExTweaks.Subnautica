using BepInEx;
using System;
using UnityEngine;

namespace Tobey.BepInExTweaks.Subnautica;

[BepInDependency("com.bepis.bepinex.configurationmanager", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class BepInExTweaks : BaseUnityPlugin
{
    public static BepInExTweaks Instance { get; private set; }

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
}
