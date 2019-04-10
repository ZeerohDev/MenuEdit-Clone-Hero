using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace MenuEdit
{
    public class Replacer
    {
        public string ObjectName { get; set; }
        public ComponentType ObjectType { get; set; }
        public string ObjectScene { get; set; }
        public string MediaLocation { get; set; }
        public string FontName { get; set; }
        public string TextContent { get; set; }

        public Replacer(string objectName, ComponentType objectType, string objectScene, string imageName)
        {
            ObjectName = objectName;
            ObjectType = objectType;
            ObjectScene = objectScene;
            MediaLocation = imageName;
        }

        public Replacer(string objectName, ComponentType objectType, string objectScene, string fontName, string textContent)
        {
            ObjectName = objectName;
            ObjectType = objectType;
            ObjectScene = objectScene;
            FontName = fontName;
            TextContent = textContent;
        }

        public void Replace(Text component) { ReplaceText(component); }
        public void Replace(Image component) { ReplaceImage(component); }
        public void Replace(VideoPlayer component) { ReplaceVideo(component); }
        public void Replace()
        {
            switch (ObjectType)
            {
                case ComponentType.Image:
                    foreach (var item in Utils.GetComponents<Image>())
                    {
                        if (item.name == ObjectName) { Replace(item); return; }
                    }
                    break;
                case ComponentType.Text:
                    foreach (var item in Utils.GetComponents<Text>())
                    {
                        if (item.name == ObjectName) { Replace(item); return; }
                    }
                    break;
                case ComponentType.VideoPlayer:
                    foreach (var item in Utils.GetComponents<VideoPlayer>())
                    {
                        if (item.name == ObjectName) { Replace(item); return; }
                    }
                    break;
            }
        }

        private void ReplaceText(Text component)
        {
            if (TextContent != String.Empty) { component.text = TextContent; }
            if (FontName != String.Empty)
            {
                foreach (string fontName in Font.GetOSInstalledFontNames())
                {
                    if (fontName == FontName)
                    {
                        if (!Dictionaries.FontExists(fontName, component.fontSize))
                        {
                            Dictionaries.AddFont(fontName, Font.CreateDynamicFontFromOSFont(fontName, component.fontSize));
                        }
                        component.font = Dictionaries.GetFont(fontName + component.fontSize);
                    }
                }
            }
        }

        private void ReplaceImage(Image component)
        {
            Sprite sprite;
            if (Dictionaries.SpriteExists(MediaLocation)) { sprite = Dictionaries.GetSprite(MediaLocation); }
            else { sprite = Utils.CreateSpriteFromTex(MediaLocation); }
            component.overrideSprite = sprite;
        }

        private void ReplaceVideo(VideoPlayer component)
        {
            component.prepareCompleted += new VideoPlayer.EventHandler(OnPrepareComplete);
            component.playOnAwake = false;
            component.url = MediaLocation;
            component.renderMode = 0;
            component.isLooping = true;
            component.skipOnDrop = true;
            component.Prepare();
        }

        private void OnPrepareComplete(VideoPlayer component)
        {
            component.Play();
        }

        public enum ComponentType
        {
            Image,
            Text,
            VideoPlayer,
        }
    }
}
