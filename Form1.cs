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

        private void button3_Click(object sender, EventArgs e)  // TODO: refactor - add the song into a list after it's selected + check if files exist + checkboxes
        {
            string playlist    = textBox1.Text;
            string destination = textBox2.Text;
            //
            try
            {
                PlaylistParser parser = null;
                string pl_type = Path.GetExtension(playlist).Substring(1);  // extension without the dot
                //
                if (pl_type.ToUpper().Equals("ASX"))
                    parser = new ASXPlaylistParser();
                else if (pl_type.ToUpper().Equals("M3U") || pl_type.ToUpper().Equals("M3U8") || pl_type.ToUpper().Equals("BSL"))
                    parser = new M3UPlaylistParser();
                else if (pl_type.ToUpper().Equals("PLS") || pl_type.ToUpper().Equals("KPL"))
                    parser = new PLSPlaylistParser();
                else if (pl_type.ToUpper().Equals("WPL"))
                    parser = new WPLPlaylistParser();
                else if (pl_type.ToUpper().Equals("XML"))
                    parser = new XMLPlaylistParser();
                else if (pl_type.ToUpper().Equals("XSPF"))
                    parser = new XSPFPlaylistParser();
                else
                    throw new Exception("Unsupported file extension!");
                //
                parser.LoadPlaylist(playlist);
                //
                progressBar1.Value = 0;
                progressBar1.Minimum = 0;
                progressBar1.Maximum = parser.SongsCount;
                button3.Enabled = false;
                foreach (string item in parser.ItemsPaths)
                {
                    progressBar1.Value = progressBar1.Value + 1;
                    File.Copy(item, destination + '\\' + Path.GetFileName(item), checkBox1.Enabled);    // TODO: skipping files!
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
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
