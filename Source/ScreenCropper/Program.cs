/*

Screen Cropper Project - A lightweight screenshot taking program
Copyright (C) 2019 MICHAEL NAIFIELD

*/

using Squirrel;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenCropper
{
    static class Program
    {
        private static MainForm MainForm = new MainForm();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            CheckForUpdates();

            if (Utils.IsAlreadyRunning())
            {
                MessageBox.Show("Screen Cropper is already running!");
                return;
            }

            Application.EnableVisualStyles();

            Application.Run();

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledExpection);
        }

        static async Task CheckForUpdates()
        {
            using (var manager = new UpdateManager(@"C:\Users\Michael\Desktop\Dev\Personal\ScreenCropperCSharp\Source"))
            {
                await manager.UpdateApp();
            }
        }
        
        static void OnUnhandledExpection(object sender, UnhandledExceptionEventArgs args)
        {
            MessageBox.Show(args.ToString(), "Screen Cropper error!");
        }
    }
}
