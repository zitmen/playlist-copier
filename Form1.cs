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
                M3UPlaylistParser parser = new M3UPlaylistParser();
                parser.LoadPlaylist(playlist);
                //
                progressBar1.Value = 0;
                progressBar1.Minimum = 0;
                progressBar1.Maximum = parser.SongsCount;
                button3.Enabled = false;
                foreach (string item in parser.ItemsPaths)
                {
                    progressBar1.Value = progressBar1.Value + 1;
                    File.Copy(item, destination + '\\' + Path.GetFileName(item), checkBox1.Enabled);
                }
                //
                progressBar1.Value = 0;
                button3.Enabled = true;
                //
                string message = String.Format("{0} songs were successfully copied into the selected destination.", parser.SongsCount);
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
