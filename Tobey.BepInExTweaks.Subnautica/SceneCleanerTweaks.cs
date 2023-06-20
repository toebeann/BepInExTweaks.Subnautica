using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tobey.BepInExTweaks.Subnautica;

[DisallowMultipleComponent]
public class SceneCleanerTweaks : MonoBehaviour
{
    public HashSet<GameObject> GameObjects => new() { Chainloader.ManagerObject, ThreadingHelper.Instance.gameObject };
    public HashSet<SceneCleanerPreserve> SceneCleanerPreserves;

    internal ConfigEntry<bool> AddSceneCleanerPreserve { get; } =
            BepInExTweaks.Instance.Config.Bind(
                section: "General",
                key: "Preserve BepInEx runtime GameObjects",
                defaultValue: true,
                configDescription: new(
                    description: "Prevents important BepInEx GameObjects from being destroyed when the game cleans the scene.",
                    tags: new[] { new ConfigurationManagerAttributes { IsAdvanced = true } }
                )
            );

    private void Awake()
    {
        AddSceneCleanerPreserve.SettingChanged += AddSceneCleanerPreserve_SettingChanged;
        AddSceneCleanerPreserve_SettingChanged(this, null);
    }

    private void AddSceneCleanerPreserve_SettingChanged(object _, EventArgs __)
    {
        if (AddSceneCleanerPreserve.Value)
        {
            enabled = true;
        }
        else
        {
            enabled = false;
        }
    }

    private void OnEnable()
    {
        SceneCleanerPreserves = new();
        foreach (GameObject gameObject in GameObjects)
        {
            if (!gameObject.GetComponent<SceneCleanerPreserve>())
            {
                SceneCleanerPreserves.Add(gameObject.AddComponent<SceneCleanerPreserve>());
            }
        }
    }

    private void OnDisable()
    {
        foreach (SceneCleanerPreserve sceneCleanerPreserve in SceneCleanerPreserves)
        {
            Destroy(sceneCleanerPreserve);
        }
        SceneCleanerPreserves.Clear();
        SceneCleanerPreserves = null;
    }

    private void OnDestroy()
    {
        AddSceneCleanerPreserve.SettingChanged -= AddSceneCleanerPreserve_SettingChanged;
    }
}
