using System;
using System.Windows.Forms;

using Gma.System.MouseKeyHook;
using Microsoft.Win32;

namespace VSTSWorkItemShortcut
{
    public partial class Form1 : Form
    {
        private string baseUrl = string.Empty;

        public const int WM_NCLBUTTONDOWN = 0xA1;

        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public Form1()
        {
            InitializeComponent();
            baseUrl = Properties.Settings.Default.BaseUrl;
            HookEvents();
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
        }

        private void txtitemnumber_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (this.txtitemnumber.Text.Trim().Length <= 0)
                    return;
                if (this.txtitemnumber.Text.IsContainsAlphabets())
                    return;

                if (e.KeyCode == Keys.Enter)
                {
                    System.Diagnostics.Process.Start($"{baseUrl}/{this.txtitemnumber.Text}");
                    this.txtitemnumber.Text = string.Empty;
                    this.WindowState = FormWindowState.Minimized;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, @"Error Opening URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HookEvents()
        {
            Hook.GlobalEvents().KeyDown += async (sender, e) =>
                                           {
                                               if ((e.Control && e.Alt && e.KeyCode == Keys.I) ||
                                                   (e.Alt && e.KeyCode == Keys.Space))
                                               {
                                                   this.Show();
                                                   this.WindowState = FormWindowState.Normal;
                                                   this.txtitemnumber.Focus();
                                               }
                                               else if (e.Control && e.Alt && e.KeyCode == Keys.M)
                                               {
                                                   pictureBox2_Click(this, null);
                                               }
                                               else if (e.Control && e.Alt && e.KeyCode == Keys.C)
                                               {
                                                   if (
                                                       MessageBox
                                                          .Show(@"Are you sure you want to close VSTS WI Shortcut opener?",
                                                                @"Confirmation", MessageBoxButtons.YesNo,
                                                                MessageBoxIcon.Question) == DialogResult.Yes)
                                                       this.Close();
                                               }
                                           };
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.notifyIcon1.Visible = true;
                this.ShowInTaskbar = false;
                this.Hide();
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                this.notifyIcon1.Visible = false;
                this.Show();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnhelp_Click(object sender, EventArgs e)
        {
            using (var helpForm = new formhelp("readme.txt"))
            {
                helpForm.StartPosition = FormStartPosition.CenterParent;
                helpForm.ShowDialog();
            }
        }

        private void btnhelp_MouseHover(object sender, EventArgs e)
        {
            var tip = new ToolTip();
            tip.SetToolTip(this.btnhelp, "Help");
        }

        private void pictureBox2_MouseHover(object sender, EventArgs e)
        {
            var tip = new ToolTip();
            tip.SetToolTip(this.pictureBox2, "Minimize");
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            var tip = new ToolTip();
            tip.SetToolTip(this.pictureBox1, "Close");
        }

        private void btnAutoStart_Click(object sender, EventArgs e)
        {
            SetStartup();
        }

        private void SetStartup()
        {
            var rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (!isRegExist("VSTSWorkitemShortcut"))
            {
                rk.SetValue("VSTSWorkitemShortcut", Application.ExecutablePath);
            }
            else
            {
                rk.DeleteValue("VSTSWorkitemShortcut", false);
            }
        }

        private bool isRegExist(string name)
        {
            var rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            return rk.GetValue(name) != null;
        }

        private void btnAutoStart_MouseHover(object sender, EventArgs e)
        {
            var tip = new ToolTip();
            tip.RemoveAll();
            tip.SetToolTip(this.btnAutoStart, !isRegExist("VSTSWorkitemShortcut") ? "Set it as Auto Start on windows startup" : "Remove from Windows Auto Start");
        }

        private void MemoryCleanupTimer_Tick(object sender, EventArgs e)
        {
            MemoryHelper.MemoryCleanup();
        }
    }
}