using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Digitales_Fotobuch.code;
using System.Windows;
using System.Globalization;

namespace Digitales_Fotobuch.controls
{
    /// <summary>
    /// Interaktionslogik für tagControl.xaml
    /// </summary>
    public partial class TagControl : UserControl
    {
        private readonly Tag tagData;
        public static readonly RoutedEvent TextBoxSavedEvent =
                        EventManager.RegisterRoutedEvent("TextBoxSavedEvent", RoutingStrategy.Bubble,
                        typeof(RoutedEventHandler), typeof(TagControl));

        public static readonly RoutedEvent TagDeleteEvent =
                        EventManager.RegisterRoutedEvent("TagDeleteEvent", RoutingStrategy.Bubble,
                        typeof(RoutedEventHandler), typeof(TagControl));

        public TagControl(string name, bool changePossible)
        {
            //Daten muessen festgelegt sein, bevor Element initalisiert wird
            tagData = new Tag(name, false, changePossible);

            InitializeComponent();

            //Loaded funktion wird erweitert
            Loaded += ControlLoaded;
        }

        public event RoutedEventHandler SettingConfirmed
        {
            add    { AddHandler(TextBoxSavedEvent,    value); }
            remove { RemoveHandler(TextBoxSavedEvent, value); }
        }

        public event RoutedEventHandler TagDeleted
        {
            add    { AddHandler(TagDeleteEvent,    value); }
            remove { RemoveHandler(TagDeleteEvent, value); }
        }


        public Tag GetCurrentTagInfo()
        {
            return tagData;
        }

        private void ControlLoaded(object sender, EventArgs e)
        {
            // Text wird auf den Namen des Tags angepasst
            tagConTextBox.Text = tagData.GetName();

            //TagControl an den neuen Namen anpassen
            SetNewTagControlWidth();

            //Nur enable wenn das Control neu ist
            tagConTextBox.IsEnabled = tagData.IsChangePossible();
        }

        private void TagConTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            //TagControl an den neuen Namen anpassen
            SetNewTagControlWidth();
        }

        private void TagConButtonClick(object sender, RoutedEventArgs e)
        {
            //Event ans Hauptfenster senden um Tag auch in XML zu loeschen
            RaiseEvent(new RoutedEventArgs(TagDeleteEvent));

            //Loescht das UserControl
            ((WrapPanel)Parent).Children.Remove(this);
        }

        private void TagConGridPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Wenn der Modus Filtern ist
            if (tagConTextBox.IsEnabled == false)
            {
                if (tagData.IsActive() == true)
                {
                    //Farbe auf Inaktiv umstellen
                    SetControlGray();

                    // Tag als inaktiv setzen
                    tagData.SetInactive();
                }
                else
                {
                    //Farbe auf Aktiv umstellen
                    SetControlGreen();

                    // Tag als aktiv setzen
                    tagData.SetActive();
                }
            }
        }

        public void SetControlGreen()
        {
            tagConEllipse.Fill       = Brushes.Green;
            tagConTextBox.Background = Brushes.Green;
            tagConButton.Background  = Brushes.DarkGreen;
        }

        public void SetControlGray()
        {
            tagConEllipse.Fill       = Brushes.DarkGray;
            tagConTextBox.Background = Brushes.DarkGray;
            tagConButton.Background  = Brushes.LightGray;
        }

        private void SetNewTagControlWidth()
        {
            //Laenge des Textes anhand der verwendeten Font errechnen
            FormattedText formattedText = new FormattedText(tagConTextBox.Text,
                                                            CultureInfo.CurrentCulture,
                                                            FlowDirection.LeftToRight,
                                                            new Typeface(tagConTextBox.FontFamily, 
                                                                         tagConTextBox.FontStyle,
                                                                         tagConTextBox.FontWeight,
                                                                         tagConTextBox.FontStretch),
                                                            tagConTextBox.FontSize,
                                                            tagConTextBox.Foreground,
                                                            VisualTreeHelper.GetDpi(tagConTextBox).PixelsPerDip);

            //Laenge des Textes + Margin der TextBox zur Ellipse + Laenge des Button 
            //+ 10 als Offset zwischen Text und Button
            double controlSize = formattedText.Width + tagConTextBox.Margin.Left + tagConButton.Width + 10.0;

            //Laenge zuweisen
            tagConGrid.Width    = controlSize;
            tagConEllipse.Width = controlSize;
        }

        private void UserControlKeyDown(object sender, KeyEventArgs e)
        {
            //Wenn der Benutzer eine Enter-Taste gedrueckt hat -> Speichere 
            if(e.Key == Key.Enter)
            {
                //Nur ausfuehren wenn sich der Text geaendert hat
                if (tagData.GetName() != tagConTextBox.Text)
                {
                    //Inhalt des TagControls speichern
                    bool nameChanged = tagData.SetName(tagConTextBox.Text);

                    //Wurde der Name geaendert?
                    if (nameChanged == true)
                    {
                        //Wenn Textbox geandert wurde -> Keine Aenderung mehr zulassen
                        tagConTextBox.IsEnabled = false;

                        //Event ans Hauptfenster senden um Tag auch in XML zu veraendern
                        RaiseEvent(new RoutedEventArgs(TextBoxSavedEvent));
                    }
                }
            }
        }

        private void TagConTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            //Nur ausfuehren wenn sich der Text geaendert hat
            if (tagData.GetName() != tagConTextBox.Text)
            {
                //Inhalt des TagControls speichern
                bool nameChanged = tagData.SetName(tagConTextBox.Text);

                //Wurde der Name geaendert?
                if (nameChanged == true)
                {
                    //Wenn Textbox geandert wurde -> Keine Aenderung mehr zulassen
                    tagConTextBox.IsEnabled = false;

                    //Event ans Hauptfenster senden um Tag auch in XML zu veraendern
                    RaiseEvent(new RoutedEventArgs(TextBoxSavedEvent));
                }
            }
        }
    }
}
