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


        #region lyrics colors

        [DataMember]
        public string ActiveColor { get; set; }

        [DataMember]
        public string HighlightColor { get; set; }

        [DataMember]
        public string InactiveColor { get; set; }

        [DataMember]
        public string ActiveBorderColor { get; set; }
        
        [DataMember]
        public string InactiveBorderColor { get; set; }

        [DataMember]
        public string ActiveInstrumentalColor { get; set; }

        #endregion lyrics colors


        #region static background color

        [DataMember]
        public string BgColor { get; set; }

        #endregion static background color


        #region dynamic backgrounds

        [DataMember]
        public string Grad0Color { get; set; }

        [DataMember]
        public string Grad1Color { get; set; }

        [DataMember]
        public string Rhythm0Color { get; set; }

        [DataMember]
        public string Rhythm1Color { get; set; }

        #endregion dynamic backgrounds

        
        #region chord color

        [DataMember]
        public string InactiveChordColor { get; set; }

        [DataMember]
        public string HighlightChordColor { get; set; }

        #endregion chord color       
    }



    public class ThemesList
    {

        //public string Name { get; set; }


        private ThemeItem Default = new ThemeItem()
        {
            Name = "Default",

            ActiveColor = "#00ACFF",
            HighlightColor = "#FFFF00",
            InactiveColor = "#FFFFFF",
            ActiveBorderColor = "#010101",
            InactiveBorderColor = "#8000FF",
            
            ActiveInstrumentalColor = "#808080",

            BgColor = "#FCA903",

            Grad0Color = "#48D1CC", //Color.MediumTurquoise,
            Grad1Color = "#ADFF2F", //Color.GreenYellow,
            Rhythm0Color = "#000000", //Color.Black,
            Rhythm1Color = "#1E90FF", //Color.DodgerBlue,

            InactiveChordColor = "#FF8C00",
            HighlightChordColor = "#8B0000",

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
