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

            this.TransparencyKey = Color.LimeGreen;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            screenDC = GetDC(IntPtr.Zero);
            screenCompatibleDC = CreateCompatibleDC(screenDC);
        }

        ~frmMain()
        {
            WinAPIHelper.UnhookWindowsHookEx(HookID);
            ReleaseDC(IntPtr.Zero, screenDC);
        }

        private void ShowFullscreen()
        {
            if (this.Size != SystemInformation.VirtualScreen.Size)
            {
                this.Location = new Point(0, 0);
                this.Size = SystemInformation.VirtualScreen.Size;
                this.Bounds = new Rectangle(0, 0, this.Size.Width, this.Size.Height);
            }
            Visible = true;
        }


        private List<Key> VirtualKeyCodesForCombinations = new List<Key>() { Key.LeftCtrl, Key.LeftAlt, Key.V };

        private IntPtr HookID = IntPtr.Zero;
        private static LowLevelKeyboardProc HookProc;

        private Point startPointScreenshotRect;

        private bool isTakingScreenshot = false;

        private IntPtr screenDC;
        private IntPtr screenCompatibleDC;

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return WinAPIHelper.CallNextHookEx(HookID, nCode, wParam, lParam);
            }

            if (Keyboard.IsKeyDown(Key.Escape))
            {
                isTakingScreenshot = false;
                Hide();
            }

            bool combinationPressed = true;
            foreach (var key in VirtualKeyCodesForCombinations)
            {
                if (!Keyboard.IsKeyDown(key))
                {
                    combinationPressed = false;
                    ScreenCropperDrawer.FillRectangle(SystemInformation.VirtualScreen, this.Handle);
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

            isTakingScreenshot = true;
            
            if (Visible)
            {
                //Visible = false;
            }

            Rectangle selectionRect = ScreenCropperExtensions.RectangleFromTwoPoints(startPointScreenshotRect, MousePosition);

            ScreenCropperDrawer.FillRectangle(selectionRect, this.Handle);
        }

        private void frmMain_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (isTakingScreenshot)
            {
                Visible = false;

                Rectangle selectionRect = ScreenCropperExtensions.RectangleFromTwoPoints(startPointScreenshotRect, MousePosition);

                IntPtr screenBitmap = CreateCompatibleBitmap(screenDC, selectionRect.Width, selectionRect.Height);
                IntPtr tempScreenBitmap = SelectObject(screenCompatibleDC, screenBitmap);

                BitBlt(screenCompatibleDC, 0, 0, selectionRect.Width, selectionRect.Height, screenDC, selectionRect.X, selectionRect.Y, TernaryRasterOperations.SRCCOPY);

                screenBitmap = SelectObject(screenCompatibleDC, tempScreenBitmap);

                OpenClipboard(IntPtr.Zero);
                EmptyClipboard();
                SetClipboardData(CLIPFORMAT.CF_BITMAP, screenBitmap);
                CloseClipboard();

                isTakingScreenshot = false;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        static extern bool EmptyClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool CloseClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetClipboardData(CLIPFORMAT uFormat, IntPtr hMem);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);

        [DllImport("gdi32.dll")]
        static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
        static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject", SetLastError = true)]
        static public extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

    }
}
