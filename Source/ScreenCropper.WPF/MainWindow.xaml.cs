using System;
using System.Collections.Generic;
using System.Linq;
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

        Point startPoint = new Point(-1, -1);
        Point currPoint = new Point(-1, -1);

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

        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
