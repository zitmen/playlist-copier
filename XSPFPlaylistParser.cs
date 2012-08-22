using System;
using System.IO;
using System.Collections;
using System.Xml;
using System.Text;

namespace PlaylistCopier
{
    /*
     *.XSPF (VLC Player)
     */
    class XSPFPlaylistParser : PlaylistParser
    {
        override public void LoadPlaylist(string fpath)
        {
            ItemsPaths = new ArrayList();
            //
            try
            {
                StreamReader docIn = new StreamReader(fpath, Encoding.GetEncoding("UTF-8"));
                XmlDocument doc = new XmlDocument();
                doc.Load(docIn);
                CheckFileType(doc);
                foreach (XmlElement item in doc.GetElementsByTagName("location"))
                    ItemsPaths.Add(item.InnerText.Trim());
            }
            catch (IOException)
            {
                throw new Exception("Error occured during the file reading process!");
            }
            catch (XmlException)
            {
                throw new Exception("Error occured during the file parsing process!");
            }
        }

        private void CheckFileType(XmlDocument doc)
        {
            XmlElement root = doc.DocumentElement;
            if (!root.Name.ToLower().Equals("playlist"))
                throw new Exception("The file is not a valid XSPF playlist!");
            //
            if (!doc.NamespaceURI.Equals("http://xspf.org/ns/0/"))
                throw new Exception("The file is not a valid XSPF playlist!");
        }
    }
}
