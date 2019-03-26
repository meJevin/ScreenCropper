using System;
using System.Drawing;
using System.Windows.Input;
using System.Collections.Generic;

namespace ScreenCropper
{
    public static class Utils
    {
        public static string GetCombinationString(List<Key> combination)
        {
            string result = "";

            KeyConverter converter = new KeyConverter();

            foreach (Key key in combination)
            {
                result += converter.ConvertToString(key) + " + ";
            }

            result = result.Remove(result.Length - 3, 3);

            result = result.Replace("Left", "L").Replace("Right", "R");

            return result;
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
