using Squirrel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenCropper
{
    public partial class AboutF : Form
    {
        public AboutF()
        {
            InitializeComponent();

            Init();
        }

        public void Init()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);

            CurrentVersionLabel.Text = fileVersion.FileVersion;
        }

        private void CheckForUpdatesClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
    }
}
