using kar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Karaboss.Themes
{
    public class Theme
    {
        public string Name { get; set; }        

        public Color ActiveColor { get; set; }
        public Color HighlightColor { get; set; }
        public Color InactiveColor { get; set; }

        public Color ActiveBorderColor { get; set; }
        public Color InactiveBorderColor { get; set; }

        public Color BgColor { get; set; }
        public Color Grad0Color { get; set; }
        public Color Grad1Color { get; set; }
        public Color Rhythm0Color { get; set; }
        public Color Rhythm1Color { get; set; }

        public Color ActiveInstrumentalColor { get; set; }

        public Color InactiveChordColor { get; set; }
        public Color HighlightChordColor { get; set; }

        public Theme (string name)
        {
            Name = name;
        }
        
        public Theme(
            string name,
            Color activecolor, 
            Color highlightcolor, 
            Color inactivecolor, 
            Color activebordercolor, 
            Color inactivebordercolor, 
            Color bgcolor, 
            Color grad0color, 
            Color grad1color, 
            Color rhythm0color, 
            Color rhythm1color,
            Color activeinstrumentalcolor,
            Color inactivechordcolor,
            Color highlightchordcolor) 
        { 
            
            Name = name;
            ActiveColor = activecolor;
            HighlightColor = highlightcolor;
            InactiveColor = inactivecolor;
            ActiveBorderColor = activebordercolor;
            InactiveChordColor = inactivebordercolor;
            BgColor = bgcolor;
            Grad0Color = grad0color;
            Grad1Color = grad1color;
            Rhythm0Color = rhythm0color;
            Rhythm1Color = rhythm1color;
            ActiveInstrumentalColor = activeinstrumentalcolor;
            InactiveChordColor = inactivechordcolor;
            HighlightChordColor = highlightchordcolor;
        }


    }

    

    public class Themes : IEnumerable
    {


        #region properties
        /// <summary>
        /// Usual file
        /// </summary>
        private string file = string.Empty;
        public string File
        {
            get
            {
                if (file == "")
                    file = "Themes.xml";
                return file;
            }
            set
            {
                file = value;
            }
        }
             
        #endregion


        private Theme Default = new Theme("Default")
        {            
            ActiveColor = Color.FromArgb(255, 0, 172, 255),
            HighlightColor = Color.FromArgb(255, 255, 255, 0),
            InactiveColor = Color.FromArgb(255, 255, 255),
            ActiveBorderColor = Color.FromArgb(255, 0, 0, 0),
            InactiveBorderColor = Color.FromArgb(255, 128, 0, 255),
            BgColor = Color.FromArgb(255, 252, 169, 3),
            Grad0Color = Color.FromArgb(255, 72, 209, 204),
            Grad1Color = Color.FromArgb(255, 173, 255, 47),
            Rhythm0Color = Color.FromArgb(255, 0, 0, 0),
            Rhythm1Color = Color.FromArgb(255, 30, 144, 255),
            ActiveInstrumentalColor = Color.FromArgb(255, 128, 128, 128),
            InactiveChordColor = Color.FromArgb(255, 128, 0, 255),
            HighlightChordColor = Color.FromArgb(255, 255, 0, 0),

        };       
        
        
        public List<Theme> AvailableThemes;

        public Themes()
        {
            AvailableThemes = new List<Theme>();
            AvailableThemes.Add(Default);

        }


        public int Count
        {
            get { return AvailableThemes.Count; }
        }


        public bool ThemeExists(string themename)
        {
            Theme thtarget = AvailableThemes.Where(z => z.Name.Equals(themename)).FirstOrDefault();
            return thtarget == null ? false : true;
        }

        public Theme getTheme(string themename)
        {
            Theme thtarget = AvailableThemes.Where(z => z.Name == themename).FirstOrDefault();
            return thtarget;
        }


        public int SelectedIndex(Theme curItem)
        {
            return AvailableThemes.IndexOf(curItem);
        }


        public void Add(Theme theme)
        {
            if (theme.Name == "Default")
            {
                MessageBox.Show("Default theme already exists", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AvailableThemes.Add(theme);
            // Sort list with theme name
            AvailableThemes.Sort();
            
        }

        public void Remove(Theme theme)
        {
            if (theme.Name == "Default")
            {
                MessageBox.Show("Default theme cannot be removed", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AvailableThemes.Remove(theme);
        }

        
        public void Clear()
        {
            AvailableThemes.Clear();
            AvailableThemes.Add(Default);

        }

     

        // Implementation for the GetEnumerator method.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public LineEnum GetEnumerator()
        {
            return new LineEnum(AvailableThemes);
        }

    }

    public class LineEnum : IEnumerator
    {
        public List<Theme> AvailableThemes;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public LineEnum(List<Theme> list)
        {
            AvailableThemes = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < AvailableThemes.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public Theme Current
        {
            get
            {
                try
                {
                    return AvailableThemes[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }

    


}
