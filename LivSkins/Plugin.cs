using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LivSkins
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> CamModel;
        public static ConfigEntry<string> SelectedSkin;
        public static List<Renderer> CamRenderers = new List<Renderer>();
        public static Dictionary<string, Texture> Skins = new Dictionary<string, Texture>();
        private static bool gotSkins;
        
        LckBodyCameraSpawner BodyCameraSpawner;

        private Plugin()
        {
            new Harmony(PluginInfo.GUID).PatchAll(Assembly.GetExecutingAssembly());
        }

        private void Start()
        {
            CamModel = Config.Bind("Visuals", "CameraModel", false,
                "Choose if you would like to use the 'old' Camera model");
            SelectedSkin = Config.Bind("Visuals", "Skin", "", "The name of the skin selected");
            GorillaTagger.OnPlayerSpawned(delegate
            {
                foreach (var spawn in Resources.FindObjectsOfTypeAll<LckWallCameraSpawner>())
                {
                    var RendC = spawn.transform.FindChildRecursive("CameraRenderModel");
                    if (CamModel.Value)
                    {
                        RendC.GetChild(0).gameObject.SetActive(false);
                        RendC.GetChild(2).gameObject.SetActive(true);
                        CamRenderers.Add(RendC.GetChild(2).GetChild(0).GetComponent<Renderer>());
                    }
                    else
                    {
                        CamRenderers.Add(RendC.GetChild(0).GetChild(0).GetChild(0).GetComponent<Renderer>());
                    }
                }

                GetSkinImages();
                SelectedSkin.SettingChanged += OnSkinChanged;
                StartCoroutine(DelaySetCosOOpid());
            });
        }

        private void FixedUpdate()
        {
            if(!gotSkins){ GetSkinImages();}
            else
            {
                if (Keyboard.current.tabKey.wasPressedThisFrame)
                {
                    if (!BodyCameraSpawner)
                    {
                        BodyCameraSpawner = GTPlayer.Instance.transform.GetComponentInChildren<LckBodyCameraSpawner>();
                    }

                    if (!BodyCameraSpawner.tabletSpawnInstance.isSpawned)
                    {
                        BodyCameraSpawner.SpawnCamera(GTPlayer.Instance.GetComponentInChildren<GorillaGrabber>(), Camera.main.transform);
                    }
                }
            }
        }

        public static void AddRend(Renderer renderer)
        {
            if (renderer.name == "Body") CamRenderers.Add(renderer);
            OnSkinChanged(null, null);
        }

        public static void OnSkinChanged(object sender, EventArgs e)
        {
            if (Skins.Keys.Contains(SelectedSkin.Value))
                foreach (var r in CamRenderers)
                {
                    var mat = r.material;
                    mat.name = Skins[SelectedSkin.Value].name;
                    mat.EnableKeyword("_USE_TEXTURE");
                    mat.DisableKeyword("_USE_TEX_ARRAY_ATLAS");
                    mat.mainTexture = Skins[SelectedSkin.Value];
                }
        }

        private IEnumerator DelaySetCosOOpid()
        {
            yield return new WaitForSecondsRealtime(3);
            OnSkinChanged(null, null);
        }

        private static void GetSkinImages()
        {
            var modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var skinPath = "";
            if (!Directory.Exists(Path.Combine(modPath, "SkinImages")))
                skinPath = Directory.CreateDirectory(Path.Combine(modPath, "SkinImages")).Name;
            else
                skinPath = Path.Combine(modPath, "SkinImages");

            foreach (var skin in Directory.GetFiles(skinPath)) MakeIntoTexture(skin);
        }

        private static void MakeIntoTexture(string path)
        {
            var tex = new Texture2D(2, 2);
            var imgdata = File.ReadAllBytes(path);
            tex.LoadImage(imgdata);
            var name = Path.GetFileNameWithoutExtension(path);
            tex.name = name;
            tex.filterMode = FilterMode.Point;
            if (!Skins.Keys.Contains(name))
            {
                Skins.Add(name, tex);
                gotSkins = true;
            }
        }
    }
}