using System;
using System.Collections;
using System.IO;
using System.Text;

namespace PlaylistCopier
{
    /*
     *.M3U  (WinAmp)
     *.M3U8 (WinAmp UTF-8 encoded)
     *.BSL  (BSplayer)
     */
    class M3UPlaylistParser : PlaylistParser
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
                    if (song[0] == '#') continue;   // comment (info for an audio player)
                    ItemsPaths.Add(song);
                }
            }
            catch (IOException)
            {
                throw new Exception("Error occured during the file reading process!");
            }
        }

        private void CheckFileType(string first_line)
        {
            if (!first_line.Equals("#EXTM3U"))
                throw new Exception("The file is not a valid M3U/M3U8/BSL playlist!");
        }
    }
}
