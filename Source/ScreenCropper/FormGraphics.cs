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
        public static void FillRectangle(Rectangle rect, IntPtr windowHandle)
        {
            IntPtr drawReg = CreateRectRgn(rect.Left,
                        rect.Top,
                        rect.Left + rect.Width,
                        rect.Top + rect.Height
                        );

            SetWindowRgn(windowHandle, drawReg, true);
        }

        [DllImport("gdi32.dll")]
        static public extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);

        [DllImport("user32.dll")]
        static public extern IntPtr SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool redraw);
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
