using System;
using System.IO;

namespace Digital_Photobook.code
{
    static public class FileHandling
    {
        private static string currentSubFolder;
        static public string CreateNewPictureFolder()
        {
            DateTime dateTime = DateTime.Now;
            string folderName = dateTime.Year.ToString() + "_" + dateTime.Month.ToString() + "_" + dateTime.Day.ToString() + "_" +
                                dateTime.Hour.ToString() + "_" + dateTime.Minute.ToString() + "_" + dateTime.Second;

            //Hauptordner
            string pictureMainFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Digitales Fotobuch");

            //Besteht der Hauptordner noch nicht?
            if (Directory.Exists(pictureMainFolder) == false)
            {
                //Hauptordner erstellen
                Directory.CreateDirectory(pictureMainFolder);
            }

            currentSubFolder = Path.Combine(pictureMainFolder, folderName);

            //Neuen Unterordner erstellen
            Directory.CreateDirectory(currentSubFolder);

            return currentSubFolder;
        }

        static void CopyFile(int index, string oldPath)
        {
            string newFilePath = Path.Combine(currentSubFolder, index.ToString() + ".jpg");


            string ext = Path.GetExtension(oldPath).ToLower();

            if (ext == ".png")
            {
            }
        }
    }
}
