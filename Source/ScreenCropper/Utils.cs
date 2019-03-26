using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ScreenCropper
{
    public static class Utils
    {
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

        public static string NullTerminate(string str)
        {
            if (!str.Contains("\0"))
            {
                return str;
            }

            return str.Substring(0, str.IndexOf('\0') - 1);
        }

        public static Rectangle RectangleFromTwoPoints(System.Drawing.Point p1, System.Drawing.Point p2)
        {
            return new Rectangle(Math.Min(p1.X, p2.X),
               Math.Min(p1.Y, p2.Y),
               Math.Abs(p1.X - p2.X),
               Math.Abs(p1.Y - p2.Y));
        }
    }
}
