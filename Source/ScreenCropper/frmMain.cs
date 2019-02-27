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
            this.Size = SystemInformation.VirtualScreen.Size;
            this.Show();
        }


        private List<Key> VirtualKeyCodesForCombinations = new List<Key>() { Key.LeftCtrl, Key.LeftAlt, Key.V };

        private IntPtr HookID = IntPtr.Zero;
        private static LowLevelKeyboardProc HookProc;


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
    }
}
