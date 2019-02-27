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
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            HookProc = HookCallback;
            HookID = WinAPIHelper.SetHook(HookProc);
        }

        ~frmMain()
        {
            WinAPIHelper.UnhookWindowsHookEx(HookID);
        }

        private void ShowFullscreen()
        {
            if (this.Size != SystemInformation.VirtualScreen.Size)
            {
                this.Location = new Point(0, 0);
                this.Size = SystemInformation.VirtualScreen.Size;
                this.Bounds = new Rectangle(0, 0, this.Size.Width, this.Size.Height);
            }
            this.Show();
        }


        private List<Key> VirtualKeyCodesForCombinations = new List<Key>() { Key.LeftCtrl, Key.LeftAlt, Key.V };

        private IntPtr HookID = IntPtr.Zero;
        private static LowLevelKeyboardProc HookProc;

        private Point startPointScreenshotRect;

        private bool isTakingScreenshot = false;

        private System.Windows.Shapes.Rectangle selectionRectangle;

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return WinAPIHelper.CallNextHookEx(HookID, nCode, wParam, lParam);
            }

            if (Keyboard.IsKeyDown(Key.Escape))
            {
                Hide();
            }

            bool combinationPressed = true;
            foreach (var key in VirtualKeyCodesForCombinations)
            {
                if (!Keyboard.IsKeyDown(key))
                {
                    combinationPressed = false;
                    break;
                }
            }

            if (combinationPressed)
            {
                ShowFullscreen();
            }

            return WinAPIHelper.CallNextHookEx(HookID, nCode, wParam, lParam);
        }


        private void frmMain_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                startPointScreenshotRect = MousePosition;
            }
        }

        private void frmMain_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (this.Opacity > 0)
            {
                this.Opacity = 0;
            }

            isTakingScreenshot = true;

            Text = MousePosition.ToString();

            ScreenCropperDrawer.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0, 0)), ScreenCropperExtensions.RectangleFromTwoPoints(startPointScreenshotRect, MousePosition));
        }

        private void frmMain_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (isTakingScreenshot)
            {
                Hide();
                this.Opacity = 0.25;
                isTakingScreenshot = false;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {

        }
    }
}
