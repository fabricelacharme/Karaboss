#region License

/* Copyright (c) 2024 Fabrice Lacharme
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

using KMusicXml.MusicXml.Domain;
using MusicXml.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MusicXml
{
	public static class MusicXmlParser
	{
		public static Score GetScore(string filename)
		{
			return GetScore(GetXmlDocumentFromFile(filename));
		}

		public static Score GetScoreFromString(string str)
		{
			return GetScore(GetXmlDocumentFromString(str));
		}

		public static Score GetScore(Stream str)
		{
			return GetScore(GetXmlDocument(str));
		}
		
		private static Score GetScore(XmlDocument document)
		{
			var score = new Score();

			var movementTitleNode = document.SelectSingleNode("score-partwise/movement-title");
			if (movementTitleNode != null)
				score.MovementTitle = movementTitleNode.InnerText;

			score.Identification = GetIdentification(document);
			
			// part-lis is the list of tracks
			var partNodes = document.SelectNodes("score-partwise/part-list/score-part");
			
			if (partNodes != null)
			{
				foreach (XmlNode partNode in partNodes)
				{
					var part = new Part();
					score.Parts.Add(part);

					if (partNode.Attributes != null)
						part.Id = partNode.Attributes["id"].InnerText;
					
					var partNameNode = partNode.SelectSingleNode("part-name");					
					if (partNameNode != null)
						part.Name = partNameNode.InnerText;

					
					// Fab
					var PartMidiInstrumentNode = partNode.SelectSingleNode("midi-instrument");
					if (PartMidiInstrumentNode != null)
					{
                        var partMidiChannel = PartMidiInstrumentNode.SelectSingleNode("midi-channel");
						if (partMidiChannel != null)
							part.MidiChannel = Convert.ToInt32(partMidiChannel.InnerText);

                        var partMidiProgram = PartMidiInstrumentNode.SelectSingleNode("midi-program");
                        if (partMidiProgram != null)
                            part.MidiProgram = Convert.ToInt32(partMidiProgram.InnerText);

                        var partVolume = PartMidiInstrumentNode.SelectSingleNode("volume");
                        if (partVolume != null)
                            part.Volume = Convert.ToInt32(partVolume.InnerText);

                        var partPan = PartMidiInstrumentNode.SelectSingleNode("pan");
                        if (partPan != null)
                            part.Pan = Convert.ToInt32(partPan.InnerText);
                    }


					// 
                    var measuresXpath = string.Format("//part[@id='{0}']/measure", part.Id);

					var measureNodes = partNode.SelectNodes(measuresXpath);

					if (measureNodes != null)
					{
						foreach (XmlNode measureNode in measureNodes)
						{
							var measure = new Measure();

							if (measureNode.Attributes != null)
							{
								var measureWidthAttribute = measureNode.Attributes["width"];
								decimal w;
								if (measureWidthAttribute != null && decimal.TryParse(measureWidthAttribute.InnerText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,out w))
									measure.Width = w;
							}


							// <attributes>
							var attributesNode = measureNode.SelectSingleNode("attributes");

							if (attributesNode != null)
							{
								measure.Attributes = new MeasureAttributes();

								var divisionsNode = attributesNode.SelectSingleNode("divisions");
								if (divisionsNode != null)
									measure.Attributes.Divisions = Convert.ToInt32(divisionsNode.InnerText);

								
								// CLE !!!!
								var keyNode = attributesNode.SelectSingleNode("key");

								if (keyNode != null)
								{
									measure.Attributes.Key = new Key();

									var fifthsNode = keyNode.SelectSingleNode("fifths");
									if (fifthsNode != null)
										measure.Attributes.Key.Fifths = Convert.ToInt32(fifthsNode.InnerText);

									var modeNode = keyNode.SelectSingleNode("mode");
									if (modeNode != null)
										measure.Attributes.Key.Mode = modeNode.InnerText;
								}
								
								measure.Attributes.Time = GetTime(attributesNode);

								measure.Attributes.Clef = GetClef(attributesNode);
							}


                            // Division
                            var tempoNode = measureNode.SelectSingleNode("sound");
							if (tempoNode != null) 
							{
								//measure.Attributes.Time.Tempo = Convert.ToInt32(tempoNode.InnerText);
								measure.Attributes.TempoChange.Tempo = Convert.ToInt32(tempoNode.InnerText);
							}



                            var childNodes = measureNode.ChildNodes;

							foreach (XmlNode node in childNodes)
							{
								MeasureElement measureElement = null;

								if (node.Name == "note")
								{
									var newNote = GetNote(node);
									measureElement = new MeasureElement {Type = MeasureElementType.Note, Element = newNote};
								}
								else if (node.Name == "harmony")
								{
									var newChord = GetChord(node);
                                    measureElement = new MeasureElement { Type = MeasureElementType.Chord, Element = newChord };
                                }
								else if (node.Name == "backup")
								{
									measureElement = new MeasureElement {Type = MeasureElementType.Backup, Element = GetBackupElement(node)};
								}
								else if (node.Name == "forward")
								{
									measureElement = new MeasureElement {Type = MeasureElementType.Forward, Element = GetForwardElement(node)};
								}
								else if (node.Name == "direction")
								{
									// FAB : Try to catch Tempo
									var childdirectionNodes = node.ChildNodes;
									foreach (XmlNode cnode in childdirectionNodes)
									{
										if (cnode.Name == "sound")
										{
											try
											{
												string s = cnode.OuterXml;
												string b = "tempo=";
												if (s.Contains(b))
												{
													s = s.Replace("<", "");
													s = s.Replace("/>", "");
													s = s.Replace("\"", "");
													int i = s.IndexOf(b);
													s = s.Substring(i + b.Length).Trim();

													//measure.Attributes.Time.Tempo = Convert.ToInt32(s);
													measure.Attributes.TempoChange.Tempo = Convert.ToInt32(s);
												}
											}
											catch (Exception err)
											{
												Console.Write(err.Message);
											}
                                        }                                        											
                                    }
									
                                }

								if (measureElement != null)
									measure.MeasureElements.Add(measureElement);
							}
							
							part.Measures.Add(measure);
						}
					}
				}
			}

			return score;
		}

		private static Forward GetForwardElement(XmlNode node)
		{
			var forward = new Forward();

			var forwardNode = node.SelectSingleNode("duration");

			if (forwardNode != null)
			{
				forward.Duration = Convert.ToInt32(forwardNode.InnerText);
			}

			return forward;
		}

		private static Backup GetBackupElement(XmlNode node)
		{
			var backup = new Backup();

			var backupNode = node.SelectSingleNode("duration");

			if (backupNode != null)
			{
				backup.Duration = Convert.ToInt32(backupNode.InnerText);
			}

			return backup;
		}


		private static Chord GetChord(XmlNode chordNode)
		{
			var chord = new Chord();

			return chord;
		}

		private static Note GetNote(XmlNode noteNode)
		{
			var note = new Note();

			var typeNode = noteNode.SelectSingleNode("type");
			if (typeNode != null)
				note.Type = typeNode.InnerText;

			var voiceNode = noteNode.SelectSingleNode("voice");
			if (voiceNode != null)
				note.Voice = Convert.ToInt32(voiceNode.InnerText);

			var durationNode = noteNode.SelectSingleNode("duration");
			if (durationNode != null)
				note.Duration = Convert.ToInt32(durationNode.InnerText);

			var accidental = noteNode.SelectSingleNode("accidental");
			if (accidental != null)
				note.Accidental = accidental.InnerText;

			note.Lyrics = GetLyrics(noteNode);

			note.Pitch = GetPitch(noteNode);

			var staffNode = noteNode.SelectSingleNode("staff");
			if (staffNode != null)
				note.Staff = Convert.ToInt32(staffNode.InnerText);

			var chordNode = noteNode.SelectSingleNode("chord");
			if (chordNode != null)
				note.IsChordTone = true;
			
			var restNode = noteNode.SelectSingleNode("rest");
			if (restNode != null)
				note.IsRest = true;

			return note;
		}

		private static Pitch GetPitch(XmlNode noteNode)
		{
			var pitch = new Pitch();
			var pitchNode = noteNode.SelectSingleNode("pitch");
			if (pitchNode != null)
			{
				var stepNode = pitchNode.SelectSingleNode("step");
				if (stepNode != null)
					pitch.Step = stepNode.InnerText[0];

				var alterNode = pitchNode.SelectSingleNode("alter");
				if (alterNode != null)
					pitch.Alter = Convert.ToInt32(alterNode.InnerText);

				var octaveNode = pitchNode.SelectSingleNode("octave");
				if (octaveNode != null)
					pitch.Octave = Convert.ToInt32(octaveNode.InnerText);
			}
			else
			{
				return null;
			}

			return pitch;
		}


        private static List<Lyric> GetLyrics(XmlNode noteNode)
		{
            var lyric = new Lyric();
            List<Lyric> lstLyrics = new List<Lyric>();
			
			var allLyricsnodes = noteNode.ChildNodes;    //.Descendants("lyric");

			if (allLyricsnodes != null)
			{
				foreach (var node in allLyricsnodes)
				{

				}
			}

			// =============================================
			/*
            var lyricNode = noteNode.SelectSingleNode("lyric");
            if (lyricNode != null)
            {
                var syllabicNode = lyricNode.SelectSingleNode("syllabic");

                var syllabicText = string.Empty;

                if (syllabicNode != null)
                    syllabicText = syllabicNode.InnerText;

                switch (syllabicText)
                {
                    case "":
                        lyric.Syllabic = Syllabic.None;
                        break;
                    case "begin":
                        lyric.Syllabic = Syllabic.Begin;
                        break;
                    case "single":
                        lyric.Syllabic = Syllabic.Single;
                        break;
                    case "end":
                        lyric.Syllabic = Syllabic.End;
                        break;
                    case "middle":
                        lyric.Syllabic = Syllabic.Middle;
                        break;
                }

                var textNode = lyricNode.SelectSingleNode("text");
                if (textNode != null)
                    lyric.Text = textNode.InnerText;
            }
			*/
            return lstLyrics;
        }


        private static Lyric GetLyric(XmlNode noteNode)
		{
			var lyric = new Lyric();

			var lyricNode = noteNode.SelectSingleNode("lyric");
			if (lyricNode != null)
			{
				var syllabicNode = lyricNode.SelectSingleNode("syllabic");

				var syllabicText = string.Empty;

				if (syllabicNode != null)
					syllabicText = syllabicNode.InnerText;

				switch (syllabicText)
				{
					case "":
						lyric.Syllabic = Syllabic.None;
						break;
					case "begin":
						lyric.Syllabic = Syllabic.Begin;
						break;
					case "single":
						lyric.Syllabic = Syllabic.Single;
						break;
					case "end":
						lyric.Syllabic = Syllabic.End;
						break;
					case "middle":
						lyric.Syllabic = Syllabic.Middle;
						break;
				}

				var textNode = lyricNode.SelectSingleNode("text");
				if (textNode != null)
					lyric.Text = textNode.InnerText;
			}
			return lyric;
		}

		private static Clef GetClef(XmlNode attributesNode)
		{
			var clef = new Clef();

			var clefNode = attributesNode.SelectSingleNode("clef");

			if (clefNode != null)
			{
				var lineNode = clefNode.SelectSingleNode("line");
				if (lineNode != null)
					clef.Line = Convert.ToInt32(lineNode.InnerText);

				var signNode = clefNode.SelectSingleNode("sign");
				if (signNode != null)
					clef.Sign = signNode.InnerText;
			}
			return clef;
		}

		private static Time GetTime(XmlNode attributesNode)
		{
			var time = new Time();

			var timeNode = attributesNode.SelectSingleNode("time");
			if (timeNode != null)
			{
				var beatsNode = timeNode.SelectSingleNode("beats");

				if (beatsNode != null)
					time.Beats = Convert.ToInt32(beatsNode.InnerText);

				var beatTypeNode = timeNode.SelectSingleNode("beat-type");

				if (beatTypeNode != null)
					time.Mode = beatTypeNode.InnerText;

				var symbol = TimeSymbols.Normal;

				if (timeNode.Attributes != null)
				{
					var symbolAttribute = timeNode.Attributes["symbol"];

					if (symbolAttribute != null)
					{
						switch (symbolAttribute.InnerText)
						{
							case "common":
								symbol = TimeSymbols.Common;
								break;
							case "cut":
								symbol = TimeSymbols.Cut;
								break;
							case "single-number":
								symbol = TimeSymbols.SingleNumber;
								break;
						}
					}
				}

				time.Symbol = symbol;
			}
			return time;
		}

		private static Identification GetIdentification(XmlNode document)
		{
			var identificationNode = document.SelectSingleNode("score-partwise/identification");

			if (identificationNode != null)
			{
				var identification = new Identification();

				var composerNode = identificationNode.SelectSingleNode("creator[@type='composer']");
				identification.Composer = composerNode != null ? composerNode.InnerText : string.Empty;

				var rightsNode = identificationNode.SelectSingleNode("rights");
				identification.Rights = rightsNode != null ? rightsNode.InnerText : string.Empty;

				identification.Encoding = GetEncoding(identificationNode);

				return identification;
			}

			return null;
		}

		private static MusicXml.Domain.Encoding GetEncoding(XmlNode identificationNode)
		{
			var encodingNode = identificationNode.SelectSingleNode("encoding");

			var encoding = new MusicXml.Domain.Encoding();

			if (encodingNode != null)
			{
				encoding.Software = GetInnerTextOfChildTag(encodingNode, "software");

				encoding.Description = GetInnerTextOfChildTag(encodingNode, "encoding-description");

				var encodingDate = encodingNode.SelectSingleNode("encoding-date");
				if (encodingDate != null)
					encoding.EncodingDate = Convert.ToDateTime(encodingDate.InnerText);
			}

			return encoding;
		}

		private static string GetInnerTextOfChildTag(XmlNode encodingNode, string tagName)
		{
			var softwareStringBuilder = new StringBuilder();
			
			var encodingSoftwareNodes = encodingNode.SelectNodes(tagName);

			if (encodingSoftwareNodes != null)
			{
				foreach (XmlNode node in encodingSoftwareNodes)
				{
					softwareStringBuilder.AppendLine(node.InnerText);
				}
			}

			return softwareStringBuilder.ToString();
		}
		
		private static XmlDocument GetXmlDocumentFromString(string str) 
		{
			var document = new XmlDocument();
            document.XmlResolver = null;
			document.LoadXml(str);
			return document;
		}
		private static XmlDocument GetXmlDocumentFromFile(string fileName)
		{
			var document = new XmlDocument();
			document.XmlResolver = null;
			document.Load(fileName);
			return document;
		}
		private static XmlDocument GetXmlDocument(Stream stream)
		{
            var document = new XmlDocument();
            document.XmlResolver = null;
            document.Load(stream);
			return document;
		}
	}
}
