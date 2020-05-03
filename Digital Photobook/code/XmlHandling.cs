using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Linq;
using System;

namespace Digitales_Fotobuch.code
{
    public class XmlHandling
    {
        private XDocument docTags         = new XDocument();
        private XDocument docPictures     = new XDocument();
        private XDocument docPicturesTags = new XDocument();

        #region Oeffentliche Funktionen

        public XDocument GetAllReadedTags()
        {
            return docTags;
        }
        public void ReadXmlTags()
        {
            /* Ermittle den Pfad der XML-Datei fuer Tags */
            string xmlFileTagsPath = GetXmlFilePath("Tags");

            /* Wenn kein XML-File fuer Tags vorhanden ist -> erstelle eins */
            if (File.Exists(xmlFileTagsPath) == false)
            {
                //Erstelle Tags
                CreateXmlFile("Tags");

                //Lese Tags
                docTags = ReadXmlFile(xmlFileTagsPath);

                //Erstelle ein Standard Tag mit der aktuellen Jahreszahl
                InsertNewTag(DateTime.Now.Year.ToString());

                //Lese Tags
                docTags = ReadXmlFile(xmlFileTagsPath);
            }
            else
            {
                //Ansonsten nur Lesen
                docTags = ReadXmlFile(xmlFileTagsPath);
            }
        }
        public void ReadXmlPictures()
        {
            /* Ermittle den Pfad der XML-Datei fuer Pictures */
            string xmlFilePicsPath = GetXmlFilePath("Pictures");

            /* Wenn kein XML-File fuer Pictures vorhanden ist -> erstelle eins */
            if (File.Exists(xmlFilePicsPath) == false)
            {
                //Erstelle Pictures
                CreateXmlFile("Pictures");

                //Lese Pictures
                docPictures = ReadXmlFile(xmlFilePicsPath);
            }
            else
            {
                //Ansonsten nur Lesen
                docPictures = ReadXmlFile(xmlFilePicsPath);
            }
        }
        public void ReadXmlPicturesTags()
        {
            /* Ermittle den Pfad der XML-Datei fuer Zwischentabelle */
            string xmlFilePicsTagsPath = GetXmlFilePath("PicturesTags");

            /* Wenn kein XML-File fuer Zwischentabelle vorhanden ist -> erstelle eins */
            if (File.Exists(xmlFilePicsTagsPath) == false)
            {
                //Erstelle Zwischentabelle
                CreateXmlFile("PicturesTags");

                //Lese Zwischentabelle
                docPicturesTags = ReadXmlFile(xmlFilePicsTagsPath);
            }
            else
            {
                //Ansonsten nur Lesen
                docPicturesTags = ReadXmlFile(xmlFilePicsTagsPath);
            }
        }

        public void InsertNewTag(string tagName)
        {
            /* Ermittle den Pfad der XML-Datei fuer Tags */
            string xmlFileTagsPath = GetXmlFilePath("Tags");

            //Neues Tag-Element erstellen
            XElement tag = new XElement("Tag");

            //Attribut hinzufuegen
            tag.Add(new XAttribute("name", tagName));

            //Haenge ein neues Element im Root an
            docTags.Element("root").Add(tag);

            //XML-Datei speichern
            docTags.Save(xmlFileTagsPath);
        }

        public void ChangeTag(string oldTagName, string newTagName)
        {
            //Suche Tag (LINQ)
            XElement element = docTags.Descendants("Tag")
                                      .Where(x => (string)x.Attribute("name") == oldTagName).Single();

            //Aendere Namen des Tags
            element.SetAttributeValue("name", newTagName);

            //XML-Datei speichern
            docTags.Save(GetXmlFilePath("Tags"));
        }

        public void DeleteTag(string tagName)
        {
            //Loesche das gewuenschte Element (LINQ)
            docTags.Descendants("Tag")
                .Where(x => (string)x.Attribute("name") == tagName)
                .Remove();

            //XML-Datei speichern
            docTags.Save(GetXmlFilePath("Tags"));
        }
        #endregion
        
        #region Private Funktionen
        private XDocument ReadXmlFile(string xmlPath)
        {
            //Lese das gesamte XML File aus
            return XDocument.Load(xmlPath);
        }
        private void CreateXmlFile(string fileName)
        {
            /* Basispfad erstellen */
            string curXmlPath = GetExePath();

            /* XML Ordner an den Pfad hinzufuegen */
            curXmlPath = Path.Combine(curXmlPath, "xml");

            /* Wenn kein "xml" vorhanden ist -> erstelle ihn */
            if (Directory.Exists(curXmlPath) == false)
            {
                Directory.CreateDirectory(curXmlPath);
            }

            /* XML-Datei zum Pfad anfuegen */
            curXmlPath = Path.Combine(curXmlPath, fileName + ".xml");

            /* Root Element und XML Datei erstellen */
            CreateRootElement(curXmlPath);
        }

        private void CreateRootElement(string xmlPath)
        {
            //Erstelle ein root Element
            XDocument srcTree = new XDocument
            (
                new XElement("root")
            ) ;

            //Speicher das Root Element in der XML-Datei
            srcTree.Save(xmlPath);
        }

        private string GetXmlFilePath(string fileName)
        {
            /* Basispfad erstellen */
            string curTagsXmlPath = GetExePath();

            /* XML Ordner an den Pfad hinzufuegen */
            curTagsXmlPath = Path.Combine(curTagsXmlPath, "xml");

            /* XML-Datei an den Pfad anfuegen */
            curTagsXmlPath = Path.Combine(curTagsXmlPath, fileName + ".xml");

            return curTagsXmlPath;
        }

        private string GetExePath()
        {
            string curExePath = Assembly.GetEntryAssembly().Location;

            curExePath = Path.GetDirectoryName(curExePath);

            return curExePath;
        }
        #endregion
    }
}
