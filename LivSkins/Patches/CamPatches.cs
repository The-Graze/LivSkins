using HarmonyLib;
using UnityEngine;

namespace LivSkins.Patches
{
    [HarmonyPatch(typeof(TabletSpawnInstance))]
    [HarmonyPatch("SpawnCamera", MethodType.Normal)]
    internal static class SpawnPatch
    {
        private static void Postfix(TabletSpawnInstance __instance)
        {
            Plugin.AddRend(__instance._cameraGameObjectInstance.transform.FindChildRecursive("Powered By LIV").parent
                .GetChild(0).GetComponent<Renderer>());
            __instance._cameraGameObjectInstance.AddComponent<SkinHandler>();
        }
    }

    [HarmonyPatch(typeof(LckBodyCameraSpawner))]
    [HarmonyPatch("OnEnable", MethodType.Normal)]
    internal static class BodyMakePatch
    {
        private static void Postfix(LckBodyCameraSpawner __instance)
        {
            var RendC = __instance.transform.FindChildRecursive("CameraRenderModel");
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

    [HarmonyPatch(typeof(LckWallCameraSpawner))]
    [HarmonyPatch("CreatePrewarmCamera", MethodType.Normal)]
    internal static class WallMakePatch
    {
        private static void Postfix(LckBodyCameraSpawner __instance)
        {
            try
            {
                var RendC = __instance.transform.FindChildRecursive("CameraRenderModel");
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
            catch
            {
                // ignored
            }
        }
    }
}