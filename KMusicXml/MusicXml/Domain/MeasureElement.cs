﻿namespace MusicXml.Domain
{
	public class MeasureElement
	{
		public MeasureElementType Type { get; set; }
		public object Element { get; set; }
	}

	public enum MeasureElementType
	{
		Note,
		Chord,
		Backup,
		Forward,
		Barline,
		Ending,
		TempoChange,
		Time,
		Coda
	}
}
