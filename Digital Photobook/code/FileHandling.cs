using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Digital_Photobook.code
{
    static public class FileHandling
    {
        private static string currentSubFolder;

        static public void CreateNewPictureFolder()
        {
            DateTime dateTime = DateTime.Now;
            string folderName = dateTime.Year.ToString() + "_" + dateTime.Month.ToString() + "_" + dateTime.Day.ToString() + "_" +
                                dateTime.Hour.ToString() + "_" + dateTime.Minute.ToString() + "_" + dateTime.Second;

            //Hauptordner
            string pictureMainFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Digital Photobook");

            //Besteht der Hauptordner noch nicht?
            if (Directory.Exists(pictureMainFolder) == false)
            {
                //Hauptordner erstellen
                Directory.CreateDirectory(pictureMainFolder);
            }

            currentSubFolder = Path.Combine(pictureMainFolder, folderName);

            //Neuen Unterordner erstellen
            Directory.CreateDirectory(currentSubFolder);
        }

        static public string CopyFile(int index, string oldPath)
        {
            string newFilePath = Path.Combine(currentSubFolder, index.ToString() + ".jpg");

            //Bild laden
            Image image = Image.FromFile(oldPath);

            //Bild als JPEG abspeichern
            image.Save(newFilePath, ImageFormat.Jpeg);

            return newFilePath;
        }

        static public int GetFileCount()
        {
            return Directory.EnumerateFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Digital Photobook"), "*.jpg", SearchOption.AllDirectories).Count();
        }
    }
}
