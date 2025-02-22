﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using System.Linq;
using Liv.Lck.GorillaTag;
using UnityEngine.Events;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.Playables;
using UnityEngine.SearchService;
namespace LivSkins
{
    class ExtraStuffHandler : MonoBehaviour
    {
        GameObject? LeftRightButton, ExtraC, RecordButton, PictureButton;
        TextMeshPro? Lable, SkinName, PhotoText;

        GtNotificationController? notfifC;
        GTLckController? cCont;
        void Start()
        {
            notfifC = transform.FindChildRecursive("Notification").GetComponent<GtNotificationController>();
            cCont = gameObject.GetComponent<GTLckController>();
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

            RecordButton = transform.FindChildRecursive("Record").gameObject;
            RecordButton.transform.localScale = new Vector3(0.4f, 1, 1);
            RecordButton.transform.localPosition = new Vector3(-0.48f, -0.144f, 0);
            RecordButton.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
            RecordButton.transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
            RecordButton.GetComponent<GtRecordButton>()._name = "REC";

            PictureButton = Instantiate(RecordButton, RecordButton.transform.parent);
            Destroy(PictureButton.GetComponent<GtRecordButton>());
            PictureButton.name = "Picture";
            PictureButton.transform.localScale = new Vector3(0.4f, 1, 1);
            PictureButton.transform.localPosition = new Vector3(-0.71f, -0.144f, 0);
            PictureButton.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
            PictureButton.transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
            PictureButton.transform.FindChildRecursive("Action").GetComponent<GtColliderTriggerProcessor>()._onTriggeredEnded.RemoveAllListeners();
            PictureButton.transform.FindChildRecursive("Action").GetComponent<GtColliderTriggerProcessor>()._onTriggeredStarted.RemoveAllListeners();
            PictureButton.transform.FindChildRecursive("Action").GetComponent<GtColliderTriggerProcessor>()._onTriggeredStarted.AddListener(StartPhotoCountdown);
            PictureButton.transform.FindChildRecursive("Action").transform.localPosition = Vector3.zero;
            PictureButton.transform.GetChild(1).GetChild(2).GetComponent<TextMeshPro>().text = "PIC";
            PhotoText = PictureButton.transform.FindChildRecursive("Visuals").GetChild(2).GetComponent<TextMeshPro>();

            if (LeftRightButton != null)
            {
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
        }

        void StartPhotoCountdown()
        {
            StartCoroutine(PhotoCountdown());
        }

        IEnumerator PhotoCountdown()
        {
            if (PhotoText != null)
            {
                PhotoText.text = "3";
                yield return new WaitForSeconds(0.7f);
                PhotoText.text = "2";
                yield return new WaitForSeconds(0.7f);
                PhotoText.text = "1";
                yield return new WaitForSeconds(0.7f);

                TakePicture();

                PhotoText.text = "Done";
                yield return new WaitForSeconds(0.7f);
                PhotoText.text = "PIC";
            }
        }

        public  void TakePicture()
        {
            Camera c = DecideCam();
            RenderTexture activeRenderTexture = RenderTexture.active;
            RenderTexture.active = c.targetTexture;

            c.Render();

            Texture2D image = new Texture2D(c.targetTexture.width, c.targetTexture.height);
            image.ReadPixels(new Rect(0, 0, c.targetTexture.width, c.targetTexture.height), 0, 0);
            image.Apply();
            RenderTexture.active = activeRenderTexture;

            byte[] bytes = image.EncodeToPNG();
            Destroy(image);

            string FileName = $"GorillaTag_{DateTime.Now}";
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Gorilla Tag")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Gorilla Tag"));
            }
            string sanatizedDate = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".png";
            string path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), $"Gorilla Tag","Gorilla Tag_"+sanatizedDate));
            File.WriteAllBytes(path, bytes);
        }
        IEnumerator WaitToSetBack()
        {
            if (notfifC != null)
            {
                yield return new WaitForSeconds(notfifC._notificationShowDuration + 1);
                notfifC._pcMessage.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = "VIDEOS";
            }
            else
            {
                yield return new WaitForSeconds(0);
            }
        }

        private Camera DecideCam()
        {
            if (cCont != null)
            {

                switch (cCont._currentCameraMode)
                {
                    default:
                        return cCont._selfieCamera.GetCameraComponent();
                    case CameraMode.FirstPerson:
                        return cCont._firstPersonCamera.GetCameraComponent();
                    case CameraMode.ThirdPerson:
                        return cCont._thirdPersonCamera.GetCameraComponent();
                    case CameraMode.Selfie:
                        return cCont._selfieCamera.GetCameraComponent();
                }
            }
            else
            {
                return GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>();
            }
        }

        private void Up()
        {
            int i = Array.IndexOf(Plugin.Skins.Keys.ToArray(), Plugin.SelectedSkin.Value);
            if (i >= Plugin.Skins.Keys.Count)
            {
                return;
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
            int i = Array.IndexOf(Plugin.Skins.Keys.ToArray(), Plugin.SelectedSkin.Value);
            if (i <= 0)
            {
                return;
            }
            else
            {
                i--;
                var wawa = Plugin.Skins.Keys.ToList();
                Plugin.SelectedSkin.SetSerializedValue(wawa[i]);
            }
        }

        void FixedUpdate()
        {
            if (SkinName != null)
            {
                string toSet = Plugin.SelectedSkin.Value.ToUpper();
                if (toSet.Count() > 10)
                {
                    toSet = toSet.Substring(0, 6);
                }
                SkinName.text = toSet;
            }
            if (Lable != null)
            {
                if (Lable.text != "SKIN:")
                {
                    Lable.text = "Skin:";
                }
            }
        }
    }
}
