using Digitales_Fotobuch.controls;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace Digitales_Fotobuch.code
{
    static public class TagControlHandling
    {
        static public WrapPanel PlaceTagsOnWrapPanel(WrapPanel wrapPanel, XDocument doc)
        {
            //Liste von Elemente in der XML-Datei erstellen
            List<XElement> elements = doc.Root.Elements("Tag").ToList();

            foreach (XElement element in elements)
            {
                //Erstelle eine neues Tag-Control aus dem abgespeicherten Name, darf nicht geandert werden
                TagControl control = new TagControl(element.Attribute("name").Value, false);

                //TagControl als Kind zum wrap-Panel hinzufügen
                wrapPanel.Children.Add(control);
            }
            
            return wrapPanel;
        }

        static public bool IsAFilterActive(UIElementCollection children)
        {
            foreach (TagControl child in children)
            {
                if (child.GetCurrentTagInfo().IsActive() == true)
                {
                    return true;
                }
            }
            return false;
        }

        static public WrapPanel ResetActiveFilter(WrapPanel wrapPanel)
        {
            for(int i = 0; i < wrapPanel.Children.Count; i++)
            {
                TagControl child = (TagControl)wrapPanel.Children[i];

                if (child.GetCurrentTagInfo().IsActive() == true)
                {
                    child.GetCurrentTagInfo().SetInactive();
                    child.SetControlGray();

                    wrapPanel.Children[i] = child;
                }
            }

            return wrapPanel;
        }

        static public List<Tag> GetAllActiveTags(UIElementCollection children)
        {
            List<Tag> tagList = new List<Tag>();

            foreach (TagControl child in children)
            {
                if (child.GetCurrentTagInfo().IsActive() == true)
                {
                    tagList.Add(child.GetCurrentTagInfo());
                }
            }

            return tagList;
        }
    } 
}
