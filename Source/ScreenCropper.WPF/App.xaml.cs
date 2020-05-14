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

namespace ScreenCropper.WPF
{
    public partial class App : Application
    {
        ScreenshotOverlay Overlay = new ScreenshotOverlay();

        protected override void OnStartup(StartupEventArgs e)
        {
            HotkeyManager.Current.AddOrReplace("Test", Key.C, ModifierKeys.Control | ModifierKeys.Alt, ShortcutHandler);
        }

        private void ShortcutHandler(object sender, HotkeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Hidden)
            {
                Overlay.Show();
            }
        }
    }
}
