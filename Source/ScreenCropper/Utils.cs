﻿using System;
using System.Drawing;

namespace ScreenCropper
{
    public static class Utils
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
