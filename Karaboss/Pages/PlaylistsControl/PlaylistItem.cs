#region License

/* Copyright (c) 2018 Fabrice Lacharme
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Fabrice Lacharme
 * Email: fabrice.lacharme@gmail.com
 */

#endregion
using System;
using System.Runtime.Serialization;

namespace Karaboss
{
    /*
    * PLAYLISTS MANAGEMENT
    * A playlist is a collection of playlistItems
    * 
    * DataContract => reference to System.Runtime.Serialization.dll
    * 
    */

   
    [DataContract]
    public class PlaylistItem
    {
        //[DataMember]
        //public bool Selected { get; set; }
        
        [DataMember]
        public string Artist { get; set; }

        [DataMember]
        public string Song { get; set; }

        [DataMember]
        public String File { get; set; }

        [DataMember]
        public string Album { get; set; }
        
        [DataMember]
        public string Length { get; set; }

        [DataMember]
        public int Notation { get; set; }

        [DataMember]
        public string DirSlideShow { get; set; }

        [DataMember]
        public bool MelodyMute { get; set; }

        [DataMember]
        public string KaraokeSinger { get; set; }
            
    }



}
