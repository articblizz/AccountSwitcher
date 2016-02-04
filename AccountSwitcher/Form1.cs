using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AccountSwitcher
{
    public partial class Form1 : Form
    {
        public List<SteamAccount> Accounts= new List<SteamAccount>();

        public Form1()
        {
            InitializeComponent();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AddAccountForm form = new AddAccountForm())
            {
                if(form.ShowDialog() == DialogResult.OK)
                {
                    SteamAccount acc = new SteamAccount(form.FileName, this);
                    Controls.Add(acc);
                    Accounts.Add(acc);

                    SortAccounts();
                }
            }
        }

        public void SortAccounts()
        {
            for (int i = 0; i < Accounts.Count; i++)
            {
                int x = (int)(i % 3) * 180;
                int y = (int)(i / 3) * 140;
                Accounts[i].Location = new Point(10 + x, 40 + y);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if(!File.Exists(Properties.Settings.Default.SteamPath))
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = "Please show me where the Steam.exe is located!";
                    ofd.Filter = "Steam Executable File (*.exe)|*.exe";
                    if(ofd.ShowDialog() == DialogResult.OK)
                    {
                        Properties.Settings.Default.SteamPath = ofd.FileName;
                        Properties.Settings.Default.Save();
                    }
                }
            }

            LoadAccounts();

            this.Icon = Properties.Resources.steamico;
            notifyIcon1.Icon = Properties.Resources.steamico;

            if (Properties.Settings.Default.StartMinimized == true)
            {
                toolStripComboBox1.SelectedIndex = 1;
                //WindowState = FormWindowState.Minimized;
                
            }
            else
                toolStripComboBox1.SelectedIndex = 0;
        }

        void LoadAccounts()
        {
            try
            {
                var files = Directory.GetFiles("Accounts");

                foreach(string file in files)
                {
                    SteamAccount acc = new SteamAccount(Path.GetFileName(file), this);
                    Controls.Add(acc);
                    Accounts.Add(acc);
                }
                SortAccounts();
            }
            catch { }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if(WindowState == FormWindowState.Minimized)
            {
                RefreshContext();
                this.Hide();
                notifyIcon1.Visible = true;
            }
        }

        void RefreshContext()
        {
            contextMenuStrip1.Items.Clear();
            Accounts.ForEach(account =>
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = account.Nickname;
                item.Click += new EventHandler((s, a) => { account.Switch(); });
                contextMenuStrip1.Items.Add(item);

            });
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
            this.WindowState = FormWindowState.Normal;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Q: Is this safe?\r\nA: If you're using SteamGuard properly, you're good. This program is encrypting every password that stores. Source sends to anyone who wants it.");
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox1.SelectedIndex == 1)
            {
                Properties.Settings.Default.StartMinimized = true;
                Properties.Settings.Default.Save();

                RegisterInStartup(true);
            }
            else
            {

                Properties.Settings.Default.StartMinimized = false;
                Properties.Settings.Default.Save();

                RegisterInStartup(false);
            }
        }

        private void RegisterInStartup(bool isChecked)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (isChecked)
            {
                registryKey.SetValue("AccountSwitcher", Application.ExecutablePath);
            }
            else
            {
                try
                {
                    registryKey.DeleteValue("AccountSwitcher");

                }
                catch { }
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.StartMinimized == true)
            {
                RefreshContext();
                this.Hide();
                notifyIcon1.Visible = true;
            }
        }
    }
}
