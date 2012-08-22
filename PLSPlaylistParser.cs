using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PlaylistCopier
{
    /*
     *.PLS (iTunes, QuickTime Player, RealPlayer, Winamp, Foobar, etc.)
     *.KPL (KMPlayer)
     */
    class PLSPlaylistParser : PlaylistParser
    {
        override public void LoadPlaylist(string fpath)
        {
            ItemsPaths = new ArrayList();
            //
            try
            {
                string[] items = File.ReadAllText(fpath, Encoding.GetEncoding("UTF-8")).Replace("\r\n", "\n").Split('\n');
                CheckFileType(items[0]);
                foreach (string item in items)
                {
                    string song = item.Trim();
                    if (song.Length == 0) continue; // empty line
                    Match match = Regex.Match(song, @"^File[0-9]+=(.*)$", RegexOptions.IgnoreCase); // looking for file paths
                    if (match.Success)
                        ItemsPaths.Add(match.Groups[0].Value);
                }
            }
            catch (IOException)
            {
                throw new Exception("Error occured during the file reading process!");
            }
        }

        private void CheckFileType(string first_line)
        {
            if (!first_line.Equals("[playlist]"))
                throw new Exception("The file is not a valid KPL playlist!");
        }
    }
}
