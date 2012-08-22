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
            EnableCopyButton();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(this);
            if (result != DialogResult.OK) return;
            if (openFileDialog1.FileName == null) return;
            textBox1.Text = openFileDialog1.FileName;
            //
            // read the playlist
            string playlist = textBox1.Text;
            PlaylistParser parser = null;
            try
            {
                string pl_type = Path.GetExtension(playlist).Substring(1);  // file extension without the dot
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //
            // check the files and add them into the checklist
            checkedListBox1.Items.Clear();
            foreach (string item in parser.ItemsPaths)
                checkedListBox1.Items.Add(item, File.Exists(item));
            //
            // status message
            if (checkedListBox1.CheckedItems.Count == checkedListBox1.Items.Count)
            {
                string message = String.Format("All {0} items were successfully imported.", checkedListBox1.CheckedItems.Count);
                MessageBox.Show(this, message, "Done.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string message = String.Format
                (
                    "{0} were not found at the specified locations, hence they are not checked in tle list! The remaining {1} items were successfully imported.",
                    checkedListBox1.Items.Count - checkedListBox1.CheckedItems.Count, checkedListBox1.CheckedItems.Count
                );
                MessageBox.Show(this, message, "Done.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //
            EnableCopyButton();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog(this);
            if (result != DialogResult.OK) return;
            textBox2.Text = folderBrowserDialog1.SelectedPath;
            //
            EnableCopyButton();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = checkedListBox1.CheckedItems.Count;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            checkBox1.Enabled = false;
            checkedListBox1.Enabled = false;
            //
            BackgroundWorker bgWork = new BackgroundWorker();
            bgWork.DoWork += new DoWorkEventHandler(CopyFilesWork);
            bgWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CopyFilesDone);
            bgWork.RunWorkerAsync();
        }

        private void CopyFilesWork(object sender, DoWorkEventArgs e)
        {
            int copied = 0;
            string destination = textBox2.Text;
            foreach (string item in checkedListBox1.CheckedItems)
            {
                progressBar1.Value = progressBar1.Value + 1;
                try
                {
                    File.Copy(item, destination + '\\' + Path.GetFileName(item), checkBox1.Enabled);
                    copied++;
                }
                catch (IOException) { }
            }
            e.Result = copied;
        }

        private void CopyFilesDone(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = 0;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            checkBox1.Enabled = false;
            checkedListBox1.Enabled = false;
            //
            // status message
            int copied = (int)e.Result;
            if (copied == checkedListBox1.CheckedItems.Count)
            {
                MessageBox.Show
                (
                    this, "All selected songs were successfully copied into the selected destination.",
                    "Done.", MessageBoxButtons.OK, MessageBoxIcon.Information
                );
            }
            else
            {
                MessageBox.Show(this, String.Format
                (
                    "{0} out of {1} selected songs were successfully copied into the selected destination. Some of the files were not found!",
                    copied, checkedListBox1.CheckedItems.Count
                ), "Done.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            /*
             * Elegant solution of the problem: http://stackoverflow.com/questions/4454058/no-itemchecked-event-in-a-checkedlistbox
             * ---
             * A nice trick to deal with events that you cannot process when they are raised is to delay the processing.
             * Which you can do with the Control.BeginInvoke() method, it runs as soon as all events are dispatched,
             * side-effects are complete and the UI thread goes idle again. Often helpful for TreeView as well, another cranky control.
             */
            this.BeginInvoke((MethodInvoker)delegate
            {
                EnableCopyButton();
            });
        }

        private void EnableCopyButton()
        {
            // if it wasn't an empty playlist, enable the 'copy' button and if destination exists
            if ((checkedListBox1.CheckedItems.Count > 0) && (Directory.Exists(textBox2.Text)))
                button3.Enabled = true;
            else
                button3.Enabled = false;
        }
    }
}
