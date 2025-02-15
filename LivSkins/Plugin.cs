using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace LivSkins
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> CamModel;
        public static ConfigEntry<string> SelectedSkin;
        public static List<Renderer> CamRenderers = new List<Renderer>();
        public static Dictionary<string, Texture> Skins = new Dictionary<string, Texture>();
        static bool gotSkins;
        Plugin()
        {
            HarmonyPatches.ApplyHarmonyPatches();
        }

        public static void AddRend(Renderer renderer)
        {
            if (renderer.name == "Body")
            {
                CamRenderers.Add(renderer);
            }
            Plugin.OnSkinChanged(null, null);
        }

        void Start()
        {
            CamModel = Config.Bind("Visuals", "CameraModel", false, "Choose if you would like to use the 'old' Camera model");
            SelectedSkin = Config.Bind("Visuals", "Skin", "", "The name of the skin selected");
            GorillaTagger.OnPlayerSpawned(delegate
            {
                foreach (var spawn in Resources.FindObjectsOfTypeAll<LckWallCameraSpawner>())
                {
                    Transform RendC = spawn.transform.FindChildRecursive("CameraRenderModel");
                    if (Plugin.CamModel.Value)
                    {
                        RendC.GetChild(0).gameObject.SetActive(false);
                        RendC.GetChild(2).gameObject.SetActive(true);
                        Plugin.CamRenderers.Add(RendC.GetChild(2).GetChild(0).GetComponent<Renderer>());
                    }
                    else
                    {
                        Plugin.CamRenderers.Add(RendC.GetChild(0).GetChild(0).GetChild(0).GetComponent<Renderer>());
                    }
                }
                GetSkinImages();
                SelectedSkin.SettingChanged += OnSkinChanged;
                StartCoroutine(DelaySetCosOOpid());
            });
        }

        public static void OnSkinChanged(object sender, EventArgs e)
        {
            if (Skins.Keys.Contains(SelectedSkin.Value))
            {
                foreach (Renderer r in CamRenderers)
                {
                    var mat = r.material;
                    mat.name = Skins[SelectedSkin.Value].name;
                    mat.EnableKeyword("_USE_TEXTURE");
                    mat.DisableKeyword("_USE_TEX_ARRAY_ATLAS");
                    mat.mainTexture = Skins[SelectedSkin.Value];
                }
            }

        }

        void FixedUpdate()
        {
            if (!gotSkins)
            {
                GetSkinImages();
            }
        }

        IEnumerator DelaySetCosOOpid()
        {
            yield return new WaitForSecondsRealtime(3);
            Plugin.OnSkinChanged(null, null);
        }
        static void GetSkinImages()
        {
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string skinPath = "";
            if (!Directory.Exists(Path.Combine(modPath, "SkinImages")))
            {
                skinPath = Directory.CreateDirectory(Path.Combine(modPath, "SkinImages")).Name;
            }
            else
            {
                skinPath = Path.Combine(modPath, "SkinImages");
            }

            foreach (var skin in Directory.GetFiles(skinPath))
            {
                MakeIntoTexture(skin);
            }
        }

        static void MakeIntoTexture(string path)
        {
            Texture2D tex = new Texture2D(2, 2);
            var imgdata = File.ReadAllBytes(path);
            tex.LoadImage(imgdata);
            string name = Path.GetFileNameWithoutExtension(path);
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
