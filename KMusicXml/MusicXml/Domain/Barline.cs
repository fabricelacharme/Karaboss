namespace MusicXml.Domain
{
    public enum RepeatDirections
    {
        forward,
        backward
    }

       

    public class Barline
    {
        internal Barline() 
        {
            
        }

        public int Duration { get; set; }
        public RepeatDirections Direction { get; internal set; }
        public int Measure {  get; internal set; }  

        
        

    }
}
