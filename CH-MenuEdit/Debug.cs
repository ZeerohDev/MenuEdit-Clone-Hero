using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MenuEdit
{
    public class Debug
    {
        public static bool DebugEnabled = true;

        public static void WriteLine(string msg, bool timeStamp = true)
        {
            using (StreamWriter writer = new StreamWriter("Debug.txt", true))
            {
                var tag = timeStamp ? "[MenuEdit - " + DateTime.Now.ToString("hh:mm:ss") + "] " : "[MenuEdit] ";
                writer.WriteLine(tag + msg);
            }
        }

        public static void WriteLine(string msg, bool timeStamp = true, params object[] args)
        {
            using (StreamWriter writer = new StreamWriter("Debug.txt", true))
            {
                var tag = timeStamp ? "[MenuEdit - " + DateTime.Now.ToString("hh:mm:ss") + "] " : "[MenuEdit] ";
                writer.WriteLine(tag + string.Format(msg, args));
            }
        }
    }
}
