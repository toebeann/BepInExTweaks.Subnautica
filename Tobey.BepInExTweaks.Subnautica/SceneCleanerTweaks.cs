using BepInEx;
using BepInEx.Bootstrap;
using System.Collections.Generic;
using UnityEngine;

namespace Tobey.BepInExTweaks.Subnautica;

[DisallowMultipleComponent]
public class SceneCleanerTweaks : MonoBehaviour
{
    public SceneCleanerPreserve SceneCleanerPreserve { get; private set; }

    public HashSet<GameObject> GameObjects => new() { Chainloader.ManagerObject, ThreadingHelper.Instance.gameObject };
    private HashSet<SceneCleanerPreserve> sceneCleanerPreserves;

    private void OnEnable()
    {
        sceneCleanerPreserves = new();
        foreach (GameObject gameObject in GameObjects)
        {
            if (!gameObject.GetComponent<SceneCleanerPreserve>())
            {
                sceneCleanerPreserves.Add(gameObject.AddComponent<SceneCleanerPreserve>());
            }
        }
    }

    private void OnDisable()
    {
        foreach (SceneCleanerPreserve sceneCleanerPreserve in sceneCleanerPreserves)
        {
            Destroy(sceneCleanerPreserve);
        }
        sceneCleanerPreserves.Clear();
        sceneCleanerPreserves = null;
    }
}
