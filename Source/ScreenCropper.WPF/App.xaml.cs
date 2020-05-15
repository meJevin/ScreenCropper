using Hardcodet.Wpf.TaskbarNotification;
using NHotkey;
using NHotkey.Wpf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using ScreenCropper.WPF.Windows;

namespace ScreenCropper.WPF
{
    public partial class App : Application
    {
        ScreenshotOverlay Overlay;
        SCSettings Settings;

        TaskbarIcon TrayIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            Overlay = new ScreenshotOverlay();
            Settings = new SCSettings();

            HotkeyManager.Current.AddOrReplace("Test", Key.C, ModifierKeys.Control | ModifierKeys.Alt, ShortcutHandler);

            InitTrayIcon();
        }

        private void InitTrayIcon()
        {
            var settingsMI = new MenuItem() { Header = "Settings" };
            var restartMI = new MenuItem() { Header = "Restart" };
            var exitMI = new MenuItem() { Header = "Exit" };

            settingsMI.Click += SettingsMenuItem_Click;
            restartMI.Click += RestartMenuItem_Click;
            exitMI.Click += ExitMenuItem_Click;

            ContextMenu trayCM = new ContextMenu();
            trayCM.Items.Add(settingsMI);
            trayCM.Items.Add(restartMI);
            trayCM.Items.Add(exitMI);

            TrayIcon = new TaskbarIcon();
            TrayIcon.Icon = new System.Drawing.Icon(@"Images\ScreenCropper.ico");
            TrayIcon.ContextMenu = trayCM;
        }

        private void ShortcutHandler(object sender, HotkeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Hidden)
            {
                Overlay.Show();
            }
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Settings.Show();
        }

        private void RestartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Application.Restart();

            Current.Shutdown();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();
        }
    }
}
