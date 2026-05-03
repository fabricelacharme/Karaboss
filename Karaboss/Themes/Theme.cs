using kar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Karaboss.Themes
{
    [DataContract]
    public class ThemeItem
    {

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ActiveColor { get; set; }

        [DataMember]
        public string HighlightColor { get; set; }

        [DataMember]
        public string InactiveColor { get; set; }

        /*
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
        */

    }



    public class ThemesList
    {

        public string Name { get; set; }


        private ThemeItem Default = new ThemeItem()
        {
            Name = "Default",
            ActiveColor = "#00ACFF",
            HighlightColor = "#FFFF00",
            InactiveColor = "#FFFFFF",

        };

        public ThemesList()
        {            
            
        }
        

        public void AddDefaultTheme()
        {
            if (!ItemExists("Default"))
                this._themes.Add(Default);
        }


        public int Count()
        {
            return this._themes.Count;
        }


        public bool Add(ThemeItem item)
        {
            if (ItemExists(item.Name) == false)
            {
                this._themes.Add(item);
                return true;
            }
            else
            {
                string tx = "<" + item.Name + "> already exists";
                MessageBox.Show(tx, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }            
        }
       

        public bool Add(string name, string activecolor, string highlightcolor, string inactivecolor)
        {
            if (ItemExists(name) == false)
            {
                ThemeItem theme = new ThemeItem()
                {
                    Name = name,
                    ActiveColor=activecolor,
                    HighlightColor=highlightcolor,
                    InactiveColor=inactivecolor
                };                                
                this._themes.Add(theme);
                return true;
            }
            else
            {
                string tx = "<" + name + "> already exists";
                MessageBox.Show(tx, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }

        public bool Remove(string name)
        {
            if (name == "Default")
            {
                string tx = "The theme <" + name + "> cannot be removed";
                MessageBox.Show(tx, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;

            }

            if (ItemExists(name) == false)
            {
                string tx = "The theme <" + name + "> does not exists";
                MessageBox.Show(tx, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }


            ThemeItem theme = GetThemeByName(name);

            this._themes.Remove(theme);
            return true;

        }


        public bool ItemExists(string Name)
        {
            ThemeItem thtarget = _themes.Where(z => z.Name == Name).FirstOrDefault();
            return thtarget == null ? false : true;
        }

        private ObservableCollection<ThemeItem> _themes = new ObservableCollection<ThemeItem>();
        public ObservableCollection<ThemeItem> Themes
        {
            get
            {
                if (_themes == null)
                {
                    _themes = new ObservableCollection<ThemeItem>();
                }
                return _themes;
            }
            set
            {
                if (_themes != value)
                {
                    _themes = value;
                }
            }
        }


        public ThemeItem GetThemeByName(string Name)
        {
            ThemeItem thtarget = Themes.Where(z => z.Name == Name).FirstOrDefault();
            return thtarget;
        }


        /*
        #region properties
      
        private static string file = string.Empty;
        public static string File
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


        private static Theme Default = new Theme("Default")
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
        
        
        public static List<Theme> AvailableThemes;


        public static List<Theme> Load()
        {
            List<Theme> list = new List<Theme>();

            return list;
        }


        public static int Count
        {
            get { return AvailableThemes.Count; }
        }


        public static bool ThemeExists(string themename)
        {
            Theme thtarget = AvailableThemes.Where(z => z.Name.Equals(themename)).FirstOrDefault();
            return thtarget == null ? false : true;
        }

        public static Theme getTheme(string themename)
        {
            Theme thtarget = AvailableThemes.Where(z => z.Name == themename).FirstOrDefault();
            return thtarget;
        }


        public static int SelectedIndex(Theme curItem)
        {
            return AvailableThemes.IndexOf(curItem);
        }


        public static void Add(Theme theme)
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

        public static void Remove(Theme theme)
        {
            if (theme.Name == "Default")
            {
                MessageBox.Show("Default theme cannot be removed", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AvailableThemes.Remove(theme);
        }

        
        public static void Clear()
        {
            AvailableThemes.Clear();
            AvailableThemes.Add(Default);

        }
        */

    }

    public class ThemesListEnum : IEnumerator
    {
        public ObservableCollection<ThemeItem> _themes;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public ThemesListEnum(ObservableCollection<ThemeItem> list)
        {
            _themes = list;
        }

        public bool MoveNext()
        {
            if (_themes == null)
                return true;

            position++;
            return (position < _themes.Count);
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

        public ThemeItem Current
        {
            get
            {
                try
                {
                    return _themes[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }


    }


    public class ThemesListHelper
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


        public ThemesList Load(string fileName)
        {
            ThemesList thl;
            
            // Open file containing all playlistGroups "playlistGroup.xml"
            try
            {
                using (var fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(ThemesList));
                    if (fs.Length > 0)
                        thl = (ThemesList)xml.Deserialize(fs);
                    else
                    {
                        thl = new ThemesList();
                    }
                }               

                thl.AddDefaultTheme();
                return thl;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading themes file:\n" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                
                thl = new ThemesList();
                thl.AddDefaultTheme();
                return thl;
            }
        }

        public bool Save(string fileName, ThemesList thl)
        {
            try
            {
                // Remove Default from list
                //thl.Remove("Default");
                
                XmlSerializer serializer = new XmlSerializer(typeof(ThemesList));
                TextWriter textWriter = new StreamWriter(@fileName);
                serializer.Serialize(textWriter, thl);
                textWriter.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while saving themes file:\n" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }

        public bool ThemeExist(ObservableCollection<ThemeItem> tls, string name)
        {
            name = name.ToLower();
            ThemeItem thtarget = tls.Where(z => z.Name.ToLower() == name).FirstOrDefault();
            return thtarget == null ? false : true;
        }


    }


}
