namespace SongLyrics.Api.Classes
{
    public abstract class Contribuitor
    {
        public string Name { get; set; }
        public Contribuitor(string name)
        {
            Name = name;
        }
    }
}
