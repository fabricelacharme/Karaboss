using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFNViewer.SongIni
{

    /// <summary>
    /// Representation of the trajectories the text or image can take.
    /// </summary>
    public enum TrajectoryKind
    {
        PlainBottomToTop,
        PlainTopToBottom,
        BottomLeftToTopRight,
        BottomRightToTopLeft,
        TopRightToBottomLeft,
        TopLeftToBottomRight,
        Still,
        StarWars,
        MadCircles,
        BackToFront1,
        BackToFront2
    }

    public class Trajectory
    {
        public TrajectoryKind Kind { get; set; }
        public double TotalTime { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }

        public Trajectory(TrajectoryKind kind, double totalTime, double width, double height, double depth)
        {
            this.Kind = kind;
            this.TotalTime = totalTime;
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
        }

        // impl std::default::Default for Trajectory
        public static Trajectory Default()
        {
            return new Trajectory(TrajectoryKind.PlainBottomToTop, 1.0, 1.0, 1.0, 1.0);
        }

        private string ConcatenateValues(double total_time, double width, double height, double depth)
        {
            string value = "";
            value += '*';
            value += total_time.ToString();

            value += '*';
            value += width.ToString();

            value += '*';
            value += height.ToString();

            value += '*';
            value += depth.ToString();

            return value;
        }

        // impl From<&str> for Trajectory
        public static Trajectory FromString(string s)
        {
            string[] parts = s.Split('*');
            string key = parts[0];
            string[] value = parts;

            // dbg!(&key, &value);
            Console.WriteLine($"[key] = {key}, [value] = {string.Join(", ", value)}");

            double total_time = 1.5;
            double width = 1.0;
            double height = 1.0;
            double depth = 1.0;

            if (!string.IsNullOrEmpty(key))
            {
                total_time = double.Parse(value[1]);
                width = double.Parse(value[2]);
                height = double.Parse(value[3]);
                depth = double.Parse(value[4]);
            }

            // dbg!(&value);
            Console.WriteLine($"[value] = {string.Join(", ", value)}");

            switch (key)
            {
                case "PlainBottomToTop":
                    return new Trajectory(TrajectoryKind.PlainBottomToTop, total_time, width, height, depth);
                case "PlainTopToBottom":
                    return new Trajectory(TrajectoryKind.PlainTopToBottom, total_time, width, height, depth);
                case "BottomLeftToTopRight":
                    return new Trajectory(TrajectoryKind.BottomLeftToTopRight, total_time, width, height, depth);
                case "BottomRightToTopLeft":
                    return new Trajectory(TrajectoryKind.BottomRightToTopLeft, total_time, width, height, depth);
                case "TopRightToBottomLeft":
                    return new Trajectory(TrajectoryKind.TopRightToBottomLeft, total_time, width, height, depth);
                case "TopLeftToBottomRight":
                    return new Trajectory(TrajectoryKind.TopLeftToBottomRight, total_time, width, height, depth);
                case "Still":
                    return new Trajectory(TrajectoryKind.Still, total_time, width, height, depth);
                case "StarWars":
                    return new Trajectory(TrajectoryKind.StarWars, total_time, width, height, depth);
                case "MadCircles":
                    return new Trajectory(TrajectoryKind.MadCircles, total_time, width, height, depth);
                case "BackToFront1":
                    return new Trajectory(TrajectoryKind.BackToFront1, total_time, width, height, depth);
                case "BackToFront2":
                    return new Trajectory(TrajectoryKind.BackToFront2, total_time, width, height, depth);
                default:
                    return new Trajectory(TrajectoryKind.PlainBottomToTop, total_time, width, height, depth);
            }
        }

        // impl ToString for Trajectory
        public override string ToString()
        {
            switch (this.Kind)
            {
                case TrajectoryKind.PlainBottomToTop:
                    {
                        string value = "PlainBottomToTop";
                        value += this.ConcatenateValues(this.TotalTime, this.Width, this.Height, this.Depth);
                        return value;
                    }
                case TrajectoryKind.BottomLeftToTopRight:
                    {
                        string value = "BottomLeftToTopRight";
                        value += this.ConcatenateValues(this.TotalTime, this.Width, this.Height, this.Depth);
                        return value;
                    }
                case TrajectoryKind.BottomRightToTopLeft:
                    {
                        string value = "BottomRightToTopLeft";
                        value += this.ConcatenateValues(this.TotalTime, this.Width, this.Height, this.Depth);
                        return value;
                    }
                case TrajectoryKind.TopRightToBottomLeft:
                    {
                        string value = "TopRightToBottomLeft";
                        value += this.ConcatenateValues(this.TotalTime, this.Width, this.Height, this.Depth);
                        return value;
                    }
                case TrajectoryKind.TopLeftToBottomRight:
                    {
                        string value = "TopLeftToBottomRight";
                        value += this.ConcatenateValues(this.TotalTime, this.Width, this.Height, this.Depth);
                        return value;
                    }
                case TrajectoryKind.Still:
                    {
                        string value = "Still";
                        value += this.ConcatenateValues(this.TotalTime, this.Width, this.Height, this.Depth);
                        return value;
                    }
                case TrajectoryKind.StarWars:
                    {
                        string value = "StarWars";
                        value += this.ConcatenateValues(this.TotalTime, this.Width, this.Height, this.Depth);
                        return value;
                    }
                case TrajectoryKind.MadCircles:
                    {
                        string value = "MadCircles";
                        value += this.ConcatenateValues(this.TotalTime, this.Width, this.Height, this.Depth);
                        // Have a file?
                        // Upload
                        return value;
                    }
                case TrajectoryKind.BackToFront1:
                    {
                        string value = "BackToFront1";
                        value += this.ConcatenateValues(this.TotalTime, this.Width, this.Height, this.Depth);
                        return value;
                    }
                case TrajectoryKind.BackToFront2:
                    {
                        string value = "BackToFront2";
                        value += this.ConcatenateValues(this.TotalTime, this.Width, this.Height, this.Depth);
                        return value;
                    }
                default:
                    {
                        return "PlainBottomToTop*1*1*1*1";
                    }
            }
        }
    }

}
