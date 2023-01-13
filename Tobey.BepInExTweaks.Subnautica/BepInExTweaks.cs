using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Tobey.BepInExTweaks.Subnautica;

[BepInDependency("com.bepis.bepinex.configurationmanager", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[DisallowMultipleComponent]
public class BepInExTweaks : BaseUnityPlugin
{
    public static BepInExTweaks Instance { get; private set; }

    public SceneCleanerTweaks SceneCleanerTweaks { get; private set; }

    private IEnumerable<MonoBehaviour> tweaks = Enumerable.Empty<MonoBehaviour>();
    public List<MonoBehaviour> Tweaks => tweaks?.ToList();

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

    private void OnEnable() => AddTweaks();

    private void AddTweaks() => ThreadingHelper.Instance.StartAsyncInvoke(() =>
    {
        var tweaks = GetAllTweaks();
        return () => this.tweaks = tweaks.Select(type => gameObject.AddComponent(type) as MonoBehaviour).ToList();
    });

    private IEnumerable<Type> GetAllTweaks() => AccessTools.AllTypes()
        .Where(type => (type.IsSubclassOf(typeof(MonoBehaviour)) || type.IsAssignableFrom(typeof(MonoBehaviour)))
        && type.GetCustomAttribute<TweakAttribute>() is not null).ToHashSet();

    private void RemoveTweaks()
    {
        foreach (var tweak in Tweaks)
        {
            Destroy(tweak);
        }
        tweaks = Enumerable.Empty<MonoBehaviour>();
    }

    private void OnDisable() => RemoveTweaks();
}
