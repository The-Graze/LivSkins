using System;
using System.Linq;
using Liv.Lck.GorillaTag;
using TMPro;
using UnityEngine;

namespace LivSkins
{
    internal class SkinHandler : MonoBehaviour
    {
        private TextMeshPro? Lable, SkinName;
        private GameObject? LeftRightButton, ExtraC;

        private void Start()
        {
            ExtraC = new GameObject("Extra Menu");
            ExtraC.transform.SetParent(transform.FindChildRecursive("Settings Group"));
            ExtraC.transform.localPosition = new Vector3(-0.07f, -0.11f, 0.1f);
            ExtraC.transform.localRotation = Quaternion.Euler(315, 0, 0);
            ExtraC.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);

            LeftRightButton = Instantiate(transform.FindChildRecursive("FOV").gameObject, ExtraC.transform, false);
            LeftRightButton.transform.localPosition = Vector3.zero;
            LeftRightButton.transform.localRotation = Quaternion.Euler(Vector3.zero);
            LeftRightButton.name = "SkinSelect";

            Lable = LeftRightButton?.transform.FindChildRecursive("Label").GetComponent<TextMeshPro>();

            SkinName = LeftRightButton?.transform.FindChildRecursive("Value").GetComponent<TextMeshPro>();

            if (LeftRightButton != null)
                foreach (Transform button in LeftRightButton.transform.FindChildRecursive("Triggers").transform)
                {
                    if (button.name == "Decrement")
                    {
                        button.GetComponent<GtColliderTriggerProcessor>()._onTriggeredEnded.RemoveAllListeners();
                        button.GetComponent<GtColliderTriggerProcessor>()._onTriggeredStarted.RemoveAllListeners();
                        button.GetComponent<GtColliderTriggerProcessor>()._onTriggeredStarted.AddListener(Down);
                    }

                    if (button.name == "Increment")
                    {
                        button.GetComponent<GtColliderTriggerProcessor>()._onTriggeredEnded.RemoveAllListeners();
                        button.GetComponent<GtColliderTriggerProcessor>()._onTriggeredStarted.RemoveAllListeners();
                        button.GetComponent<GtColliderTriggerProcessor>()._onTriggeredStarted.AddListener(Up);
                    }
                }
        }

        private void Update()
        {
            if (SkinName != null)
            {
                var toSet = Plugin.SelectedSkin.Value.ToUpper();
                if (toSet.Count() > 10) toSet = toSet.Substring(0, 6);
                SkinName.text = toSet;
            }

            if (Lable != null)
                if (Lable.text != "SKIN:")
                    Lable.text = "Skin:";
        }

        private void Up()
        {
            var i = Array.IndexOf(Plugin.Skins.Keys.ToArray(), Plugin.SelectedSkin.Value);
            if (i >= Plugin.Skins.Keys.Count)
            {
            }
            else
            {
                i++;
                var wawa = Plugin.Skins.Keys.ToList();
                Plugin.SelectedSkin.SetSerializedValue(wawa[i]);
            }
        }

        private void Down()
        {
            var i = Array.IndexOf(Plugin.Skins.Keys.ToArray(), Plugin.SelectedSkin.Value);
            if (i <= 0)
            {
            }
            else
            {
                i--;
                var wawa = Plugin.Skins.Keys.ToList();
                Plugin.SelectedSkin.SetSerializedValue(wawa[i]);
            }
        }
    }
}