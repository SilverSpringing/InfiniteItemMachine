using HarmonyLib;
using UnityEngine;
using TMPro;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace InfiniteItems
{
    [HarmonyPatch(typeof(LevelGenerator), "StartGenerate")]
    class GenerateBegin
    {
        static void Prefix(LevelGenerator __instance)
        {
            if (Singleton<CoreGameManager>.Instance.currentMode == InfiniteItemsPlugin.Instance.CREATIVE_MODE)
            {
                Singleton<CoreGameManager>.Instance.sceneObject.levelTitle = "CRT";
                __instance.ld = CustomInfiniteItemMachineFloor.GetFloor();
            }
        }
    }

    [HarmonyPatch(typeof(CoreGameManager), "PlayBegins")]
    class AddMP3PlayerPatch
    {
        static void Postfix()
            {
            new GameObject("PurpleLockerMenu", new Type[]
            {
                typeof(PurpleLockerMenu)
            });

            Singleton<PurpleLockerMenu>.Instance.Create();
        }
    }


    [HarmonyPatch(typeof(CoreGameManager), "ReturnToMenu")]
    class ResetMode
    {
        static void Prefix(CoreGameManager __instance)
        {
            if (__instance.currentMode == InfiniteItemsPlugin.Instance.CREATIVE_MODE) CoreGameManager.Instance.currentMode = Mode.Main;
        }
    }

    [HarmonyPatch(typeof(CoreGameManager), "PlayBegins")]
    class DestroyBaldi
    {
        static void Postfix(CoreGameManager __instance)
        {
        }
    }

    [HarmonyPatch(typeof(BaseGameManager), "BeginPlay")]
    class FillMap
    {
        static void Postfix(BaseGameManager __instance)
        {
            if (CoreGameManager.Instance.currentMode == InfiniteItemsPlugin.Instance.CREATIVE_MODE) __instance.CompleteMapOnReady();
        }
    }

    [HarmonyPatch(typeof(MainMenu), "Start")]
    class AddCreativeMode
    {
        static void Postfix(MainMenu __instance)
        {

            GameObject CreativeButton;
            GameObject VAR_TEXT = new GameObject();

            GameObject[] ALL_OBJECTS = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
            foreach (GameObject o in ALL_OBJECTS)
            {
                //SET NEW MODE TEXT
                if (o.name == "ModeText")
                {
                    TMP_Text ModeText = o.GetComponent<TMP_Text>();
                    VAR_TEXT = o;
                    ModeText.font = __instance.transform.Find("Reminder").GetComponent<TMP_Text>().font;
                    ModeText.fontSize = __instance.transform.Find("Reminder").GetComponent<TMP_Text>().fontSize;
                    o.GetComponent<RectTransform>().sizeDelta = new Vector2(360, 128);
                    o.transform.localPosition = new Vector3(0, -112, 0);
                }

                //CREATIVE MODE BUTTON
                if (o.name == "Free")
                {
                    TMP_Text ModeText2ForSomeFuckingReason;

                    foreach (GameObject ob in ALL_OBJECTS) if (o.name == "ModeText") ModeText2ForSomeFuckingReason = o.GetComponent<TMP_Text>();
                    CreativeButton = GameObject.Instantiate(o, o.transform.parent.transform);
                    CreativeButton.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationManager.Instance.GetLocalizedText("Button_Creative");
                    GameObject.Destroy(CreativeButton.GetComponent<TextLocalizer>());
                    CreativeButton.transform.SetSiblingIndex(3);
                    CreativeButton.transform.localPosition = new Vector3(0, -36, 0);

                    CreativeButton.GetComponentInChildren<StandardMenuButton>().OnHighlight.AddListener(() => VAR_TEXT.GetComponent<TMP_Text>().text = LocalizationManager.Instance.GetLocalizedText("Button_Creative_Desc"));
                    CreativeButton.GetComponentInChildren<StandardMenuButton>().OnPress.AddListener(
                        delegate { 
                            CoreGameManager.Instance.currentMode = InfiniteItemsPlugin.Instance.CREATIVE_MODE;
                            SceneManager.LoadScene("Game");
                        }
                   );
                }
            }
        }
    }
}