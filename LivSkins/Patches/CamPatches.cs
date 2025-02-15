using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace LivSkins.Patches
{
    [HarmonyPatch(typeof(TabletSpawnInstance))]
    [HarmonyPatch("SpawnCamera", MethodType.Normal)]
    static class SpawnPatch
    {
        static void Postfix(TabletSpawnInstance __instance)
        {
            Plugin.AddRend(__instance._cameraGameObjectInstance.transform.FindChildRecursive("Powered By LIV").parent.GetChild(0).GetComponent<Renderer>());
            __instance._cameraGameObjectInstance.AddComponent<ExtraStuffHandler>();
        }
    }
    [HarmonyPatch(typeof(LckBodyCameraSpawner))]
    [HarmonyPatch("OnEnable", MethodType.Normal)]
    static class BodyMakePatch
    {
        static void Postfix(LckBodyCameraSpawner __instance)
        {
            Transform RendC = __instance.transform.FindChildRecursive("CameraRenderModel");
            if (Plugin.CamModel.Value)
            {
                RendC.GetChild(2).gameObject.SetActive(false);
                RendC.GetChild(0).gameObject.SetActive(true);
                Plugin.CamRenderers.Add(RendC.GetChild(2).GetChild(0).GetComponent<Renderer>());
            }
            else
            {
                Plugin.CamRenderers.Add(RendC.GetChild(2).GetChild(0).GetChild(0).GetComponent<Renderer>());
            }
            Plugin.OnSkinChanged(null, null);
        }
    }
}
