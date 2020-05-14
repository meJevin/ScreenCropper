using System;
using System.Collections.Generic;
using System.Drawing;
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
using Clipboard = System.Windows.Forms.Clipboard;
using Point = System.Windows.Point;
using Color = System.Windows.Media.Color;

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

            Activated += MainWindow_Activated;
            Deactivated += MainWindow_Deactivated;
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            Background = new SolidColorBrush(Color.FromArgb(35, 0, 0, 0));
            Console.WriteLine("Activated");
        }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            // Save selected area
            Rect temp = new Rect(StartPoint, CurrentPoint);
            Int32Rect selection = new Int32Rect((int)temp.X, (int)temp.Y, (int)temp.Width, (int)temp.Height);
            
            if (selection.Width == 0 || selection.Height == 0)
            {
                return;
            }

            Bitmap b = new Bitmap(selection.Width, selection.Height);
            Graphics g = Graphics.FromImage(b);
            g.CopyFromScreen(selection.X, selection.Y, 0, 0, new System.Drawing.Size(selection.Width, selection.Height));

            Clipboard.SetImage(b);

            b.Dispose();
            g.Dispose();
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

            Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (IsTakingScreenshot)
            {
                CurrentPoint = Mouse.GetPosition(this);

                Rect selection = new Rect(StartPoint, CurrentPoint);

                SelectionRectange.Margin = new Thickness(selection.X, selection.Y, 0, 0);
                SelectionRectange.Width = selection.Width;
                SelectionRectange.Height = selection.Height;
            }
        }

        private async void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsTakingScreenshot = false;

            SelectionRectange.Width = 0;
            SelectionRectange.Height = 0;

            await Task.Delay(50);

            Hide();
        }
    }
}
