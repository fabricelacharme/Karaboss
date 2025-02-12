﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;

namespace Karaboss.MP3
{
    public class MusicTag
    {
        #region Variables

        internal string m_strArtist = "";
        internal string m_strAlbum = "";
        internal string m_strGenre = "";
        internal string m_strTitle = "";
        internal string m_strComment = "";
        internal int m_iYear = 0;
        internal int m_iDuration = 0;
        internal int m_iTrack = 0;
        internal int m_iNumTrack = 0;
        internal int m_TimesPlayed = 0;
        internal int m_iRating = 0;
        internal byte[] m_CoverArtImageBytes = null;
        internal string m_AlbumArtist = string.Empty;
        internal string m_Composer = string.Empty;
        internal string m_Conductor = string.Empty;
        internal string m_FileType = string.Empty;
        internal int m_BitRate = 0;
        internal string m_FileName = string.Empty;
        internal string m_Lyrics = string.Empty;
        internal int m_iDiscId = 0;
        internal int m_iNumDisc = 0;
        internal bool m_hasAlbumArtist = false;
        internal DateTime m_dateTimeModified = DateTime.MinValue;
        internal DateTime m_dateTimePlayed = DateTime.MinValue;
        internal string m_Codec = string.Empty;
        internal string m_BitRateMode = string.Empty;
        internal int m_BPM = 0;
        internal int m_Channels = 0;
        internal int m_SampleRate = 0;
        internal string m_ReplayGainTrack = "";
        internal string m_ReplayGainTrackPeak = "";
        internal string m_ReplayGainAlbum = "";
        internal string m_ReplayGainAlbumPeak = "";
        internal string m_imageURL = string.Empty;
        internal string m_mbArtistId;
        internal string m_mbDiscId;
        internal string m_mbReleaseArtistId;
        internal string m_mbReleaseCountry;
        internal string m_mbReleaseGroupId;
        internal string m_mbReleaseId;
        internal string m_mbReleaseStatus;
        internal string m_mbReleaseType;
        internal string m_mbTrackId;

        #endregion

        #region ctor

        /// <summary>
        /// empty constructor
        /// </summary>
        public MusicTag() { }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="tag"></param>
        public MusicTag(MusicTag tag)
        {
            if (tag == null) return;
            Artist = tag.Artist;
            Album = tag.Album;
            Genre = tag.Genre;
            Title = tag.Title;
            Comment = tag.Comment;
            Year = tag.Year;
            Duration = tag.Duration;
            Track = tag.Track;
            TimesPlayed = tag.m_TimesPlayed;
            Rating = tag.Rating;
            BitRate = tag.BitRate;
            Composer = tag.Composer;
            CoverArtImageBytes = tag.CoverArtImageBytes;
            AlbumArtist = tag.AlbumArtist;
            Lyrics = tag.Lyrics;
            Comment = tag.Comment;
            ReplayGainTrack = tag.ReplayGainTrack;
            ReplayGainTrackPeak = tag.ReplayGainTrackPeak;
            ReplayGainAlbum = tag.ReplayGainAlbum;
            ReplayGainAlbumPeak = tag.ReplayGainAlbumPeak;
            MusicBrainzArtistId = tag.MusicBrainzArtistId;
            MusicBrainzDiscId = tag.MusicBrainzDiscId;
            MusicBrainzReleaseArtistId = tag.MusicBrainzReleaseArtistId;
            MusicBrainzReleaseCountry = tag.MusicBrainzReleaseCountry;
            MusicBrainzReleaseGroupId = tag.MusicBrainzReleaseGroupId;
            MusicBrainzReleaseId = tag.MusicBrainzReleaseId;
            MusicBrainzReleaseStatus = tag.MusicBrainzReleaseStatus;
            MusicBrainzReleaseType = tag.MusicBrainzReleaseType;
            MusicBrainzTrackId = tag.MusicBrainzTrackId;

            DateTimePlayed = tag.DateTimePlayed;
            DateTimeModified = tag.DateTimeModified;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method to clear the current item
        /// </summary>
        public void Clear()
        {
            m_strArtist = "";
            m_strAlbum = "";
            m_strGenre = "";
            m_strTitle = "";
            m_strComment = "";
            m_FileType = "";
            m_iYear = 0;
            m_iDuration = 0;
            m_iTrack = 0;
            m_iNumTrack = 0;
            m_TimesPlayed = 0;
            m_iRating = 0;
            m_BitRate = 0;
            m_Composer = "";
            m_Conductor = "";
            m_AlbumArtist = "";
            m_Lyrics = "";
            m_iDiscId = 0;
            m_iNumDisc = 0;
            m_hasAlbumArtist = false;
            m_Codec = "";
            m_BitRateMode = "";
            m_BPM = 0;
            m_Channels = 0;
            m_SampleRate = 0;
            m_dateTimeModified = DateTime.MinValue;
            m_dateTimePlayed = DateTime.MinValue;
            m_ReplayGainTrack = "";
            m_ReplayGainTrackPeak = "";
            m_ReplayGainAlbum = "";
            m_ReplayGainAlbumPeak = "";
            m_imageURL = "";
            m_mbArtistId = "";
            m_mbDiscId = "";
            m_mbReleaseArtistId = "";
            m_mbReleaseCountry = "";
            m_mbReleaseGroupId = "";
            m_mbReleaseId = "";
            m_mbReleaseStatus = "";
            m_mbReleaseType = "";
            m_mbTrackId = "";
        }

        public bool IsMissingData
        {
            get
            {
                return Artist.Length == 0
                       || Album.Length == 0
                       || Title.Length == 0
                       || Artist.Length == 0
                       || Genre.Length == 0
                       || Track == 0
                       || Duration == 0;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Property to get/set the comment field of the music file
        /// </summary>
        public string Comment
        {
            get { return m_strComment; }
            set
            {
                if (value == null) return;
                m_strComment = value.Trim();
            }
        }

        /// <summary>
        /// Property to get/set the Title field of the music file
        /// </summary>
        public string Title
        {
            get { return m_strTitle; }
            set
            {
                if (value == null) return;
                m_strTitle = value.Trim();
            }
        }

        /// <summary>
        /// Property to get/set the Artist field of the music file
        /// </summary>
        public string Artist
        {
            get { return m_strArtist; }
            set
            {
                if (value == null) return;
                m_strArtist = value.Trim();
            }
        }

        /// <summary>
        /// Property to get/set the comment Album name of the music file
        /// </summary>
        public string Album
        {
            get { return m_strAlbum; }
            set
            {
                if (value == null) return;
                m_strAlbum = value.Trim();
            }
        }

        /// <summary>
        /// Property to get/set the Genre field of the music file
        /// </summary>
        public string Genre
        {
            get { return m_strGenre; }
            set
            {
                if (value == null) return;
                m_strGenre = value.Trim();
            }
        }

        /// <summary>
        /// Property to get/set the Year field of the music file
        /// </summary>
        public int Year
        {
            get { return m_iYear; }
            set { m_iYear = value; }
        }

        /// <summary>
        /// Property to get/set the duration in seconds of the music file
        /// </summary>
        public int Duration
        {
            get { return m_iDuration; }
            set { m_iDuration = value; }
        }

        /// <summary>
        /// Property to get/set the Track number field of the music file
        /// </summary>
        public int Track
        {
            get { return m_iTrack; }
            set { m_iTrack = value; }
        }

        /// <summary>
        /// Property to get/set the Total Track number field of the music file
        /// </summary>
        public int TrackTotal
        {
            get { return m_iNumTrack; }
            set { m_iNumTrack = value; }
        }

        /// <summary>
        /// Property to get/set the Disc Id field of the music file
        /// </summary>
        public int DiscID
        {
            get { return m_iDiscId; }
            set { m_iDiscId = value; }
        }

        /// <summary>
        /// Property to get/set the Total Disc number field of the music file
        /// </summary>
        public int DiscTotal
        {
            get { return m_iNumDisc; }
            set { m_iNumDisc = value; }
        }

        /// <summary>
        /// Property to get/set the Track number field of the music file
        /// </summary>
        public int Rating
        {
            get { return m_iRating; }
            set { m_iRating = value; }
        }

        /// <summary>
        /// Property to get/set the number of times this file has been played
        /// </summary>
        public int TimesPlayed
        {
            get { return m_TimesPlayed; }
            set { m_TimesPlayed = value; }
        }

        public string FileType
        {
            get { return m_FileType; }
            set { m_FileType = value; }
        }

        public int BitRate
        {
            get { return m_BitRate; }
            set { m_BitRate = value; }
        }

        public string AlbumArtist
        {
            get { return m_AlbumArtist; }
            set { m_AlbumArtist = value; }
        }

        public bool HasAlbumArtist
        {
            get { return m_hasAlbumArtist; }
            set { m_hasAlbumArtist = value; }
        }

        public string Composer
        {
            get { return m_Composer; }
            set { m_Composer = value; }
        }

        public string Conductor
        {
            get { return m_Conductor; }
            set { m_Conductor = value; }
        }

        public string FileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

        public string Lyrics
        {
            get { return m_Lyrics; }
            set { m_Lyrics = value; }
        }

        public string Codec
        {
            get { return m_Codec; }
            set { m_Codec = value; }
        }

        public string BitRateMode
        {
            get { return m_BitRateMode; }
            set { m_BitRateMode = value; }
        }

        public int BPM
        {
            get { return m_BPM; }
            set { m_BPM = value; }
        }

        public int Channels
        {
            get { return m_Channels; }
            set { m_Channels = value; }
        }

        public int SampleRate
        {
            get { return m_SampleRate; }
            set { m_SampleRate = value; }
        }

        public byte[] CoverArtImageBytes
        {
            get { return m_CoverArtImageBytes; }
            set { m_CoverArtImageBytes = value; }
        }

        public DateTime DateTimeModified
        {
            get { return m_dateTimeModified; }
            set { m_dateTimeModified = value; }
        }

        /// <summary>
        /// Last UTC time the song was played
        /// </summary>
        public DateTime DateTimePlayed
        {
            get { return m_dateTimePlayed; }
            set { m_dateTimePlayed = value; }
        }

        /*
        public string CoverArtFile
        {
            get { return Utils.GetImageFile(m_CoverArtImageBytes, m_FileName); }
        }
        */
        public string ReplayGainTrack
        {
            get { return m_ReplayGainTrack; }
            set { m_ReplayGainTrack = value; }
        }

        public string ReplayGainTrackPeak
        {
            get { return m_ReplayGainTrackPeak; }
            set { m_ReplayGainTrackPeak = value; }
        }

        public string ReplayGainAlbum
        {
            get { return m_ReplayGainAlbum; }
            set { m_ReplayGainAlbum = value; }
        }

        public string ReplayGainAlbumPeak
        {
            get { return m_ReplayGainAlbumPeak; }
            set { m_ReplayGainAlbumPeak = value; }
        }

        public string ImageURL
        {
            get { return m_imageURL; }
            set { m_imageURL = value; }
        }

        /// <summary>
        /// MusicBrainzArtistId
        /// ID3: TXXX
        /// </summary>
        public string MusicBrainzArtistId
        {
            get { return m_mbArtistId; }
            set { m_mbArtistId = value ?? ""; }
        }

        /// <summary>
        /// MusicBrainzDiscId
        /// ID3: TXXX
        /// </summary>
        public string MusicBrainzDiscId
        {
            get { return m_mbDiscId; }
            set { m_mbDiscId = value ?? ""; }
        }

        /// <summary>
        /// MusicBrainzReleaseArtistId
        /// ID3: TXXX
        /// </summary>
        public string MusicBrainzReleaseArtistId
        {
            get { return m_mbReleaseArtistId; }
            set { m_mbReleaseArtistId = value ?? ""; }
        }

        /// <summary>
        /// MusicBrainzReleaseCountry
        /// ID3: TXXX
        /// </summary>
        public string MusicBrainzReleaseCountry
        {
            get { return m_mbReleaseCountry; }
            set { m_mbReleaseCountry = value ?? ""; }
        }

        /// <summary>
        /// MusicBrainzReleaseGroupId
        /// ID3: TXXX
        /// </summary>
        public string MusicBrainzReleaseGroupId
        {
            get { return m_mbReleaseGroupId; }
            set { m_mbReleaseGroupId = value ?? ""; }
        }

        /// <summary>
        /// MusicBrainzReleaseId
        /// ID3: TXXX
        /// </summary>
        public string MusicBrainzReleaseId
        {
            get { return m_mbReleaseId; }
            set { m_mbReleaseId = value ?? ""; }
        }

        /// <summary>
        /// MusicBrainzReleaseStatus
        /// ID3: TXXX
        /// </summary>
        public string MusicBrainzReleaseStatus
        {
            get { return m_mbReleaseStatus; }
            set { m_mbReleaseStatus = value ?? ""; }
        }

        /// <summary>
        /// MusicBrainzReleaseType
        /// ID3: TXXX
        /// </summary>
        public string MusicBrainzReleaseType
        {
            get { return m_mbReleaseType; }
            set { m_mbReleaseType = value ?? ""; }
        }

        /// <summary>
        /// MusicBrainzTrackId
        /// ID3: TXXX
        /// </summary>
        public string MusicBrainzTrackId
        {
            get { return m_mbTrackId; }
            set { m_mbTrackId = value ?? ""; }
        }

        #endregion
    }
}
