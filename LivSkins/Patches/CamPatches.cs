using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using HarmonyLib;
using Liv.Lck.GorillaTag;
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
            Plugin.OnSkinChanged(null,null);
        }
    }
    [HarmonyPatch(typeof(LckWallCameraSpawner))]
    [HarmonyPatch("CreatePrewarmCamera", MethodType.Normal)]
    static class WallMakePatch
    {
        static void Postfix(LckBodyCameraSpawner __instance)
        {
            try
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
            catch
            {

            }
        }
    }
    [HarmonyPatch(typeof(GtRecordButton))]
    [HarmonyPatch("UpdateVisualState", MethodType.Normal)]
    static class RecPatch
    {
        static bool Prefix(GtRecordButton __instance)
        {
            __instance._bodyRenderer.material = ((__instance._currentState == GtRecordButton.State.Recording) ? __instance._settings.RecordingBodyMaterial : __instance._settings.DefaultBodyMaterial);
            __instance._label.color = ((__instance._currentState == GtRecordButton.State.Recording) ? __instance._settings.SecondaryTextColor : __instance._settings.PrimaryTextColor);
            switch (__instance._currentState)
            {
                case GtRecordButton.State.Idle:
                    __instance._label.text = "REC";
                    break;
                case GtRecordButton.State.Saving:
                    __instance._label.text = "SAV";
                    break;
                default:
                    __instance._label.text = "00:00";
                    break;
            }
            return false;
        }
    }
}
