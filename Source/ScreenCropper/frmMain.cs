using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ScreenCropper
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            InitializeScreenCropper();

            MouseHookProcedure = MouseHookCallback;
            MouseHookID = WinAPIHelper.SetGlobalMouseHook(MouseHookProcedure);

            KeyboardHookProcedure = KeyboardHookCallback;
            KeyboardHookID = WinAPIHelper.SetGlobalKeyboardHook(KeyboardHookProcedure);

            // Get device contexts for out screen
            screenDC = GetDC(IntPtr.Zero);
            screenCompatibleDC = CreateCompatibleDC(screenDC);

            // This clears the BG initially
            WinAPIHelper.DrawWindowRectangle(SystemInformation.VirtualScreen, this.Handle);
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
        private List<Keys> CurrentCombination = new List<Keys>() { Keys.LControlKey, Keys.LMenu, Keys.C };

        private bool isChangingCombination = false;

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

        private IntPtr screenDC = IntPtr.Zero;
        private IntPtr screenCompatibleDC = IntPtr.Zero;

        #endregion

        #region Privates Methods

        #region Hook callbacks

        /// <summary>
        /// Mouse hook is responsible for:
        /// 1. Deciding when to start taking a screenshot
        /// 2. Determening the area of the screenshot that the user is currently taking
        /// 3. Deciding when to stop taking a screenshot abd save it to clipboard
        /// </summary>
        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return WinAPIHelper.CallNextHookEx(MouseHookID, nCode, wParam, lParam);
            }

            MouseMessages mouseMessage = (MouseMessages)wParam;

            LowLevelMouseHookStructure mouseInfo = (LowLevelMouseHookStructure)Marshal.PtrToStructure(lParam, typeof(LowLevelMouseHookStructure));

            if (mouseMessage == MouseMessages.WM_LBUTTONDOWN)
            {
                // We can start taking a screenshot only if the overlay is visible and user pressed LMB
                if (!isTakingScreenshot && overlayVisible)
                {
                    StartTakingScreenshot(new Point(mouseInfo.point.x, mouseInfo.point.y));
                }
            }
            else if (mouseMessage == MouseMessages.WM_LBUTTONUP)
            {
                // Stop taking a screenshot and save it to clipboard, but only if we're actually taking it
                if (isTakingScreenshot)
                {
                    StopTakingScreenshot();

                    CopySelectedAreaToClipBoard();
                }
            }
            else if (mouseMessage == MouseMessages.WM_MOUSEMOVE)
            {
                // If we're taking a screenshot and moving a mouse, this means we're changing the desired screenshot area
                if (isTakingScreenshot)
                {
                    HandleScreenshotSelectionChange(new Point(mouseInfo.point.x, mouseInfo.point.y));
                    Invalidate();
                }
            }

            return WinAPIHelper.CallNextHookEx(MouseHookID, nCode, wParam, lParam);
        }

        /// <summary>
        /// Keyboard hook is responsible for:
        /// 1. Deciding when to show the screenshot overlay
        /// 2. Prematurely stop taking a screenshot if Escape key was pressed
        /// </summary>
        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return WinAPIHelper.CallNextHookEx(KeyboardHookID, nCode, wParam, lParam);
            }

            if (Keyboard.IsKeyDown(Key.Escape))
            {
                // If we're taking a screenshot or issued an overlay, let's cancel them
                if (isTakingScreenshot || overlayVisible)
                {
                    StopTakingScreenshot();
                }
            }
            else if (IsCombinationPressed())
            {
                // User has finally pressed his combination, let's show the overlay and wait for him to cancel or start taking a screenshot
                ShowScreenshotOverlay();
            }

            return WinAPIHelper.CallNextHookEx(KeyboardHookID, nCode, wParam, lParam);
        }

        #endregion

        #region Screenshot Related

        /// <summary>
        /// Black overlay which is a system-on-top fullscreen form
        /// </summary>
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

            WinAPIHelper.DrawWindowRectangle(SystemInformation.VirtualScreen, this.Handle);
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

        /// <summary>
        /// Occurs when user moves the mouse while selecting the desired area of screenshot
        /// </summary>
        /// <param name="currentMousePosition">New mouse position</param>
        private void HandleScreenshotSelectionChange(Point currentMousePosition)
        {
            overlayVisible = false;

            if (BackColor != Color.White)
            {
                BackColor = Color.White;
            }

            Rectangle selectionRect = Utils.RectangleFromTwoPoints(selectionStartPoint, currentMousePosition);

            // Look at the defenition for more details
            WinAPIHelper.DrawWindowRectangle(selectionRect, this.Handle);

            lastCursorPosition = currentMousePosition;
        }

        /// <summary>
        /// Copies currently selected are to clipboard
        /// </summary>
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

        #endregion

        #region Combination Related

        private void StartChangingCombination()
        {
            isChangingCombination = true;
        }
        
        private void StopChangingCombination()
        {
            isChangingCombination = false;
        }

        /// <summary>
        /// Iterates over all the keys in the current combination and checks if it's down on the keyboard
        /// </summary>
        /// <returns></returns>
        private bool IsCombinationPressed()
        {
            bool combinationPressed = true;
            foreach (var key in CurrentCombination)
            {
                if (!Keyboard.IsKeyDown(KeyInterop.KeyFromVirtualKey((int)key)))
                {
                    combinationPressed = false;
                    break;
                }
            }

            return combinationPressed;
        }

        private void ShowCurrentCombinationInTrayIcon()
        {
            if (trayIcon.Icon == null)
            {
                trayIcon.Icon = new Icon("ScreenCropper.ico");
            }

            trayIcon.BalloonTipTitle = "Your combination";
            trayIcon.BalloonTipText = Utils.GetCombinationString(CurrentCombination);
            trayIcon.Visible = true;
            trayIcon.ShowBalloonTip(3000);
        }

        #endregion

        #region Application Related

        /// <summary>
        /// The combination is stored in the registry. This function loads it in, or creates a default one [LCTRL + LALT + C] if there is no regestry entry to read.
        /// It's also backwards-compatible with the version written in Delphi
        /// </summary>
        private void LoadCombinationFromRegistry()
        {
            RegistryKey screenCropperKey = OpenScreenCropperRegKey();

            var loadedCombinationValue = screenCropperKey.GetValue("ActivationCombination");

            if (loadedCombinationValue != null)
            {
                // Succesful load of combination
                string loadedCombinationString = loadedCombinationValue as string;
                loadedCombinationString = Utils.NullTerminate(loadedCombinationString);

                CurrentCombination = Utils.ParseCombination(loadedCombinationString);
            }
            else
            {
                // It has disappeared or has never been there, let's create it again and default it out
                screenCropperKey.SetValue("ActivationCombination", Utils.SerializeCombination(CurrentCombination));
            }
        }

        /// <summary>
        /// Startup options of this application are stored in the registry. This functions reads them and sets the context menu checkbox accordingly.
        /// If the entry is not found, application will ask the user whether he wants it to load on startup or not
        /// </summary>
        private void CheckStartup()
        {
            var windowsStartupAppsKey = OpenWindowsStartupAppsKey();
            var screenCropperStartupValue = windowsStartupAppsKey.GetValue("ScreenCropper");

            if (screenCropperStartupValue == null)
            {
                // We're not registered in the windows startup, let's ask the user whether he wants the application to be launched at startup

                DialogResult result = MessageBox.Show("Do you want Screen Cropper to be launch at startup?", "Screen Cropper", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    windowsStartupAppsKey.SetValue("ScreenCropper", Application.ExecutablePath);
                    launchOnStartupMenuItem.Checked = true;
                }
                else
                {
                    windowsStartupAppsKey.SetValue("ScreenCropper", "None");
                    launchOnStartupMenuItem.Checked = false;
                }
            }
            else
            {
                // Initizize from current settings
                string loadedStartupPath = screenCropperStartupValue as string;
                loadedStartupPath = Utils.NullTerminate(loadedStartupPath);

                // Make sure the value in that Key is equal to the current .exe ("None means that the user has chosen not to launch the application at startup)
                if (loadedStartupPath == "None")
                {
                    launchOnStartupMenuItem.Checked = false;
                }
                else
                {
                    if (Application.ExecutablePath != loadedStartupPath)
                    {
                        windowsStartupAppsKey.SetValue("ScreenCropper", Application.ExecutablePath);
                    }

                    launchOnStartupMenuItem.Checked = true;
                }
            }
        }

        /// <summary>
        /// Checks regestry, initializes combination, startup etc.
        /// </summary>
        private void InitializeScreenCropper()
        {
            var screenCropperKey = OpenScreenCropperRegKey();

            if (screenCropperKey == null)
            {
                // First launch
                screenCropperKey = Registry.CurrentUser.CreateSubKey(@"Software\ScreenCropper", true);

                screenCropperKey.SetValue("ActivationCombination", Utils.SerializeCombination(CurrentCombination));

                CheckStartup();
            }
            else
            {
                // Initizize current settings

                LoadCombinationFromRegistry();

                CheckStartup();
            }
        }

        /// <summary>
        /// Opens and returns a key where Screen Cropper information is stored
        /// </summary>
        private RegistryKey OpenScreenCropperRegKey()
        {
            var currentUserRegKey = Registry.CurrentUser;

            return currentUserRegKey.OpenSubKey(@"Software\ScreenCropper", true);
        }

        /// <summary>
        /// Opens and returns a key where Windows stores paths to executables that load on startup
        /// </summary>
        private RegistryKey OpenWindowsStartupAppsKey()
        {
            var currentUserRegKey = Registry.CurrentUser;

            return currentUserRegKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        }

        #endregion

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
            DialogResult result = MessageBox.Show("Are you sure you want to exit Screen Cropper?", "Qutting Screen Cropper", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
            else
            {
                return;
            }
        }

        private void launchOnStartupMenuItem_Click(object sender, EventArgs e)
        {
            var windowsStartupAppsKey = OpenWindowsStartupAppsKey();

            if (launchOnStartupMenuItem.Checked)
            {
                windowsStartupAppsKey.SetValue("ScreenCropper", Application.ExecutablePath);
            }
            else
            {
                windowsStartupAppsKey.SetValue("ScreenCropper", "None");
            }
        }

        private void showCombinationMenuItem_Click(object sender, EventArgs e)
        {
            ShowCurrentCombinationInTrayIcon();
        }

        #endregion

    }
}
