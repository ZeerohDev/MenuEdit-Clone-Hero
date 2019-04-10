using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MenuEdit
{
    public class Loader
    {
        static GameObject go;

        public static void LoadTweak()
        {
            go = new GameObject();
            go.name = Globals.objectName;
            go.AddComponent<MainTweak>();
            GameObject.DontDestroyOnLoad(go);
            go.SetActive(true);
        }

        public static void UnloadTweak()
        {
            GameObject.DestroyImmediate(go);
        }
    }
}
