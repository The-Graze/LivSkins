using System;
using BepInEx;
using UnityEngine;

namespace LivSkins
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        Plugin()
        {
            HarmonyPatches.ApplyHarmonyPatches();
        }
    }
}
