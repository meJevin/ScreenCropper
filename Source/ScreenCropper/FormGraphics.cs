using System;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ScreenCropper
{
    public class ScreenCropperDrawer
    {
        private static Graphics screenGraphics;

        public static void FillRectangle(Brush brush, Rectangle rect)
        {
            if (screenGraphics == null)
            {
                screenGraphics = Graphics.FromHwnd(IntPtr.Zero);
            }

            screenGraphics.FillRectangle(brush, rect);
        }
    }
    public static class ScreenCropperExtensions
    {
        public static Rectangle RectangleFromTwoPoints(Point p1, Point p2)
        {
            return new Rectangle(Math.Min(p1.X, p2.X),
               Math.Min(p1.Y, p2.Y),
               Math.Abs(p1.X - p2.X),
               Math.Abs(p1.Y - p2.Y));
        }
    }
}
