using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace MenuEdit
{
    public class MainTweak : MonoBehaviour
    {
        Rect menuRect = new Rect((Screen.width - 450f) / 2f, (Screen.height - 550f) / 2f, 450f, 550f);
        bool menuOpen = false;
        int menuIndex = 0;

        string menuTitle = "MenuEdit",
               subTitle = "Main Menu";

        string profileName = "";
        bool setProfileAsDefault = false;

        int profileEditorIndex = 0;
        Vector2 objectScroll = Vector2.zero,
                mediaScroll = Vector2.zero,
                profileScroll = Vector2.zero,
                fontScroll = Vector2.zero;

        Replacer.ComponentType currentType;
        Text currentTC;
        Image currentIMG;
        VideoPlayer currentVP;

        string changedText = "",
               changedFontName = "";

        void Start()
        {
            Dictionaries.Initialize();
            Config.Initialize();
            SceneManager.sceneLoaded += OnSceneChanged;
            if (Config.CurrentProfile != null)
            {
                ReplaceAll("Main Menu");
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(Config.OpenKey)) { menuOpen = !menuOpen; }
        }

        void LateUpdate()
        {
            foreach (var tc in Utils.GetComponents<Text>())
            {
                foreach (var replacer in Config.CurrentProfile.GetTextsForScene(SceneManager.GetActiveScene().name))
                {
                    if (replacer.ObjectName == tc.name)
                    {
                        replacer.Replace(tc);
                    }
                }
            }
            foreach (var img in Utils.GetComponents<Image>())
            {
                foreach (var replacer in Config.CurrentProfile.GetObjectsForScene(SceneManager.GetActiveScene().name))
                {
                    if (replacer.ObjectName == img.name)
                    {
                        replacer.Replace(img);
                    }
                }
            }

        }

        void OnGUI()
        {
            if (menuOpen) { menuRect = GUILayout.Window(0, menuRect, OnWindow, menuTitle + " [" + subTitle + "]"); }
        }

        void OnSceneChanged(Scene scene, LoadSceneMode mode)
        {
            ReplaceAll(scene.name);
        }

        void ReplaceAll(string sceneName)
        {
            if (Config.CurrentProfile == null) return;
            foreach (var vp in Utils.GetComponents<VideoPlayer>())
            {
                foreach (var replacer in Config.CurrentProfile.GetObjectsForScene(sceneName))
                {
                    if (replacer.ObjectName == vp.name)
                    {
                        replacer.Replace(vp);
                    }
                }
            }
        }

        void OnWindow(int ID)
        {
            switch (ID)
            {
                case 0:
                    switch (menuIndex)
                    {
                        case 0:
                            subTitle = "Main Menu";
                            if (GUILayout.Button("Create Profile")) { menuIndex = 1; }
                            if (GUILayout.Button("Change Profile")) { menuIndex = 4; }
                            if (Config.CurrentProfile != null)
                            {
                                if (GUILayout.Button("Edit " + Config.CurrentProfile.ProfileName)) { menuIndex = 2; }
                            }
                            if (GUILayout.Button("Settings")) { menuIndex = 99; }
                            break;
                        case 1:
                            subTitle = "Create Profile";
                            GUILayout.Label("Profile Name");
                            profileName = GUILayout.TextField(profileName);
                            setProfileAsDefault = GUILayout.Toggle(setProfileAsDefault, "Set as Default Profile");
                            if (GUILayout.Button("Save Profile"))
                            {
                                Config.CreateProfile(profileName, setProfileAsDefault, false);
                                profileName = "";
                                setProfileAsDefault = false;
                                menuIndex = 0;
                            }
                            if (GUILayout.Button("Save & Switch to Profile"))
                            {
                                Config.CreateProfile(profileName, setProfileAsDefault, true);
                                profileName = "";
                                setProfileAsDefault = false;
                                menuIndex = 0;
                                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                            }
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("Back to Main Menu")) { menuIndex = 0; }
                            break;
                        case 2:
                            subTitle = "Edit " + Config.CurrentProfile.ProfileName;
                            switch (profileEditorIndex)
                            {
                                case 0:
                                    DrawEditorHeader("Add Objects");
                                    objectScroll = GUILayout.BeginScrollView(objectScroll);                                  
                                    GUILayout.Label("UI Images");
                                    foreach (var img in Utils.GetComponents<Image>())
                                    {
                                        if (!Config.CurrentProfile.ObjectExists(img.name))
                                        {
                                            if (GUILayout.Button("Add " + img.name)) { currentIMG = img; currentType = Replacer.ComponentType.Image; menuIndex = 3; }
                                        }
                                    }
                                    GUILayout.Label("UI Text");
                                    foreach (var tc in Utils.GetComponents<Text>())
                                    {
                                        if (!Config.CurrentProfile.TextExists(tc.name) && tc.text != "")
                                        {
                                            if (GUILayout.Button("Add " + tc.name)) { currentTC = tc; currentType = Replacer.ComponentType.Text; menuIndex = 3; }
                                        }
                                    }
                                    GUILayout.Label("UI Videos");
                                    foreach (var vp in Utils.GetComponents<VideoPlayer>())
                                    {
                                        if (!Config.CurrentProfile.ObjectExists(vp.name))
                                        {
                                            if (GUILayout.Button("Add " + vp.name)) { currentVP = vp; currentType = Replacer.ComponentType.VideoPlayer; menuIndex = 3; }
                                        }
                                    }
                                    GUILayout.EndScrollView();
                                    if (GUILayout.Button("Back to Main Menu")) { menuIndex = 0; }
                                    break;
                                case 1:
                                    DrawEditorHeader("Edit Objects");
                                    GUILayout.Label("UI Images");
                                    foreach (var obj in Utils.GetComponents<Image>())
                                    {
                                        foreach (var item in Config.CurrentProfile.GetObjectsForScene(SceneManager.GetActiveScene().name))
                                        {
                                            if (obj.name == item.ObjectName)
                                            {
                                                if (GUILayout.Button("Edit " + item.ObjectName)) { currentIMG = obj; currentType = Replacer.ComponentType.Image; menuIndex = 3; }
                                            }
                                        }
                                    }
                                    GUILayout.Label("UI Text");
                                    foreach (var obj in Utils.GetComponents<Text>())
                                    {
                                        foreach (var item in Config.CurrentProfile.GetTextsForScene(SceneManager.GetActiveScene().name))
                                        {
                                            if (obj.name == item.ObjectName)
                                            {
                                                if (GUILayout.Button("Edit " + item.ObjectName)) { currentTC = obj; currentType = Replacer.ComponentType.Text; menuIndex = 3; }
                                            }
                                        }
                                    }
                                    GUILayout.Label("UI Videos");
                                    foreach (var obj in Utils.GetComponents<VideoPlayer>())
                                    {
                                        foreach (var item in Config.CurrentProfile.GetObjectsForScene(SceneManager.GetActiveScene().name))
                                        {
                                            if (obj.name == item.ObjectName)
                                            {
                                                if (GUILayout.Button("Edit " + item.ObjectName)) { currentVP = obj; currentType = Replacer.ComponentType.VideoPlayer; menuIndex = 3; }
                                            }
                                        }
                                    }
                                    break;
                                case 2:
                                    DrawEditorHeader("Remove Objects");
                                    break;
                                case 3:
                                    DrawEditorHeader("Edit Profile");
                                    break;
                                default:
                                    menuIndex = 0;
                                    return;
                            }
                            break;
                        case 3:
                            string objectName = "";
                            if (currentType == Replacer.ComponentType.Image) { if (currentIMG == null) { mediaScroll = Vector2.zero; menuIndex = 2; } objectName = subTitle = currentIMG.name; }
                            if (currentType == Replacer.ComponentType.Text) { if (currentTC == null) { mediaScroll = Vector2.zero; menuIndex = 2; } objectName = subTitle = currentTC.name; }
                            if (currentType == Replacer.ComponentType.VideoPlayer) { if (currentVP == null) { mediaScroll = Vector2.zero; menuIndex = 2; } objectName = subTitle = currentVP.name; }
                            if (currentType != Replacer.ComponentType.Text)
                            {
                                GUILayout.Label("Media");
                                mediaScroll = GUILayout.BeginScrollView(mediaScroll);
                                foreach (string file in Config.GetAllMediaFiles())
                                {
                                    if (GUILayout.Button("Change to " + file))
                                    {
                                        var filePath = Config.GetPathForMediaFile(file);
                                        var replacer = Config.CurrentProfile.UpdateProfile(objectName, currentType.ToString(), SceneManager.GetActiveScene().name, filePath);
                                        switch (currentType)
                                        {
                                            case Replacer.ComponentType.Image:
                                                replacer.Replace(currentIMG);
                                                break;
                                            case Replacer.ComponentType.VideoPlayer:
                                                replacer.Replace(currentVP);
                                                break;
                                            default:
                                                return;
                                        }
                                    }
                                }
                                GUILayout.EndScrollView();
                            }
                            else
                            {
                                GUILayout.Label("Change Text (Current: " + currentTC.text + ")");
                                changedText = GUILayout.TextField(changedText);
                                if (GUILayout.Button("Change Text"))
                                {
                                    var replacer = Config.CurrentProfile.UpdateProfile(objectName, currentType.ToString(), SceneManager.GetActiveScene().name, changedFontName, changedText);
                                    replacer.Replace(currentTC);
                                }
                                mediaScroll = GUILayout.BeginScrollView(mediaScroll);
                                GUILayout.Label("Fonts");
                                foreach (var fontName in Font.GetOSInstalledFontNames())
                                {
                                    if (GUILayout.Button("Change font to " + fontName))
                                    {
                                        changedFontName = fontName;
                                        var replacer = Config.CurrentProfile.UpdateProfile(objectName, currentType.ToString(), SceneManager.GetActiveScene().name, changedFontName, changedText);
                                        replacer.Replace(currentTC);
                                    }
                                }
                                GUILayout.EndScrollView();
                                GUILayout.Label("Warning: These Settings RESET the scene!");
                                if (GUILayout.Button("Reset Custom Text"))
                                {
                                    changedText = "";
                                    var replacer = Config.CurrentProfile.UpdateProfile(objectName, currentType.ToString(), SceneManager.GetActiveScene().name, changedFontName, changedText);
                                    replacer.Replace(currentTC);
                                }
                                if (GUILayout.Button("Reset Custom Font"))
                                {
                                    changedFontName = "";
                                    var replacer = Config.CurrentProfile.UpdateProfile(objectName, currentType.ToString(), SceneManager.GetActiveScene().name, changedFontName, changedText);
                                    replacer.Replace(currentTC);
                                }
                            }
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("Back to Profile Editor")) { mediaScroll = Vector2.zero; changedText = ""; changedFontName = ""; menuIndex = 2; }
                            break;
                        case 4:
                            subTitle = "Change Profile";
                            profileScroll = GUILayout.BeginScrollView(profileScroll);
                            foreach (string profile in Config.GetProfiles())
                            {
                                if (Config.CurrentProfile != null)
                                {
                                    if (Config.CurrentProfile.ProfileName != profile)
                                    {
                                        if (GUILayout.Button("Switch to " + profile))
                                        {
                                            Config.LoadProfile(profile);
                                            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                                            foreach (var replacer in Config.CurrentProfile.GetObjectsForScene(SceneManager.GetActiveScene().name))
                                            {
                                                replacer.Replace();
                                            }
                                            foreach (var replacer in Config.CurrentProfile.GetTextsForScene(SceneManager.GetActiveScene().name))
                                            {
                                                replacer.Replace();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (GUILayout.Button("Switch to " + profile))
                                    {
                                        Config.LoadProfile(profile);
                                        ReplaceAll(SceneManager.GetActiveScene().name);
                                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                                    }
                                }
                            }
                            GUILayout.EndScrollView();
                            if (GUILayout.Button("Back to Main Menu")) { profileScroll = Vector2.zero; menuIndex = 0; }
                            break;
                        case 99:
                            subTitle = "Settings Menu";
                            if (GUILayout.Button("Back to Main Menu")) { menuIndex = 0; }
                            break;
                    }
                    break;
            }
            GUI.DragWindow();
        }

        void DrawEditorHeader(string subName)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<")) { objectScroll = Vector2.zero; if (profileEditorIndex - 1 < 0) { profileEditorIndex = 2; return; } profileEditorIndex--; }
            GUILayout.Label(subName);
            if (GUILayout.Button(">")) { objectScroll = Vector2.zero; if (profileEditorIndex + 1 > 3) { profileEditorIndex = 0; return; } profileEditorIndex++; }
            GUILayout.EndHorizontal();
        }
    }
}
