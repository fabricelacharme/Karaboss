using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace KFNViewer.SongIni
{
    public class Eff
    {
        /// <summary>
        /// The ID of the Eff# layer.
        /// The background layer's ID is always 51.
        /// Every following starts from 1.
        /// </summary>
        public int id;
        public int num;
        /// <summary>
        /// Collection of the animations.
        /// </summary>
        public List<Anim> anims = new List<Anim>();
        /// <summary>
        /// initial background, if there is any = LibImage
        /// </summary>
        public string initial_lib_image;
        /// <summary>
        /// initial video, if there is any = VideoFile
        /// </summary>
        public string initial_video_file;
        /// <summary>
        /// Initial font setting
        /// </summary>
        public (string, uint) initial_font;
        /// <summary>
        /// Active color
        /// </summary>
        public string initial_active_color;
        /// <summary>
        /// Initial inactive font color
        /// </summary>
        public string initial_inactive_color;
        /// <summary>
        /// Collection of the sync timestamps in ms.
        /// </summary>
        public List<int> syncs = new List<int>();
        /// <summary>
        /// Collection of the songtext lines. Separators: '/' ' '
        /// </summary>
        public List<TextEntry> texts = new List<TextEntry>();
        /// <summary>
        /// Initial trajectory of the layer.
        /// </summary>
        public Trajectory initial_trajectory;

        public Eff(int Id, 
            int Num, 
            List<Anim> Anims, 
            string Initial_lib_image, 
            string Initial_video_file, 
            (string, uint) Initial_font, 
            string Initial_active_color,
            string Initial_inactive_color,
            List<int> Syncs,
            List<TextEntry> Texts,
            Trajectory Initial_trajectory)
        {
            num = Num;
            anims = Anims;
            initial_lib_image = Initial_lib_image;
            initial_video_file = Initial_video_file;
            initial_font = Initial_font;
            initial_active_color = Initial_active_color;
            initial_inactive_color = Initial_inactive_color;
            syncs = Syncs;
            texts = Texts;
            initial_trajectory = Initial_trajectory;
        }

    }

    /// <summary>
    /// Representation of a collection of animations executed at the same time.
    /// </summary>
    public class Anim
    {
        public int time;
        public List<AnimEntry> anim_entries = new List<AnimEntry>();

        public Anim(int Time, List<AnimEntry> AnimEntries)
        {
            time = Time;
            anim_entries = AnimEntries;
        }

        public Anim Clone()
        {
            return new Anim(time, anim_entries);
        }

    }

    public class AnimEntry
    {
        public Action action = new Action.None();
        public EffectKind effect;
        public double trans_time;
        public TransType trans_type = TransType.None;

        public AnimEntry(Action Action, EffectKind Effect, double Transtime, TransType Transtype)
        {
            action = Action;
            effect = Effect;
            trans_time = Transtime;
            trans_type = Transtype;
            
            //trans_time = default;
        }
    }

    public class TextEntry
    {
        public string display;
        public List<(int, string)> fragments = new List<(int, string)>();
        public int eff_num;

        public TextEntry(string Display, List<(int, string)> Fragments, int EffNum)
        {
            display = Display;
            fragments = Fragments;
            eff_num = EffNum;            
        }

        // Equivalent to impl Into<String> for TextEntry
        public static implicit operator string(TextEntry entry)
        {
            return entry.display;
        }

        // Equivalent to impl From<String> for TextEntry
        public static implicit operator TextEntry(string s)
        {
            // dbg!(&s)
            //Debug.WriteLine(s);
            return new TextEntry(s, new List<(int, string)>(), 1);            
        }

        public TextEntry Clone()
        {
            return new TextEntry(display, fragments, eff_num);
        }

    }

    /// <summary>
    /// Representation of an animation action.
    /// </summary>
    public abstract class Action
    {
        public class None : Action { }
        public class ChgBgImg : Action { public string Value; public ChgBgImg(string v) => Value = v; }
        public class ChgColColor : Action { public string Value; public ChgColColor(string v) => Value = v; }
        public class ChgColImageColor : Action { public string Value; public ChgColImageColor(string v) => Value = v; }
        public class ChgAlphaBlending : Action { public string Value; public ChgAlphaBlending(string v) => Value = v; }
        public class ChgFloatOffsetX : Action { public double Value; public ChgFloatOffsetX(double v) => Value = v; }
        public class ChgFloatOffsetY : Action { public double Value; public ChgFloatOffsetY(double v) => Value = v; }
        public class ChgFloatDepth : Action { public double Value; public ChgFloatDepth(double v) => Value = v; }
        public class ChgTrajectory : Action { public Trajectory Value; public ChgTrajectory(Trajectory v) => Value = v; }

        public static Action From(string s)
        {
            var colon_split = s.Split(':');
            var equal_split = s.Split('=');

            var key = colon_split[0];
            var value = equal_split.Length > 1 ? equal_split[1] : string.Empty;

            switch (key)
            {
                case "ChgBgImg": return new ChgBgImg(value);
                case "ChgColColor": return new ChgColColor(value);
                case "ChgColImageColor": return new ChgColImageColor(value);
                case "ChgAlphaBlending": return new ChgAlphaBlending(value);
                case "ChgFloatOffsetX": return new ChgFloatOffsetX(double.Parse(value));
                case "ChgFloatOffsetY": return new ChgFloatOffsetY(double.Parse(value));
                case "ChgFloatDepth": return new ChgFloatDepth(double.Parse(value));
                case "ChgTrajectory": return new ChgTrajectory(Trajectory.FromString(value));
                default: return new None();
            }
        }

        public override string ToString()
        {
            if (this is ChgBgImg chgBgImg)
            {
                return "ChgBgImg:LibImage=" + chgBgImg.Value;
            }
            return " ";
        }
    }

    /// <summary>
    /// Representation of the available visual effects.
    /// </summary>
    public enum EffectKind
    {
        None = 0,
        AlphaBlending,
        MoveRight,
        MoveLeft,
        MoveTop,
        MoveBottom,
        // TODO the rest of the effects
    }

    public static class EffectExtensions
    {
        public static EffectKind FromStr(string s)
        {
            switch (s)
            {
                case "AlphaBlending": return EffectKind.AlphaBlending;
                case "MoveRight": return EffectKind.MoveRight;
                case "MoveLeft": return EffectKind.MoveLeft;
                case "MoveTop": return EffectKind.MoveTop;
                case "MoveBottom": return EffectKind.MoveBottom;
                default: return EffectKind.None;
            }
        }
    }

    /// <summary>
    /// Representation of the various transition types.
    /// </summary>
    public enum TransType
    {
        None = 0,
        Linear,
        Smooth,
        Falling,
        FallingBouncing,
        Bend1,
        Bend3,
        Bend5,
        Bounce1,
        Bounce3,
        Bounce5,
    }

    public static class TransTypeExtensions
    {
        public static TransType FromStr(string s)
        {
            switch (s)
            {
                case "Linear": return TransType.Linear;
                case "Smooth": return TransType.Smooth;
                case "Falling": return TransType.Falling;
                case "FallingBouncing": return TransType.FallingBouncing;
                case "Bend1": return TransType.Bend1;
                case "Bend3": return TransType.Bend3;
                case "Bend5": return TransType.Bend5;
                case "Bounce1": return TransType.Bounce1;
                case "Bounce3": return TransType.Bounce3;
                case "Bounce5": return TransType.Bounce5;
                default: return TransType.None;
            }
        }
    }
}


