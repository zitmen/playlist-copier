using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PlaylistCopier
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string playlist    = textBox1.Text;
            string destination = textBox2.Text;
            //
            try
            {
                string[] items = File.ReadAllText(playlist).Replace("\r\n", "\n").Split('\n');
                progressBar1.Value = 0;
                progressBar1.Minimum = 0;
                progressBar1.Maximum = items.Length;
                button3.Enabled = false;
                int songs = 0;
                foreach (string item in items)
                {
                    progressBar1.Value = progressBar1.Value + 1;
                    string song = item.Trim();
                    if (song.Length == 0) continue;
                    if (song[0] == '#') continue;
                    File.Copy(song, destination + '\\' + Path.GetFileName(song), checkBox1.Enabled);
                    ++songs;
                }
                //
                progressBar1.Value = 0;
                button3.Enabled = true;
                //
                string message = String.Format("{0} songs were successfully copied into the selected destination.", songs);
                MessageBox.Show(this, message, "Done.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (IOException)
            {
                string message = "There was an error during the copy operation!\nCheck if the playlist file and the destination folder exist.";
                MessageBox.Show(this, message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
