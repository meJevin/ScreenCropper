using System;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

            // Get device contexts for out screen
            screenDC = GetDC(IntPtr.Zero);
            screenCompatibleDC = CreateCompatibleDC(screenDC);

            // This clears the BG initially
            DrawWindowRectangle(SystemInformation.VirtualScreen);
        }

        ~frmMain()
        {
            WinAPIHelper.UnhookWindowsHookEx(HookID);

            ReleaseDC(IntPtr.Zero, screenDC);
        }


        #region Private Variables

        // Current key combination that activated Screen Cropper
        private List<Key> CurrentCombination = new List<Key>() { Key.LeftCtrl, Key.LeftAlt, Key.V };

        // This pointer is required to perform unhooking cleanup
        private IntPtr HookID = IntPtr.Zero;
        private static LowLevelKeyboardProc HookProc;

        private Point selectionStartPoint = new Point();

        // Used for the Mouse Move event, becaused apparentely it false fires a lot :)
        private Point lastCursorPosition = new Point();

        private bool isTakingScreenshot = false;

        private IntPtr screenDC;
        private IntPtr screenCompatibleDC;

        #endregion

        #region Privates Methods

        private void StopTakingScreenshot()
        {
            if (!isTakingScreenshot)
            {
                return;
            }

            this.Opacity = 0.25;
            isTakingScreenshot = false;
            Visible = false;
        }

        private void ShowScreenshotOverlay()
        {
            if (this.Size != SystemInformation.VirtualScreen.Size)
            {
                this.Location = new Point(0, 0);
                this.Size = SystemInformation.VirtualScreen.Size;
                this.Bounds = new Rectangle(0, 0, this.Size.Width, this.Size.Height);
            }
            this.Opacity = 0.5;
            Visible = true;

            DrawWindowRectangle(SystemInformation.VirtualScreen);
        }

        private void DrawWindowRectangle(Rectangle rect)
        {
            WinAPIHelper.DrawWindowRectangle(rect, this.Handle);
        }

        private bool IsCombinationPressed()
        {
            bool combinationPressed = true;
            foreach (var key in CurrentCombination)
            {
                if (!Keyboard.IsKeyDown(key))
                {
                    combinationPressed = false;
                    break;
                }
            }

            return combinationPressed;
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return WinAPIHelper.CallNextHookEx(HookID, nCode, wParam, lParam);
            }

            if (Keyboard.IsKeyDown(Key.Escape))
            {
                StopTakingScreenshot();
            }
            else if (IsCombinationPressed())
            {
                ShowScreenshotOverlay();
            }

            return WinAPIHelper.CallNextHookEx(HookID, nCode, wParam, lParam);
        }

        private void CopySelectedAreaToClipBoard()
        {
            Rectangle selectionRect = Utils.RectangleFromTwoPoints(selectionStartPoint, MousePosition);

            IntPtr screenBitmap = CreateCompatibleBitmap(screenDC, selectionRect.Width, selectionRect.Height);
            IntPtr tempScreenBitmap = SelectObject(screenCompatibleDC, screenBitmap);

            BitBlt(screenCompatibleDC, 0, 0, selectionRect.Width, selectionRect.Height, screenDC, selectionRect.X, selectionRect.Y, TernaryRasterOperations.SRCCOPY);

            screenBitmap = SelectObject(screenCompatibleDC, tempScreenBitmap);

            OpenClipboard(IntPtr.Zero);
            EmptyClipboard();
            SetClipboardData(CLIPFORMAT.CF_BITMAP, screenBitmap);
            CloseClipboard();
        }

        #endregion

        #region Mouse Events

        private void frmMain_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectionStartPoint = MousePosition;
                isTakingScreenshot = true;
            }
        }

        private void frmMain_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || lastCursorPosition == MousePosition)
            {
                return;
            }

            Rectangle selectionRect = Utils.RectangleFromTwoPoints(selectionStartPoint, MousePosition);

            DrawWindowRectangle(selectionRect);

            lastCursorPosition = MousePosition;
        }

        private void frmMain_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (isTakingScreenshot)
            {
                StopTakingScreenshot();

                CopySelectedAreaToClipBoard();
            }
        }

        #endregion

        #region DLL Imports
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

        [DllImport("user32.dll")]
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
        #endregion

        #region Tray Icon Context Menu Item Click Events

        private void quitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion
    }
}
