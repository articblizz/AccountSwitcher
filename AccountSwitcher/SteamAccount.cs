using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

namespace AccountSwitcher
{
    public partial class SteamAccount : UserControl
    {
        string fileName;

        public string Nickname;
        string username;
        string password;
        Form1 form;

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr WindowHandle);
        public const int SW_RESTORE = 9;

        public SteamAccount(string file, Form1 form)
        {
            this.fileName = file;
            this.form = form;
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void SteamAccount_Load(object sender, EventArgs e)
        {
            var lines = File.ReadAllLines(@"Accounts/" + fileName);
            username = lines[1];
            password = lines[2];
            nameLabel.Text = lines[0];
            label1.Text = "(" + username + ")";
            Nickname = lines[0];


            nameLabel.Location = new Point(this.Width / 2 - nameLabel.Width / 2, nameLabel.Location.Y) ;
            button1.Location = new Point(this.Width / 2 - button1.Width / 2, button1.Location.Y);
            label1.Location = new Point(this.Width / 2 - label1.Width / 2, label1.Location.Y);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure?", "Delete Account", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // delete account
                File.Delete(@"Accounts/" + fileName);
                form.Accounts.Remove(this);
                form.Controls.Remove(this);
                form.SortAccounts();
            }
        }

        public void Switch()
        {
            if (password == "..")
            {
                var processes = Process.GetProcessesByName("Steam");

                if (processes.Length > 0)
                    processes[0].Kill();
                Thread.Sleep(250);

                Process.Start(Properties.Settings.Default.SteamPath, "-login");
                Thread.Sleep(3500);
                FocusSteam();

                SendKeys.Send("+{TAB}");
                SendKeys.Send("^A");
                SendKeys.Send(username);
                SendKeys.Send("{TAB}");
            }
            else
            {
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;

                    var processes = Process.GetProcessesByName("Steam");

                    if (processes.Length > 0)
                        processes[0].Kill();
                    Thread.Sleep(250);


                    string login = $"-login {username} {StringCipher.Decrypt(password, decryption)}";
                    //string loginString = string.Format("-login {0} {1}", username, StringCipher.Decrypt(password, "fanvadsnoppegottellerhuraa123"));
                    Process.Start(Properties.Settings.Default.SteamPath, login);
                }).Start();
            }
        }

        private void FocusSteam()
        {
            Process[] objProcesses = Process.GetProcessesByName("Steam");
            if (objProcesses.Length > 0)
            {
                IntPtr hWnd = IntPtr.Zero;
                hWnd = objProcesses[0].MainWindowHandle;
                ShowWindowAsync(new HandleRef(null, hWnd), SW_RESTORE);
                SetForegroundWindow(objProcesses[0].MainWindowHandle);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Switch();
        }
    }
}
