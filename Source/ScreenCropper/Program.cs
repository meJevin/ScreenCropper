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
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledExpection);

            Task.Run(() => CheckForUpdates());

            if (Utils.IsAlreadyRunning())
            {
                MessageBox.Show("Screen Cropper is already running!");
                return;
            }

            Application.EnableVisualStyles();

            Application.Run();
        }

        static async Task CheckForUpdates()
        {
            using var manager = UpdateManager.GitHubUpdateManager(@"https://github.com/meJevin/ScreenCropperCSharp");

            var releaseEntry = await manager.Result.UpdateApp();

            if (releaseEntry != null)
            {
                MessageBox.Show($"Screen Cropper has been updated to {releaseEntry.Version.ToString()}!" +
                    $"\nRestart the application in order for changes to take place!", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        static void OnUnhandledExpection(object sender, UnhandledExceptionEventArgs args)
        {
            MessageBox.Show(args.ToString(), "Screen Cropper error!");
        }
    }
}
