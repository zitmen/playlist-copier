using System;
using System.IO;
using System.Collections;
using System.Xml;
using System.Text;

namespace PlaylistCopier
{
    /*
     *.WPL (Windows Media Player)
     */
    class WPLPlaylistParser : PlaylistParser
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
                foreach (XmlElement item in doc.GetElementsByTagName("media"))
                    ItemsPaths.Add(item.GetAttribute("src").Trim());
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
            if (!doc.InnerXml.TrimStart().Substring(0, 5).ToLower().Equals("<?wpl"))
                throw new Exception("The file is not a valid WPL-SMIL playlist!");
            //
            XmlElement root = doc.DocumentElement;
            if (!root.Name.ToLower().Equals("smil"))
                throw new Exception("The file is not a valid WPL-SMIL playlist!");
        }
    }
}
