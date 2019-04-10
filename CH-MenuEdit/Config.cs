using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace MenuEdit
{
    public class Config
    {
        private static Settings CurrentSettings;
        public static Profile CurrentProfile;
        public static KeyCode OpenKey = KeyCode.F3;

        private static readonly string dataLocation = Environment.CurrentDirectory.Replace('\\', '/') + "/Tweaks/Config/MenuEdit/",
                                       mediaLocation = dataLocation + "Media/",
                                       profileLocation = dataLocation + "Profiles/",
                                       settingsFile = dataLocation + "settings.cfg";

        public static void Initialize()
        {
            Directory.CreateDirectory(dataLocation);
            Directory.CreateDirectory(mediaLocation);
            Directory.CreateDirectory(profileLocation);

            if (File.Exists(settingsFile))
            {
                LoadSettings();
            }
            else
            {
                CurrentSettings = new Settings();
            }
        }

        public static void CreateProfile(string profileName, bool setAsDefault, bool switchToProfile)
        {
            if (switchToProfile) { CurrentProfile = new Profile(profileName, true); }
            else { new Profile(profileName, true); }
            if (setAsDefault) CurrentSettings.UpdateSettings("default_profile", profileName);
            Debug.WriteLine("Profile created: " + profileName);
        }

        public static void LoadProfile(string profileName, bool loadingDefault = false)
        {
            var profilePath = profileLocation + profileName + ".cfg";
            if (!File.Exists(profilePath))
            {
                Debug.WriteLine("Cannot find profile " + profileName + ".");
                if (loadingDefault) { CurrentSettings.UpdateSettings("default_profile", ""); }
                return;
            }
            CurrentProfile = new Profile(profileName, false);
            var profileData = File.ReadAllLines(profilePath);
            for (int i = 0; i < profileData.Length;)
            {
                if (i >= profileData.Length) return;
                if (profileData[i].StartsWith("#")) return;
                if (profileData[i].StartsWith("object_name"))
                {
                    Debug.WriteLine("Parsing object data...");
                    var objectName = profileData[i].Substring(profileData[i].Split('=')[0].Length + 1);
                    var objectType = profileData[i + 1].Substring(profileData[i + 1].Split('=')[0].Length + 1);
                    var objectScene = profileData[i + 2].Substring(profileData[i + 2].Split('=')[0].Length + 1);
                    var mediaPath = profileData[i + 3].Substring(profileData[i + 3].Split('=')[0].Length + 1);
                    CurrentProfile.UpdateProfile(objectName, objectType, objectScene, mediaPath, false);
                    Debug.WriteLine("Object parsed: {0}|{1}|{2}|{3}", true, objectName, objectType, objectScene, mediaPath);
                    i = i + 4;
                }
                if (profileData[i].StartsWith("text_name"))
                {
                    Debug.WriteLine("Parsing text data...");
                    var objectName = profileData[i].Substring(profileData[i].Split('=')[0].Length + 1);
                    var objectType = profileData[i + 1].Substring(profileData[i + 1].Split('=')[0].Length + 1);
                    var objectScene = profileData[i + 2].Substring(profileData[i + 2].Split('=')[0].Length + 1);
                    var fontName = profileData[i + 3].Substring(profileData[i + 3].Split('=')[0].Length + 1);
                    var textContent = profileData[i + 4].Substring(profileData[i + 4].Split('=')[0].Length + 1);
                    CurrentProfile.UpdateProfile(objectName, objectType, objectScene, fontName, textContent, false);
                    Debug.WriteLine("Text parsed: {0}|{1}|{2}|{3}|{4}", true, objectName, objectType, objectScene, fontName, textContent);
                    i = i + 6;
                }
            }
        }

        private static void LoadSettings()
        {
            CurrentSettings = new Settings();
            foreach (string line in File.ReadAllLines(settingsFile))
            {
                if (line.StartsWith("#")) continue;
                if (line.StartsWith("default_profile"))
                {
                    var profileName = line.Substring(line.Split('=')[0].Length + 1);
                    if (profileName != "")
                    {
                        Debug.WriteLine("Default profile found: " + profileName);
                        CurrentSettings.UpdateSettings("default_profile", profileName);
                        LoadProfile(profileName, true);
                    }
                }
            }
        }

        public static List<string> GetProfiles()
        {
            List<string> ret = new List<string>();
            foreach (string file in Directory.GetFiles(profileLocation))
            {
                ret.Add(Path.GetFileNameWithoutExtension(file));
            }
            return ret;
        }

        public static List<string> GetAllMediaFiles()
        {
            List<string> ret = new List<string>();
            foreach (string file in Directory.GetFiles(mediaLocation))
            {
                ret.Add(Path.GetFileName(file));
            }
            return ret;
        }

        public static string GetPathForMediaFile(string fileName)
        {
            foreach (string file in Directory.GetFiles(mediaLocation))
            {
                if (Path.GetFileName(file) == fileName)
                {
                    return file.Replace('\\', '/');
                }
            }
            return String.Empty;
        }

        public class Settings
        {
            private Dictionary<string, string> settingsDictionary;

            public Settings()
            {
                settingsDictionary = new Dictionary<string, string>();
            }

            public void UpdateSettings(string key, string value)
            {
                if (settingsDictionary.ContainsKey(key)) { settingsDictionary[key] = value; }
                else { settingsDictionary.Add(key, value); }
                SaveSettings();
            }

            private void SaveSettings()
            {
                using (StreamWriter writer = new StreamWriter(settingsFile))
                {
                    foreach (KeyValuePair<string, string> kvp in settingsDictionary)
                    {
                        writer.WriteLine(kvp.Key + "=" + kvp.Value);
                    }
                }
            }
        }

        public class Profile
        {
            public string ProfileName { get; set; }
            private List<Replacer> ObjectList = new List<Replacer>();
            private List<Replacer> TextList = new List<Replacer>();
            
            public Profile(string profileName, bool isNew)
            {
                ProfileName = profileName;
                if (isNew) { SaveProfile(); }
            }

            public Replacer UpdateProfile(string objectName, string componentType, string objectScene, string mediaPath, bool saveProfile = true)
            {
                foreach (var obj in ObjectList)
                {
                    if (obj.ObjectName == objectName)
                    {
                        obj.MediaLocation = mediaPath;
                        if (saveProfile) { SaveProfile(); }
                        return obj;
                    }
                }
                var newObj = new Replacer(objectName, (Replacer.ComponentType)Enum.Parse(typeof(Replacer.ComponentType), componentType), objectScene, mediaPath);
                ObjectList.Add(newObj);
                if (saveProfile) { SaveProfile(); }
                return newObj;
            }

            public Replacer UpdateProfile(string objectName, string componentType, string objectScene, string fontName, string textContent, bool saveProfile = true)
            {
                foreach (var obj in TextList)
                {
                    if (obj.ObjectName == objectName)
                    {
                        obj.FontName = fontName;
                        obj.TextContent = textContent;
                        if (saveProfile) { SaveProfile(); }
                        return obj;
                    }
                }
                var newObj = new Replacer(objectName, (Replacer.ComponentType)Enum.Parse(typeof(Replacer.ComponentType), componentType), objectScene, fontName, textContent);
                TextList.Add(newObj);
                if (saveProfile) { SaveProfile(); }
                return newObj;
            }

            public bool ObjectExists(string objectName)
            {
                foreach (var obj in ObjectList)
                {
                    if (obj.ObjectName == objectName)
                    {
                        return true;
                    }
                }
                return false;
            }

            public bool TextExists(string objectName)
            {
                foreach (var obj in TextList)
                {
                    if (obj.ObjectName == objectName)
                    {
                        return true;
                    }
                }
                return false;
            }

            public List<Replacer> GetObjectsForScene(string scene)
            {
                List<Replacer> ret = new List<Replacer>();
                foreach (var item in ObjectList)
                {
                    if (item.ObjectScene == scene)
                    {
                        ret.Add(item);
                    }
                }
                return ret;
            }

            public List<Replacer> GetTextsForScene(string scene)
            {
                List<Replacer> ret = new List<Replacer>();
                foreach (var item in TextList)
                {
                    if (item.ObjectScene == scene)
                    {
                        ret.Add(item);
                    }
                }
                return ret;
            }

            private void SaveProfile()
            {
                using (StreamWriter writer = new StreamWriter(profileLocation + ProfileName + ".cfg"))
                {
                    foreach (var obj in ObjectList)
                    {
                        writer.WriteLine("object_name=" + obj.ObjectName);
                        writer.WriteLine("object_type=" + obj.ObjectType.ToString());
                        writer.WriteLine("object_scene=" + obj.ObjectScene);
                        writer.WriteLine("media_location=" + obj.MediaLocation);
                    }
                    foreach (var text in TextList)
                    {
                        writer.WriteLine("text_name=" + text.ObjectName);
                        writer.WriteLine("object_type=" + text.ObjectType.ToString());
                        writer.WriteLine("object_scene=" + text.ObjectScene);
                        writer.WriteLine("font_name=" + text.FontName);
                        writer.WriteLine("text_content=" + text.TextContent);
                    }
                }
            }
        }
    }
}
