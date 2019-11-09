using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;

namespace Karaboss.Pages.ABCnotation
{
    public class MacroChoiceItem
    {   // Data bag for comboboxes of macros
        public String ID { get; set; }
        public String Name { get { return GetName(); } private set {; } }

        public MacroChoiceItem(String id)
        {   //====================================================================
            ID = id;
            return;
        }

        public override string ToString()
        {   //====================================================================
            return Name;
        }

        private String GetName()
        {   //--------------------------------------------------------------------
            //if (ID != null && ID != String.Empty) return (Macro.FromID(ID).Name);
            return null;
        }

    }

    [Serializable()]
    public class LotroToolbarItem
    {   // Data bag for what's in a toolbar
        public enum ItemType { UNKNOWN, Macro, Separator, MacroList, SongList }
        public String ID { get; set; }
        public ItemType Type { get; set; }
        public String[] Choices { get; set; }
        public String Name { get; set; }

        public LotroToolbarItem() { Type = ItemType.UNKNOWN; ID = String.Empty; Choices = null; }
        //public LotroToolbarItem(Macro mac) { Type = ItemType.Macro; ID = mac.ID; Choices = null; }
        public LotroToolbarItem(ItemType type) { Type = type; ID = String.Empty; Choices = null; }
        public LotroToolbarItem(ItemType type, String[] choices) { Type = type; ID = String.Empty; Choices = choices; }
    }

    [Serializable()]
    public class LotroToolbar
    {   // Describes the contents and display of a toolbar. This is the logical toolbar, not the UI object
        public enum BarDirection { Horizontal, Vertical };
        public String Name { get; set; }
        public Point Location { get; set; }
        public BarDirection Direction { get; set; }
        public Boolean Visible { get; set; }
        public Int32 OpacityPct { get; set; }
        [XmlArray()] public List<LotroToolbarItem> Items { get; set; }

        public LotroToolbar() { Name = String.Empty; Items = new List<LotroToolbarItem>(); Direction = BarDirection.Horizontal; OpacityPct = 100; }
    }

    [Serializable()]
    public class LotroToolbarList
    {   // This makes it easier to make a user property
        [XmlArray()] public List<LotroToolbar> Items { get; set; }
        public LotroToolbarList() { Items = new List<LotroToolbar>(); }
    }
}
