/*
Screen Cropper Project - A lightweight screenshot taking program
Copyright (C) 2019 MICHAEL NAIFIELD

*/

using System;
using System.Windows.Forms;

namespace ScreenCropper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
