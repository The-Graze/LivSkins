using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using System.Linq;
using Liv.Lck.GorillaTag;
using UnityEngine.Events;
using Liv.Lck.Tablet;
using System.IO;
using CameraMode = Liv.Lck.GorillaTag.CameraMode;
using System.Threading.Tasks;
using System.Collections;
namespace LivSkins
{
    class ExtraStuffHandler : MonoBehaviour
    {
        GameObject? LeftRightButton, ExtraC, RecordButton, PictureButton;
        TextMeshPro? Lable, SkinName;
        GtNotificationController notfifC;
        GTLckController cCont;

        void Start()
        {
            cCont = gameObject.GetComponent<GTLckController>();
            notfifC = gameObject.GetComponent<GtNotificationController>();

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
            RecordButton.GetComponent<GtRecordButton>().RECORD_BUTTON_NAME = "REC";

            PictureButton = Instantiate(RecordButton, RecordButton.transform.parent);
            Destroy(PictureButton.GetComponent<GtRecordButton>());
            PictureButton.name = "Picture";
            PictureButton.transform.localScale = new Vector3(0.4f, 1, 1);
            PictureButton.transform.localPosition = new Vector3(-0.71f, -0.144f, 0);
            PictureButton.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
            PictureButton.transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
            PictureButton.transform.FindChildRecursive("Action").GetComponent<GtColliderTriggerProcessor>()._onTriggeredEnded.RemoveAllListeners();
            PictureButton.transform.FindChildRecursive("Action").GetComponent<GtColliderTriggerProcessor>()._onTriggeredStarted.RemoveAllListeners();
            PictureButton.transform.FindChildRecursive("Action").GetComponent<GtColliderTriggerProcessor>()._onTriggeredStarted.AddListener(TakePicture);

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
            notfifC._notificationText.text = "PHOTO SAVED \r\nSAVED TO PHOTOS/GORILLA TAG: " + Path.GetFileNameWithoutExtension(path);
            notfifC._notification.SetActive(true);
            notfifC.StartCoroutine(notfifC.NotificationTimer());
        }

        private Camera DecideCam()
        {
            switch (cCont._currentCameraMode)
            {
                default:
                    return cCont._selfieCamera._camera;
                case CameraMode.FirstPerson:
                    return cCont._firstPersonCamera._camera;
                    break;
                case CameraMode.ThirdPerson:
                    return cCont._thirdPersonCamera._camera;
                case CameraMode.Selfie:
                    return cCont._selfieCamera._camera;
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
            if (Lable.text != "SKIN:")
            {
                Lable.text = "Skin:";
            }
            if (PictureButton != null)
            {
                if (PictureButton.transform.GetChild(1).GetChild(2).GetComponent<TextMeshPro>() != null)
                {
                    if (PictureButton.transform.GetChild(1).GetChild(2).GetComponent<TextMeshPro>().text != "PIC")
                    {
                        PictureButton.transform.GetChild(1).GetChild(2).GetComponent<TextMeshPro>().text = "PIC";
                    }
                }
            }

        }
    }
}
