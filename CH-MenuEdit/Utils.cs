using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MenuEdit
{
    public class Utils
    {
        public static T GetComponent<T>()
        {
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
            {
                if (go.GetComponent<T>() != null)
                {
                    return go.GetComponent<T>();
                }
            }
            return default(T);
        }

        public static List<T> GetComponents<T>()
        {
            List<T> ret = new List<T>();
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
            {
                if (go.GetComponent<T>() != null)
                {
                    ret.Add(go.GetComponent<T>());
                }
            }
            return ret;
        }

        public static List<GameObject> GetAllGameObjects()
        {
            List<GameObject> temp = new List<GameObject>();
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
            {
                temp.Add(go);
            }
            return temp;
        }

        public static void LogGameObjects()
        {
            using (StreamWriter writer = new StreamWriter("GameObjects.txt", true))
            {
                foreach (GameObject g in GameObject.FindObjectsOfType<GameObject>())
                {
                    writer.WriteLine(g.name);
                }
            }
        }

        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        public static Sprite CreateSpriteFromTex(string filePath)
        {
            Sprite ret;
            Texture2D tex = new Texture2D(2, 2);
            byte[] buffer = File.ReadAllBytes(filePath);
            if (ImageConversion.LoadImage(tex, buffer))
            {
                ret = Sprite.Create(tex, new Rect(0f, 0f, (float)tex.width, (float)tex.height), new Vector2(0f, 0f), 100f);
                return ret;
            }
            return null;
        }

        public static Color RGBtoUnity(float r, float g, float b, float alpha)
        {
            return new Color((float)r / 255, (float)g / 255, (float)b / 255, (float)alpha / 255);
        }

        public static float UnityToRGB(float val)
        {
            return (float)val * 255;
        }

        public static string GetCurrentScene()
        {
            return SceneManager.GetActiveScene().name;
        }
    }
}
