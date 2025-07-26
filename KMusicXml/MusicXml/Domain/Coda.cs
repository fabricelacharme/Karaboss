namespace MusicXml.Domain
{

    public enum CodaTypes
    {
        Start,
        ToCoda,
        DSCoda
    }
    public class Coda
    {
        /*
         * 3 types de Coda:
         *             
            <direction placement="above">
                <direction-type>
                    <words relative-y="10" font-family="Leland Text"></words>
                </direction-type>
            </direction>

            <direction placement="above">
                <direction-type>
                    <words relative-y="10">To Coda</words>
                </direction-type>
            </direction>

            <direction placement="above">
                <direction-type>
                    <words relative-y="10">D.S. al Coda</words>
                </direction-type>
            </direction>
         */

       

        internal Coda()
        {
            
        }

        public int Measure { get; internal set; } = 0;
        public CodaTypes Type { get; internal set; } = CodaTypes.Start;
    }
    
}
