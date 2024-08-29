using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.AccessControl;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static UIAtlas;


namespace COM3D2.HideThisIcon
{
    public sealed partial class HideThisIcon : BaseUnityPlugin
    {
        internal static void AddToggleButton(string buttonName)
        {
            //avoid creating the same button object twice
            if (GameObject.Find(buttonName) != null) return;

            //getting the parent and the template
            GameObject leftButtonGroup = GameObject.Find("LeftButtons");
            GameObject viewButton = GameObject.Find("View");
            GameObject okButton = GameObject.Find("UI Root/OkCancel/Ok");
            //GameObject uIRoot = GameObject.Find("UI Root");


            // duplicate the button template we found above
            GameObject toggleButton = NGUITools.AddChild(leftButtonGroup, viewButton);
            toggleButton.name = buttonName;

            //Colored button Text
            //moving it in position
            toggleButton.transform.localPosition = new Vector3(-75f, 0.5f, 0f);
            toggleButton.transform.localScale = new Vector3(0.4f, 1, 0.4f);

            //UISprite (button texture)            
            UISprite uiSprite = toggleButton.GetComponentInChildren<UISprite>(true);
            uiSprite.spriteName = showHiddenIcons ? "cm3d2_edit_scrollbutton_up" : "cm3d2_edit_scrollbutton_down";

            //TweenColor component (what changed the color on mouse hover)
            TweenColor tweenColor = toggleButton.GetComponentInChildren<TweenColor>(true);
            tweenColor.from = Color.red;
            tweenColor.to = Color.green;
            

            //UIButton (reacts to click)
            UIButton uiButton = toggleButton.GetComponentInChildren<UIButton>(true);
            uiButton.onClick.Clear();
            EventDelegate.Add(uiButton.onClick, new EventDelegate.Callback(delegate
            {
                Logger.LogInfo("State toggled");
                showHiddenIcons = !showHiddenIcons;
                uiSprite.spriteName = showHiddenIcons ? "cm3d2_edit_scrollbutton_up" : "cm3d2_edit_scrollbutton_down";
            }));




            //Buttonn with text TEST
            /*
            //moving it in position
            toggleButton.transform.localPosition = new Vector3(1.4f, 37.5f, 0f);
            toggleButton.transform.localScale = new Vector3(0.86f, 0.66f, 1f);


            UISprite uiSprite = toggleButton.GetComponentInChildren<UISprite>(true);
            //uiSprite.atlas = Resources.Load<UIAtlas>("commonUI/Atlas/AtlasCommon");
            uiSprite.spriteName = "cm3d2_edit_Category01button_base";
            //uiSprite.spriteName = "cm3d2_common_plate_white";

            //UILabel (the button text)
                        UILabel label = uiSprite.gameObject.AddComponent<UILabel>();
                        label.width = uiSprite.width - 20;
                        label.height = uiSprite.height - 20;
                        label.trueTypeFont = GameObject.Find("SystemUI Root").GetComponentsInChildren<UILabel>(true).First(lab => lab.mTrueTypeFont != null && lab.mTrueTypeFont.name.Equals("NotoSansCJKjp-DemiLight")).trueTypeFont;
                        label.color = new Color(0, 0, 0);
                        label.fontSize = 21;
                        label.depth = uiSprite.depth + 1;
                        label.text = showHiddenIcons? "Hide": "Show";

            //UIButton (reacts to click)
            UIButton uiButton = toggleButton.GetComponentInChildren<UIButton>(true);
            uiButton.onClick.Clear();
            EventDelegate.Add(uiButton.onClick, new EventDelegate.Callback(delegate
            {
                Logger.LogInfo("State toggled");
                showHiddenIcons = !showHiddenIcons;
                label.text = showHiddenIcons ? "Hide" : "Show";
            }));
            */
        }

        private static void ToggleState()
        {
            Logger.LogInfo("State toggled");
            showHiddenIcons = !showHiddenIcons;
        }
    }
}

