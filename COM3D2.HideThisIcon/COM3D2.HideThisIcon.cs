using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
using System.Diagnostics;
using wf;
using System;
using UnityEngine.SceneManagement;


namespace COM3D2.HideThisIcon
{
    [BepInPlugin("COM3D2.HideThisIcon", "Hide This Icon", "1.0")]
    public sealed partial class HideThisIcon : BaseUnityPlugin
    {
        public static HideThisIcon Instance { get; private set; }

        // Static property for the logger so you can log from other classes.
        internal static new ManualLogSource Logger => Instance?.BepInExLogger;
        private ManualLogSource BepInExLogger => base.Logger;

        internal static ConfigEntry<bool> isAutoHideEnabled;

        // Save settings
        private static string jsonPath = BepInEx.Paths.ConfigPath + "\\COM3D2.HideThisIcon.json";

        private static HashSet<string> hiddenMenus = [];
        private static bool showHiddenIcons = false;
        private static UISprite partPanelBase;
        private static UISprite itemPanelBase;
        private static UISprite setPanelBase;


        private static int childListAccessCount = 0;

        private void Awake()
        {
            // Useful for engaging coroutines or accessing non-static variables. Completely optional though.
            Instance = this;

            // Binds the configuration. In other words it sets your ConfigEntry var to your config setup.
            isAutoHideEnabled = Config.Bind("Config", "Enable Auto Hide", false, "Will hide automatically all mods in a folder ending by _HideThisIcon or _HTI");

            Harmony.CreateAndPatchAll(typeof(HideThisIconPatch));

            // Loading from JSon
            LoadJson(jsonPath);

            if (isAutoHideEnabled.Value)
            {
                string modFolderPath = Path.Combine(UTY.gameProjectPath, "Mod");
                IEnumerable<string> tohideList = Directory.GetFiles(modFolderPath, "*.*", SearchOption.AllDirectories)
                                                          .Where(g => Path.GetExtension(g).ToLower() == ".menu")
                                                          .Where(f => Path.GetDirectoryName(f).ToLower().Contains("_hidethisicon") || Path.GetDirectoryName(f).ToLower().Contains("_hti"))                                               
                                                          .Select(h => Path.GetFileName(h).ToLower());

               // foreach(string str in tohideList)
               //     Logger.LogDebug($"Adding to AutoHide: {str}");

                hiddenMenus.UnionWith(tohideList);
            }
        }

        private static void LoadJson(string path)
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                hiddenMenus = JsonConvert.DeserializeObject<HashSet<string>>(json);
            }
        }

        private static void SaveJson(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(hiddenMenus));
        }

        private static void ToggleMode(SceneEdit sceneEdit, bool forceOff = false)
        {
            showHiddenIcons = !forceOff && !showHiddenIcons;

#if DEBUG
            Logger.LogInfo($"showHiddenIcons was set to {showHiddenIcons}");
#endif

            //refresh the current panel
            //var menuButtonList = sceneEdit.m_listBtnMenuItem.Where(m => hiddenMenus.Contains(m.gcBtnEdit.m_MenuItem.m_strMenuFileName))
            //                                                .Select(b => b.gcBtnEdit);

            //SetIconState(menuButtonList);
            //sceneEdit.m_Panel_MenuItem.gcUIGrid?.Reposition();
            //sceneEdit.m_Panel_SetItem.gcUIGrid?.Reposition();
            //sceneEdit.m_Panel_MenuItem.ResetScrollPos(0f);

            SetBackgroundColor();

            //save when the hide mode is disabled
            if (!showHiddenIcons)
                SaveJson(jsonPath);
        }

        private static void SetIconState(IEnumerable<ButtonEdit> buttonList)
        {
#if DEBUG
            Logger.LogInfo($"Set State for {buttonList.Count()} icons");
#endif
            foreach (var button in buttonList)
            {
                if (showHiddenIcons)
                {
                    button.transform.parent.gameObject.SetActive(true);
                    UI2DSprite buttonSprite = button.GetComponentInChildren<UI2DSprite>();
                    buttonSprite.width = 40;
                    buttonSprite.height = 40;
                }
                else
                    button.transform.parent.gameObject.SetActive(false);
            }
        }

        private static void SetBackgroundColor()
        {
            if (partPanelBase == null) partPanelBase = GameObject.Find("UI Root/ScrollPanel-PartsType/Base")?.GetComponentInChildren<UISprite>();
            if (partPanelBase != null) partPanelBase.color = showHiddenIcons ? Color.red : Color.white;

            if (itemPanelBase == null) itemPanelBase = GameObject.Find("UI Root/ScrollPanel-MenuItem/Base")?.GetComponentInChildren<UISprite>();
            if (itemPanelBase != null) itemPanelBase.color = showHiddenIcons? Color.red : Color.white;

            if (setPanelBase == null) setPanelBase = GameObject.Find("UI Root/ScrollPanel-SetItem/Base")?.GetComponentInChildren<UISprite>();
            if (setPanelBase != null) setPanelBase.color = showHiddenIcons ? Color.red : Color.white;
        }



        static class HideThisIconPatch
        {
            [HarmonyPatch(typeof(SceneEdit), nameof(SceneEdit.ClickCallback))]
            [HarmonyPrefix]
            public static bool HandleClickPrefix(ref SceneEdit __instance)
            {
#if DEBUG
                Logger.LogMessage("---------------------------------------------");
                childListAccessCount = 0;
#endif

                bool isRightClick = Input.GetMouseButtonUp(1);

                ButtonEdit buttonEdit = UIButton.current?.GetComponentInChildren<ButtonEdit>();

                if (buttonEdit != null)
                {
                    //only register right click GetMouseButtonUp(1)
                    if (isRightClick)
                    {
                        //change icon size and add/remove from the hidden list
                        if (buttonEdit.m_MenuItem != null && showHiddenIcons)
                        {
                            string menuFileName = buttonEdit.m_MenuItem.m_strMenuFileName;
#if DEBUG                            
                            Logger.LogInfo($"Right Click was registered on {menuFileName}");
#endif
                            UI2DSprite buttonSprite = UIButton.current.GetComponentInChildren<UI2DSprite>();

                            if (!hiddenMenus.Contains(menuFileName))
                            {
                                buttonSprite.width = 40;
                                buttonSprite.height = 40;
                                hiddenMenus.Add(menuFileName);
                            }
                            else
                            {
                                buttonSprite.width = 80;
                                buttonSprite.height = 80;
                                hiddenMenus.Remove(menuFileName);
                            }
                        }

                        //toggle hide mode by right clicking on a category
                        else if (buttonEdit.m_PartsType != null) ToggleMode(__instance);

                        return false;                        
                    }

                    //disable hide mode if the category is changed
                    if (buttonEdit.m_Category != null && showHiddenIcons) ToggleMode(__instance, true);
                }
                return true;
            }

            [HarmonyPatch(typeof(SceneEdit), nameof(SceneEdit.ClickCallback))]
            [HarmonyPostfix]
            public static void HandleClickPostfix(ref SceneEdit __instance)
            {
                ButtonEdit buttonEdit = UIButton.current?.GetComponentInChildren<ButtonEdit>();

                if (buttonEdit != null && buttonEdit.m_PartsType != null)
                {
                    var menuButtonList = __instance.m_listBtnMenuItem.Where(m => hiddenMenus.Contains(m.gcBtnEdit.m_MenuItem.m_strMenuFileName))
                                                                     .Select(b => b.gcBtnEdit);

                    SetIconState(menuButtonList);
                    __instance.m_Panel_MenuItem.gcUIGrid?.Reposition();
                    __instance.m_Panel_SetItem.gcUIGrid?.Reposition();
                }
            }

            //removes gaps left by hidden items
            [HarmonyPatch(typeof(UIGrid), nameof(UIGrid.GetChildList))]
            [HarmonyPostfix]
            public static void GetChildListPostFix(ref List<Transform> __result)
            {
                if (SceneManager.GetActiveScene().name != "SceneEdit") return;

#if DEBUG
                childListAccessCount++;
                Logger.LogInfo($"Child List Access Count = {childListAccessCount}");
                //Logger.LogWarning($"StrackTrace: {Environment.StackTrace}");

                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logger.LogInfo($"Old Child List Count: {__result.Count}");
#endif
                    __result = __result.Where(c => c.gameObject.activeSelf).ToList();
#if DEBUG
                Logger.LogInfo($"New Child List Count: {__result.Count}");
                sw.Stop();
                Logger.LogInfo($"List creation time: {sw.ElapsedMilliseconds}ms");
#endif
            }
        }
    }
}