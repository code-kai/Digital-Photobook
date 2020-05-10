using System.Windows;
using Digitales_Fotobuch.controls;
using Digitales_Fotobuch.code;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using Digital_Photobook.code;

namespace Digitales_Fotobuch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum Direction
        {
            Right,
            Left,
        };

        private readonly XmlHandling xmlHandling = new XmlHandling();

        private List<string> pictureList = new List<string>();

        private DateTime imageClicked = DateTime.Now;

        private int currentPicIndex = 0;
        private bool newPictureMode = false;

        public MainWindow()
        {
            InitializeComponent();

            // Register the Bubble Event Handlers 
            AddHandler(TagControl.TextBoxSavedEvent,
                new RoutedEventHandler(TextBoxSavedEventHandlerMethod));

            AddHandler(TagControl.TagDeleteEvent,
                new RoutedEventHandler(TagDeleteEventHandlerMethod));

            AddHandler(TagControl.TagClickEvent,
                new RoutedEventHandler(TagClickEventEventHandlerMethod));

            Loaded += MainWindowLoaded;
        }

        private void TagDeleteEventHandlerMethod(object sender, RoutedEventArgs e)
        {
            //Durchlaufe alle TagControls
            foreach (TagControl item in wrapPanelTags.Children)
            {
                //Ist das das TagControl welches das Event ausgeloest hat?
                if (item.GetHashCode() == e.Source.GetHashCode())
                {
                    //Weise dem Tag einen neuen Namen zu 
                    xmlHandling.DeleteTag(item.GetCurrentTagInfo().GetName());
                    break;
                }
            }
        }
        private void TextBoxSavedEventHandlerMethod(object sender, RoutedEventArgs e)
        {
            //Durchlaufe alle TagControls
            foreach (TagControl item in wrapPanelTags.Children)
            {
                //Ist das das TagControl welches das Event ausgeloest hat?
                if (item.GetHashCode() == e.Source.GetHashCode())
                {
                    //Flag als nicht gesetzt initialisiert
                    bool duplicateName = false;

                    //Durchlaufe alle TagControls
                    foreach (TagControl control in wrapPanelTags.Children)
                    {
                        //Wenn ein anderes Control schon den selben Namen hat - Flag setzen
                        if ((control.GetCurrentTagInfo().GetName().ToLower() == item.GetCurrentTagInfo().GetName().ToLower()) &&
                            (control.GetHashCode() != item.GetHashCode()))
                        {
                            duplicateName = true;
                            break;
                        }
                    }

                    //Nur einfuegen wenn Flag nicht gesetzt wurde
                    if (duplicateName == false)
                    {
                        //Weise dem Tag einen neuen Namen zu 
                        xmlHandling.InsertNewTag(item.GetCurrentTagInfo().GetName());
                    }
                    else
                    {
                        //Fehler ausgeben und Filter entfernen
                        MessageBox.Show("Diesen Filter gibt es schon", "Fehler!", MessageBoxButton.OK, MessageBoxImage.Error);
                        wrapPanelTags.Children.Remove(item);
                    }
                    break;
                }
            }
        }

        private void TagClickEventEventHandlerMethod(object sender, RoutedEventArgs e)
        {
            if (newPictureMode == false)
            {
                List<Tag> tags = TagControlHandling.GetAllActiveTags(wrapPanelTags.Children);

                //Wenn es angeklickte Filter gibt
                if (tags.Count > 0)
                {
                    pictureList = xmlHandling.GetAllFilteredPictures(tags);

                    currentPicIndex = 1;

                    //Label beschriften
                    SetPictureLabels();

                    SetImage(currentPicIndex - 1);
                }
            }
        }


        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            //Es muessen die XML-Datei ausgelesen werden (bzw. erstellt wenn nicht vorhanden)
            xmlHandling.ReadXmlTags();
            xmlHandling.ReadXmlPicturesTags();

            //Die ausgelesenen Tags werden erstellt
            wrapPanelTags = TagControlHandling.PlaceTagsOnWrapPanel(wrapPanelTags, xmlHandling.GetAllReadedTags());

            //Maximale Anzahl an Bildern setzen
            labelPicMax.Content = FileHandling.GetFileCount().ToString();
        }

        private void ButtonAddTagClick(object sender, RoutedEventArgs e)
        {
            //Ein neues TagControl darf geandert werden
            TagControl tagControl = new TagControl("Neu", true);

            //Neues Element dem FrontEnd hinzufuegen
            wrapPanelTags.Children.Add(tagControl);
        }

        private void TraverseThroughPictures(Direction direction)
        {
            //Nur ausfuehren wenn mindestens ein Filter aktiv ist
            if (TagControlHandling.IsAFilterActive(wrapPanelTags.Children) == true || newPictureMode == true)
            {
                //Funktion beenden wenn man bereits am rechten Rand ist
                if ((direction == Direction.Right) &&
                    (currentPicIndex >= pictureList.Count))

                {
                    return;
                }

                //Funktion beenden wenn man bereits am linken Rand ist
                if ((direction == Direction.Left) &&
                    (currentPicIndex <= 1))
                {
                    return;
                }

                //Index verschieben
                if (direction == Direction.Left)
                {
                    currentPicIndex--;
                }
                else
                {
                    currentPicIndex++;
                }

                SetPictureLabels();

                SetImage(currentPicIndex - 1);
            }
        }

        private bool EditNextPicture()
        {
            //Nur ausfuehren wenn mindestens ein Filter aktiv ist
            if (TagControlHandling.IsAFilterActive(wrapPanelTags.Children) == true)
            {
                //Funktion beenden wenn man bereits am rechten Rand ist
                if (currentPicIndex >= pictureList.Count)
                {

                    //Editiermodus beenden
                    DeactivateEditMode();

                    return false;
                }

                //Kopiere die Datei um
                string newFilePath = FileHandling.CopyFile(currentPicIndex, pictureList[currentPicIndex - 1]);

                //Lege ein XML-Eintrag fuer diese Datei an
                xmlHandling.InsertNewPictureWithTags(newFilePath, TagControlHandling.GetAllActiveTags(wrapPanelTags.Children));

                //Alle Filter zuruecksetzen
                wrapPanelTags = TagControlHandling.ResetActiveFilter(wrapPanelTags);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ButtonPicRiClick(object sender, RoutedEventArgs e)
        {
            bool editWorked = true;

            if (newPictureMode == true)
            {
                editWorked = EditNextPicture();
            }

            if ((newPictureMode == false) ||
                (newPictureMode == true && editWorked == true))
            {
                TraverseThroughPictures(Direction.Right);
            }
        }
        private void ButtonPicLeClick(object sender, RoutedEventArgs e)
        {
            if (newPictureMode == false)
            {
                TraverseThroughPictures(Direction.Left);
            }
        }

        private void ButtonReadPicsClick(object sender, RoutedEventArgs e)
        {
            if (newPictureMode == false)
            {
                ActivateEditMode();
            }
            else
            {
                DeactivateEditMode();
            }
        }

        private void ActivateEditMode()
        {
            //Erstelle einen FileDialog
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg;*.jpg;*.JPG;*.JPEG;*.PNG",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            //Oeffnen den Dialog und warte bis alle bilder ausgelesen sind
            if (fileDialog.ShowDialog() == true)
            {
                //Modus -> es werden Bilder neu zugewiesen
                newPictureMode = true;

                //Liste hat nun alle neuen Dateien eingelesen
                pictureList = fileDialog.FileNames.ToList();

                //Variablen zuruecksetzen
                currentPicIndex = 1;

                //Label beschriften
                SetPictureLabels();

                //Bild des Einlesen aendern
                SetButtonReadPicsImage("x");

                //Bild setzen
                SetImage(0);

                //Neuen Ordner erstellen in den die Dateien kopiert werden
                FileHandling.CreateNewPictureFolder();
            }
        }

        private void DeactivateEditMode()
        {
            //Bild des Einlesen aendern
            SetButtonReadPicsImage("camera");

            //Modus -> es werden Bilder gefiltert
            newPictureMode = false;

            wrapPanelTags = TagControlHandling.ResetActiveFilter(wrapPanelTags);

            //Liste löschen
            pictureList.Clear();

            currentPicIndex = 0;

            //Label beschriften
            SetPictureLabels();

            //Bild aus Control loeschen
            imageCurrentPic.Source = new BitmapImage();

            //Maximale Anzahl an Bildern setzen
            labelPicMax.Content = FileHandling.GetFileCount().ToString();
        }

        private void SetPictureLabels()
        {
            //Anzahl Bilder als Max setzen
            labelPicMax.Content = pictureList.Count.ToString();

            progressBarLoadPic.Maximum = pictureList.Count;

            //Aktueller Bilderindex setzen
            labelPicCounter.Content = currentPicIndex.ToString();

            progressBarLoadPic.Value = currentPicIndex;
        }

        private void SetButtonReadPicsImage(string resourceName)
        {
            ImageSource imageSource = new BitmapImage(new Uri("resources/" + resourceName + ".png", UriKind.Relative));
            ImageBrush  imageBrush  = new ImageBrush(imageSource);

            buttonReadPics.Background = imageBrush;
        }

        private void SetImage(int index)
        {
            ImageSource imageSource = new BitmapImage(new Uri(pictureList[index], UriKind.Absolute));
            imageCurrentPic.Source = imageSource;
        }

        private void ImageCurrentPicPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (currentPicIndex != 0)
            {
                if ((DateTime.Now - imageClicked).TotalMilliseconds < 500.0)
                {
                    string photoViewerPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

                    //Ist das Betriebsystem 64bit?
                    if (Environment.Is64BitOperatingSystem == true)
                    {
                        //32-Bit Ordner zu 64-Bit Ordner 
                        photoViewerPath = photoViewerPath.Replace(" (x86)", "");
                    }

                    //Process mit Parametern vorbereiten
                    ProcessStartInfo psi = new ProcessStartInfo("rundll32.exe", string.Format("\"{0}{1}\", ImageView_Fullscreen {2}",
                                                                photoViewerPath, @"\Windows Photo Viewer\PhotoViewer.dll", pictureList[currentPicIndex - 1]));

                    //Photo Viewer starten
                    Process.Start(psi);
                }

                imageClicked = DateTime.Now;
            }
        }
    }
}

