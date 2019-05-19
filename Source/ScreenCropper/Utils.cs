using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;

namespace ScreenCropper
{
    public static class Utils
    {
        /// <summary>
        /// Creates a combination string in a human-readable format.
        /// In Screen Cropper this is used for the notification bubble, that shows the current combination
        /// </summary>
        /// <param name="combination">List of keys for the combination</param>
        /// <returns>String in a human-readable format</returns>
        public static string GetCombinationString(List<Keys> combination)
        {
            string result = "";

            KeysConverter converter = new KeysConverter();

            foreach (Keys key in combination)
            {
                result += converter.ConvertToString(key) + " + ";
            }

            result = result.Remove(result.Length - 3, 3);

            result = result.Replace("ControlKey", "CTRL").Replace("Menu", "ALT").ToUpper();

            return result;
        }

        /// <summary>
        /// Creates a serialized string from a combination.
        /// In Screen Cropper this is used to store in the regestry and later parse
        /// </summary>
        /// <param name="combination"></param>
        /// <returns></returns>
        public static string SerializeCombination(List<Keys> combination)
        {
            string result = "";

            foreach (Keys key in combination)
            {
                result += (int)key + ", ";
            }

            result = result.Remove(result.Length - 2, 2);

            return result;
        }

        /// <summary>
        /// This function parses a combination string from the regestry and returns a List of keys
        /// </summary>
        /// <param name="combinationString">Combination string</param>
        /// <returns>List of keys from the curent combination</returns>
        public static List<Keys> ParseCombination(string combinationString)
        {
            List<Keys> result = new List<Keys>();

            string[] tokens = combinationString.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            {
                result.Add((Keys)int.Parse(token));
            }

            return result;
        }

        /// <summary>
        /// Somethimes registry strings can have garbage in them and are not properly null-terminated for C#, this function is required for all strings retrieved from the registry
        /// </summary>
        /// <param name="str">String fresh out of the registry</param>
        /// <returns>Properly null-terminated string suitable for future use with C#</returns>
        public static string NullTerminate(string str)
        {
            if (!str.Contains("\0"))
            {
                return str;
            }

            return str.Substring(0, str.IndexOf('\0'));
        }

        public static Rectangle RectangleFromTwoPoints(Point p1, Point p2)
        {
            return new Rectangle(Math.Min(p1.X, p2.X),
               Math.Min(p1.Y, p2.Y),
               Math.Abs(p1.X - p2.X),
               Math.Abs(p1.Y - p2.Y));
        }

        public static bool IsAlreadyRunning()
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public static class ScreenCropperExtensions
    {
        // Syntactic sugar :)
        public static bool Contains<T>(this List<T> list, T val)
        {
            return (list.IndexOf(val) != -1);
        }
    }

}
