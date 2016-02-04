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
    public partial class AddAccountForm : Form
    {
        public string FileName;

        public AddAccountForm()
        {
            InitializeComponent();
        }

        private void AddAccountForm_Load(object sender, EventArgs e)
        {
            textBox3.Select();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                if (textBox1.Text == "" || textBox3.Text == "")
                {
                    MessageBox.Show("Missing text");
                    return;
                }
            }
            else
            {
                if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
                {
                    MessageBox.Show("Missing text");
                    return;
                }
            }


            string filename = textBox1.Text + ".acc";
            if (!Directory.Exists("Accounts"))
                Directory.CreateDirectory("Accounts");

            if (File.Exists(@"Accounts/" + filename))
            {
                MessageBox.Show("The account already exitst!");
                return;
            }

            using (FileStream fs = new FileStream(@"Accounts/" + filename, FileMode.CreateNew))
            {
                fs.Close();
            }


            string[] lines = new string[0];
            if (checkBox1.Checked)
            {
                lines = new string[]
{
                    textBox3.Text,
                    textBox1.Text,
                    ".."
};
            }
            else
            {

                string pass = StringCipher.Encrypt(textBox2.Text, "fanvadsnoppegottellerhuraa123");

                lines = new string[]
                {
                  textBox3.Text,
                  textBox1.Text,
                  pass
                };

            }

            File.WriteAllLines(@"Accounts/" + filename, lines);
            FileName = filename;
            this.DialogResult = DialogResult.OK;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
                textBox2.Enabled = false;
            else
                textBox2.Enabled = true;
        }
    }
}
