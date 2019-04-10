using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MenuEdit
{
    public class Dictionaries
    {
        private static Dictionary<string, Sprite> SpriteDictionary;
        private static Dictionary<string, Font> FontDictionary;

        public static void Initialize()
        {
            SpriteDictionary = new Dictionary<string, Sprite>();
            FontDictionary = new Dictionary<string, Font>();
        }

        public static Sprite GetSprite(string filePath)
        {
            return SpriteDictionary[filePath];
        }

        public static Font GetFont(string fontName)
        {
            return FontDictionary[fontName];
        }

        public static void AddSprite(string filePath, Sprite sprite)
        {
            SpriteDictionary.Add(filePath, sprite);
        }

        public static void AddFont(string fontName, Font font)
        {
            FontDictionary.Add(fontName + font.fontSize, font);
        }

        public static bool SpriteExists(string filePath)
        {
            var exists = SpriteDictionary.ContainsKey(filePath) ? " Exists" : " Does Not Exist";
            Debug.WriteLine("Sprite @ " + filePath + exists);
            return SpriteDictionary.ContainsKey(filePath);
        }

        public static bool FontExists(string fontName, int fontSize)
        {
            var fs = FontDictionary[fontName].fontSize;
            return FontDictionary.ContainsKey(fontName + fs);
        }
    }
}
