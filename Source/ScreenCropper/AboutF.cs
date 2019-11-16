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

        private async void CheckForUpdatesClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoadingIndicator.Visible = true;

            try
            {
                var updateInfo = await Program.UpdManager.CheckForUpdate();

                if (updateInfo.ReleasesToApply.Count > 0)
                {
                    UpdateInfoRichTextBox.Text = "Updates available! Downloading latest one, version " + updateInfo.ReleasesToApply.Last().Version;

                    await Program.UpdManager.UpdateApp();

                    UpdateInfoRichTextBox.Text = "Version " + updateInfo.ReleasesToApply.Last().Version + " has been downloaded!" +
                        "Changes will take effect after restart!";
                }
                else
                {
                    UpdateInfoRichTextBox.Text = "You're up to date!";
                }
            }
            catch (Exception ex)
            {
                UpdateInfoRichTextBox.Text = "Could not check for updates!";
            }

            LoadingIndicator.Visible = false;
        }
    }
}
