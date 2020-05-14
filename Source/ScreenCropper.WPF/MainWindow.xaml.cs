using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using NHotkey;
using NHotkey.Wpf;

namespace ScreenCropper.WPF
{ 
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            HotkeyManager.Current.AddOrReplace("Test", Key.C, ModifierKeys.Control | ModifierKeys.Alt, ShortcutHandler);

            Width = SystemInformation.VirtualScreen.Width;
            Height = SystemInformation.VirtualScreen.Height;

            Left = 0;
            Top = 0;
        }

        Point StartPoint = new Point(-1, -1);
        Point CurrentPoint = new Point(-1, -1);

        bool IsTakingScreenshot = false;

        private void ShortcutHandler(object sender, HotkeyEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Hide();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsTakingScreenshot = true;

            StartPoint = Mouse.GetPosition(this);
            CurrentPoint = StartPoint;
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (IsTakingScreenshot)
            {
                CurrentPoint = Mouse.GetPosition(this);

                Rect selectionRect = new Rect(StartPoint, CurrentPoint);

                SelectionRectange.Margin = new Thickness(selectionRect.X, selectionRect.Y, 0, 0);
                SelectionRectange.Width = selectionRect.Width;
                SelectionRectange.Height = selectionRect.Height;
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsTakingScreenshot = false;
        }
    }
}
