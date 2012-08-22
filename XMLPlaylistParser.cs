using System;
using System.IO;
using System.Collections;
using System.Xml;
using System.Text;

namespace PlaylistCopier
{
    /*
     *.XML (iTunes)
     */
    class XMLPlaylistParser : PlaylistParser
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
                foreach (XmlElement item in doc.GetElementsByTagName("key"))
                    if(item.InnerText.ToLower().Equals("location"))
                        ItemsPaths.Add(item.NextSibling.InnerText.Trim());
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
            //doc.DocumentType.Name.ToLower().Equals("plist")
            XmlElement root = doc.DocumentElement;
            if (!root.Name.ToLower().Equals("plist"))
                throw new Exception("The file is not a valid Apple PLIST playlist!");
        }
    }
}
