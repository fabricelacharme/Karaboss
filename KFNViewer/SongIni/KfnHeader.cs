using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFNViewer.SongIni
{
    /// <summary>
    /// Interface for types that can be converted to binary data.
    /// Equivalent to the Rust ToBinary trait.
    /// </summary>
    public interface IToBinary
    {
        byte[] ToBinary();
    }

    public class KfnHeader : IToBinary, ICloneable
    {

        /// <summary>
        /// Difficulty for men. Value between 1 to 5.
        /// </summary>
        public uint DiffMen { get; set; } = 0;

        /// <summary>
        /// Difficulty for women. Value between 1 to 5.
        /// </summary>
        public uint DiffWomen { get; set; } = 0;

        public uint Genre { get; set; } = 0;
        public uint Sftv { get; set; } = 0;
        public uint Musl { get; set; } = 0;
        public uint Anme { get; set; } = 0;
        public uint KfnType { get; set; } = 0;
        public string Flid { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Album { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string Composer { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string Copyright { get; set; } = string.Empty;
        public string SourceFile { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string Trak { get; set; } = string.Empty;
        public uint Rght { get; set; } = 0;
        public uint Prov { get; set; } = 0;
        public string Karafunizer { get; set; } = string.Empty;
        public string Idus { get; set; } = string.Empty;


        // Implementation of the Rust Default trait is handled by the initializers above.
        public KfnHeader() { }

        // Implementation of the Rust Debug trait (useful for debugging in C#)
        public override string ToString()
        {
            return $"KfnHeader {{ DiffMen: {DiffMen}, DiffWomen: {DiffWomen}, Genre: {Genre}, Sftv: {Sftv}, Musl: {Musl}, " +
                   $"Anme: {Anme}, KfnType: {KfnType}, Flid: \"{Flid}\", Language: \"{Language}\", Album: \"{Album}\", " +
                   $"Title: \"{Title}\", Artist: \"{Artist}\", Composer: \"{Composer}\", Comment: \"{Comment}\", " +
                   $"Copyright: \"{Copyright}\", SourceFile: \"{SourceFile}\", Year: \"{Year}\", Trak: \"{Trak}\", " +
                   $"Rght: {Rght}, Prov: {Prov}, Karafunizer: \"{Karafunizer}\", Idus: \"{Idus}\" }}";
        }

        // Implementation of the Rust Clone trait
        public object Clone()
        {
            return new KfnHeader
            {
                DiffMen = this.DiffMen,
                DiffWomen = this.DiffWomen,
                Genre = this.Genre,
                Sftv = this.Sftv,
                Musl = this.Musl,
                Anme = this.Anme,
                KfnType = this.KfnType,
                Flid = this.Flid,
                Language = this.Language,
                Album = this.Album,
                Title = this.Title,
                Artist = this.Artist,
                Composer = this.Composer,
                Comment = this.Comment,
                Copyright = this.Copyright,
                SourceFile = this.SourceFile,
                Year = this.Year,
                Trak = this.Trak,
                Rght = this.Rght,
                Prov = this.Prov,
                Karafunizer = this.Karafunizer,
                Idus = this.Idus
            };
        }

        /// <summary>
        /// Converts the header to its binary representation.
        /// </summary>
        public byte[] ToBinary()
        {
            // create the data vector
            List<byte> data = new List<byte>();

            // To learn more about the headers, please read the documentation bundled.

            // beginning of header
            data.AddRange(Encoding.ASCII.GetBytes("KFNB"));

            // men difficulty
            data.AddRange(Encoding.ASCII.GetBytes("DIFM"));
            data.Add(1);
            data.AddRange(U32ToU8Arr(this.DiffMen));

            // woman difficulty 
            data.AddRange(Encoding.ASCII.GetBytes("DIFW"));
            data.Add(1);
            data.AddRange(U32ToU8Arr(this.DiffWomen));

            // genre
            data.AddRange(Encoding.ASCII.GetBytes("GNRE"));
            data.Add(1);
            data.AddRange(U32ToU8Arr(this.Genre));

            // sftv
            data.AddRange(Encoding.ASCII.GetBytes("SFTV"));
            data.Add(1);
            data.AddRange(U32ToU8Arr(this.Sftv));

            // musl
            data.AddRange(Encoding.ASCII.GetBytes("MUSL"));
            data.Add(1);
            data.AddRange(U32ToU8Arr(this.Musl));

            // anme
            data.AddRange(Encoding.ASCII.GetBytes("ANME"));
            data.Add(1);
            data.AddRange(U32ToU8Arr(this.Anme));

            // type
            data.AddRange(Encoding.ASCII.GetBytes("TYPE"));
            data.Add(1);
            data.AddRange(U32ToU8Arr(this.KfnType));

            // flid - encryption key
            data.AddRange(Encoding.ASCII.GetBytes("FLID"));
            data.Add(2);
            data.AddRange(U32ToU8Arr((uint)Encoding.UTF8.GetByteCount(this.Flid)));
            data.AddRange(Encoding.UTF8.GetBytes(this.Flid));

            // language
            data.AddRange(Encoding.ASCII.GetBytes("LANG"));
            data.Add(2);
            data.AddRange(U32ToU8Arr((uint)Encoding.UTF8.GetByteCount(this.Language)));
            data.AddRange(Encoding.UTF8.GetBytes(this.Language));

            // title
            data.AddRange(Encoding.ASCII.GetBytes("TITL"));
            data.Add(2);
            data.AddRange(U32ToU8Arr((uint)Encoding.UTF8.GetByteCount(this.Title)));
            data.AddRange(Encoding.UTF8.GetBytes(this.Title));

            // artist
            data.AddRange(Encoding.ASCII.GetBytes("ARTS"));
            data.Add(2);
            data.AddRange(U32ToU8Arr((uint)Encoding.UTF8.GetByteCount(this.Artist)));
            data.AddRange(Encoding.UTF8.GetBytes(this.Artist));

            // album
            data.AddRange(Encoding.ASCII.GetBytes("ALBM"));
            data.Add(2);
            data.AddRange(U32ToU8Arr((uint)Encoding.UTF8.GetByteCount(this.Album)));
            data.AddRange(Encoding.UTF8.GetBytes(this.Album));

            // composer
            data.AddRange(Encoding.ASCII.GetBytes("COMP"));
            data.Add(2);
            data.AddRange(U32ToU8Arr((uint)Encoding.UTF8.GetByteCount(this.Composer)));
            data.AddRange(Encoding.UTF8.GetBytes(this.Composer));

            // comment
            data.AddRange(Encoding.ASCII.GetBytes("COMM"));
            data.Add(2);
            data.AddRange(U32ToU8Arr((uint)Encoding.UTF8.GetByteCount(this.Comment)));
            data.AddRange(Encoding.UTF8.GetBytes(this.Comment));

            // copyright
            data.AddRange(Encoding.ASCII.GetBytes("COPY"));
            data.Add(2);
            data.AddRange(U32ToU8Arr((uint)Encoding.UTF8.GetByteCount(this.Copyright)));
            data.AddRange(Encoding.UTF8.GetBytes(this.Copyright));

            // source
            data.AddRange(Encoding.ASCII.GetBytes("SORC"));
            data.Add(2);
            data.AddRange(U32ToU8Arr((uint)Encoding.UTF8.GetByteCount(this.SourceFile)));
            data.AddRange(Encoding.UTF8.GetBytes(this.SourceFile));

            // year
            data.AddRange(Encoding.ASCII.GetBytes("YEAR"));
            data.Add(2);
            data.AddRange(U32ToU8Arr((uint)Encoding.UTF8.GetByteCount(this.Year)));
            data.AddRange(Encoding.UTF8.GetBytes(this.Year));

            // track / trak
            data.AddRange(Encoding.ASCII.GetBytes("TRAK"));
            data.Add(2);
            data.AddRange(U32ToU8Arr((uint)Encoding.UTF8.GetByteCount(this.Trak)));
            data.AddRange(Encoding.UTF8.GetBytes(this.Trak));

            // karafunizer
            data.AddRange(Encoding.ASCII.GetBytes("KFNZ"));
            data.Add(2);
            data.AddRange(U32ToU8Arr((uint)Encoding.UTF8.GetByteCount(this.Karafunizer)));
            data.AddRange(Encoding.UTF8.GetBytes(this.Karafunizer));

            // rght / right
            data.AddRange(Encoding.ASCII.GetBytes("RGHT"));
            data.Add(1);
            data.AddRange(U32ToU8Arr(this.Rght));

            // prov / 
            data.AddRange(Encoding.ASCII.GetBytes("PROV"));
            data.Add(1);
            data.AddRange(U32ToU8Arr(this.Prov));

            // IDUS 
            data.AddRange(Encoding.ASCII.GetBytes("IDUS"));
            data.Add(2);
            data.AddRange(U32ToU8Arr((uint)Encoding.UTF8.GetByteCount(this.Idus)));
            data.AddRange(Encoding.UTF8.GetBytes(this.Idus));

            // END HEADER
            data.AddRange(Encoding.ASCII.GetBytes("ENDH"));
            data.Add(1);
            data.AddRange(new byte[] { 255, 255, 255, 255 });

            return data.ToArray();
        }

        /// <summary>
        /// Helper function equivalent to crate::helpers::u32_to_u8_arr.
        /// Converts a uint to a 4-byte array (Little Endian).
        /// </summary>
        private static byte[] U32ToU8Arr(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }


    }
}
