using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tobey.BepInExTweaks.Subnautica;

[BepInDependency("com.bepis.bepinex.configurationmanager", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("Tobey.FileTree", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[DisallowMultipleComponent]
public class BepInExTweaks : BaseUnityPlugin
{
    public static BepInExTweaks Instance { get; private set; }

    public HashSet<Type> TweakTypes { get; } = new(new[]
    {
        typeof(FileTreeTweaks),
        typeof(SceneCleanerTweaks)
    });

    public HashSet<MonoBehaviour> Tweaks { get; } = new();

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

    private void OnEnable() => ThreadingHelper.Instance.StartAsyncInvoke(() =>
    {
        return () => TweakTypes
            .Select(type => gameObject.AddComponent(type) as MonoBehaviour)
            .AsParallel()
            .ForAll((tweak) => Tweaks.Add(tweak));
    });

    private void OnDisable()
    {
        Tweaks.AsParallel().ForAll(Destroy);
        Tweaks.Clear();
    }
}
