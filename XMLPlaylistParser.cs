using System;
using System.IO;
using System.Collections;
using System.Xml;

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
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.XmlResolver = null;
                settings.DtdProcessing = DtdProcessing.Ignore;
                settings.ValidationType = ValidationType.None;
                using (XmlReader reader = XmlReader.Create(fpath, settings))
                {
                    CheckFileType(reader);
                    while (reader.ReadToFollowing("key"))
                    {
                        reader.ReadStartElement("key");
                        if (reader.ReadString().ToLower().Equals("location"))
                        {
                            reader.ReadEndElement();
                            reader.ReadStartElement("string");
                            string str = new Uri(reader.ReadString().Trim()).LocalPath;
                            if (str.StartsWith(@"\\localhost\")) str = str.Substring(12);
                            ItemsPaths.Add(str);
                            reader.ReadEndElement();
                        }
                        else
                            reader.ReadEndElement();
                    }
                }
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

        private void CheckFileType(XmlReader reader)
        {
            try {
                reader.ReadStartElement("plist");
            } catch(XmlException) {
                throw new Exception("The file is not a valid Apple PLIST playlist!");
            }
        }
    }
}
