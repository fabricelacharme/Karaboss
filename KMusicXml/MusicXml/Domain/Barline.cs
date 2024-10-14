namespace MusicXml.Domain
{
    public enum RepeatDirections
    {
        forward,
        backward
    }

    public enum EndingTypes
    {
        start,
        stop
    }

    public class clsEnding
    {
        public int Number { get; internal set; }
        public EndingTypes Type { get; internal set; }
    }

    public class Barline
    {
        internal Barline() 
        {
            Ending = new clsEnding();
        }

        public int Duration { get; set; }
        public RepeatDirections Direction { get; internal set; }
        public int Measure {  get; internal set; }  

        public clsEnding Ending { get; internal set; }
        

    }
}
