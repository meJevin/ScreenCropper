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

            MouseHookProcedure = MouseHookCallback;
            MouseHookID = WinAPIHelper.SetGlobalMouseHook(MouseHookProcedure);

            KeyboardHookProcedure = KeyboardHookCallback;
            KeyboardHookID = WinAPIHelper.SetGlobalKeyboardHook(KeyboardHookProcedure);

            // Get device contexts for out screen
            screenDC = GetDC(IntPtr.Zero);
            screenCompatibleDC = CreateCompatibleDC(screenDC);

            // This clears the BG initially
            DrawWindowRectangle(SystemInformation.VirtualScreen);
        }

        ~frmMain()
        {
            // Release the hooks
            WinAPIHelper.UnhookWindowsHookEx(MouseHookID);
            WinAPIHelper.UnhookWindowsHookEx(KeyboardHookID);

            // And the screen device context
            ReleaseDC(IntPtr.Zero, screenDC);
        }

        #region Private Variables

        // Current key combination that activated Screen Cropper
        private List<Key> CurrentCombination = new List<Key>() { Key.LeftCtrl, Key.LeftAlt, Key.V };

        // This pointer is required to perform unhooking cleanup
        private IntPtr KeyboardHookID = IntPtr.Zero;
        private static LowLevelHookProcedure KeyboardHookProcedure;

        private IntPtr MouseHookID = IntPtr.Zero;
        private static LowLevelHookProcedure MouseHookProcedure;

        // Used for selection drawing
        private Point selectionStartPoint = new Point();
        private Point lastCursorPosition = new Point();

        private Pen selectionRectangleBorderPen = new Pen(new SolidBrush(Color.Black), 1.5f);

        private bool isTakingScreenshot = false;
        private bool overlayVisible = false;

        private IntPtr screenDC;
        private IntPtr screenCompatibleDC;

        #endregion

        #region Privates Methods

        #region Hook callbacks
        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return WinAPIHelper.CallNextHookEx(KeyboardHookID, nCode, wParam, lParam);
            }

            MouseMessages mouseMessage = (MouseMessages)wParam;

            LowLevelMouseHookStructure mouseInfo = (LowLevelMouseHookStructure)Marshal.PtrToStructure(lParam, typeof(LowLevelMouseHookStructure));

            if (mouseMessage == MouseMessages.WM_LBUTTONDOWN)
            {
                if (!isTakingScreenshot && overlayVisible)
                {
                    StartTakingScreenshot(new Point(mouseInfo.point.x, mouseInfo.point.y));
                }
            }
            else if (mouseMessage == MouseMessages.WM_LBUTTONUP)
            {
                if (isTakingScreenshot)
                {
                    StopTakingScreenshot();

                    CopySelectedAreaToClipBoard();
                }
            }
            else if (mouseMessage == MouseMessages.WM_MOUSEMOVE)
            {
                if (isTakingScreenshot)
                {
                    HandleScreenshotSelectionChange(new Point(mouseInfo.point.x, mouseInfo.point.y));
                    Invalidate();
                }
            }

            return WinAPIHelper.CallNextHookEx(KeyboardHookID, nCode, wParam, lParam);
        }

        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return WinAPIHelper.CallNextHookEx(KeyboardHookID, nCode, wParam, lParam);
            }

            if (Keyboard.IsKeyDown(Key.Escape))
            {
                StopTakingScreenshot();
            }
            else if (IsCombinationPressed())
            {
                ShowScreenshotOverlay();
            }

            return WinAPIHelper.CallNextHookEx(KeyboardHookID, nCode, wParam, lParam);
        }
        #endregion

        private void ShowScreenshotOverlay()
        {
            if (this.Size != SystemInformation.VirtualScreen.Size)
            {
                this.Location = new System.Drawing.Point(0, 0);
                this.Size = SystemInformation.VirtualScreen.Size;
                this.Bounds = new Rectangle(0, 0, this.Size.Width, this.Size.Height);
            }
            this.Opacity = 0.5;
            Visible = true;
            overlayVisible = true;

            DrawWindowRectangle(SystemInformation.VirtualScreen);
        }

        private void StartTakingScreenshot(Point currentMousePosition)
        {
            selectionStartPoint = currentMousePosition;
            isTakingScreenshot = true;
        }

        private void StopTakingScreenshot()
        {
            this.Opacity = 0.25;
            isTakingScreenshot = false;
            Visible = false;
            overlayVisible = false;
            BackColor = Color.Black;
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
        
        private void CopySelectedAreaToClipBoard()
        {
            Rectangle selectionRect = Utils.RectangleFromTwoPoints(selectionStartPoint, lastCursorPosition);

            IntPtr screenBitmap = CreateCompatibleBitmap(screenDC, selectionRect.Width, selectionRect.Height);
            IntPtr tempScreenBitmap = SelectObject(screenCompatibleDC, screenBitmap);

            BitBlt(screenCompatibleDC, 0, 0, selectionRect.Width, selectionRect.Height, screenDC, selectionRect.X, selectionRect.Y, TernaryRasterOperations.SRCCOPY);

            screenBitmap = SelectObject(screenCompatibleDC, tempScreenBitmap);

            OpenClipboard(IntPtr.Zero);
            EmptyClipboard();
            SetClipboardData(ClipFormat.CF_BITMAP, screenBitmap);
            CloseClipboard();
        }
        
        private void HandleScreenshotSelectionChange(Point currentMousePosition)
        {
            overlayVisible = false;

            if (BackColor != Color.White)
            {
                BackColor = Color.White;
            }

            Rectangle selectionRect = Utils.RectangleFromTwoPoints(selectionStartPoint, currentMousePosition);

            DrawWindowRectangle(selectionRect);

            lastCursorPosition = currentMousePosition;
        }

        #region Form events
        private void frmMain_Paint(object sender, PaintEventArgs e)
        {
            // For some reason the clip rectangle is not the same as the one that I use to draw a specified region of this form
            // it's one pixel wider and higher, so we have to create a new one
            e.Graphics.DrawRectangle(selectionRectangleBorderPen, new Rectangle(e.ClipRectangle.Location, new Size(e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1)));
        }
        #endregion

        #endregion

        #region DLL Imports
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        static extern bool EmptyClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool CloseClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetClipboardData(ClipFormat uFormat, IntPtr hMem);

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
