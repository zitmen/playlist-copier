using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace PlaylistCopier
{
    abstract class PlaylistParser
    {
        public ArrayList ItemsPaths { set; get; }
        public int SongsCount { get { return ItemsPaths.Count; } }

        abstract public void LoadPlaylist(string fpath);
    }
}
