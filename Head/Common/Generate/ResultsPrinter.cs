using System;
using System.Collections.Generic;
using Head.Common.Interfaces.Utils;
using Head.Common.Csv;
using Common.Logging;
using Head.Common.Domain;
using Head.Common.BritishRowing;
using Head.Common.Internal.Overrides;
using System.Linq;
using Head.Common.Internal.Categories;
using Head.Common.Interfaces.Enums;
using System.Text;
using Head.Common.Utils;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Head.Common.Internal.JsonObjects;
using System.Configuration;

namespace Head.Common.Generate
{
	public class ResultsPrinter
	{
		public static void Dump(IEnumerable<ICrew> crews) 
		{
			DateTime racedate = DateTime.MinValue;
			if(!DateTime.TryParse(ConfigurationManager.AppSettings["racedate"].ToString(), out racedate))
				racedate = DateTime.MinValue;

			string raceDetails = string.Format("{0} - {1} - Provisional Results", ConfigurationManager.AppSettings["racenamelong"], racedate.ToLongDateString());
			string updated = "Updated: \t" + DateTime.Now.ToShortTimeString () + " " + DateTime.Now.ToShortDateString ();
			StringBuilder sb = new StringBuilder ();
			sb.AppendLine (updated);

			using(var fs = new FileStream(string.Format("{0} {1} Results.pdf", ConfigurationManager.AppSettings["racenamelong"], racedate.ToString("yyyy")), FileMode.Create)){
				using(Document document = new Document(PageSize.A4.Rotate())){

					// 					BaseFont bf = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
					Font font = new Font(Font.FontFamily.HELVETICA, 7f, Font.NORMAL);
					Font italic = new Font(Font.FontFamily.HELVETICA, 7f, Font.ITALIC);
					Font bold = new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD);

					// step 2:
					// we create a writer that listens to the document and directs a PDF-stream to a file            
					PdfWriter.GetInstance(document, fs);

					// step 3: we open the document
					document.Open();

					// entitle the document 
					document.Add(new Paragraph(raceDetails));
					document.AddSubject(raceDetails);

					// grab the header and seed the table 

					float[] widths = new float[] { 1f, 1f, 5f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 3f };
					PdfPTable table = new PdfPTable(widths.Count()) 
					{
						TotalWidth = 800f,
						LockedWidth = true,                    
						HorizontalAlignment = 0,
						SpacingBefore = 20f,
						SpacingAfter = 30f,
					};
					table.SetWidths(widths);

					foreach(var h in new List<string> { "Overall", "Start", "Crew", "Elapsed", "Adjustment", "Adjusted", "Category", "Category Pos", "Gender Pos", "Foreign Pos", "Notes" })
					{
						table.AddCell(new PdfPCell(new Phrase(h)) { Border = 1, HorizontalAlignment = 2, Rotation = 90 } );
					}
					sb.AppendLine (new List<string>{ "Overall", "StartNumber", "CrewName", "SequenceStart", "SequenceFinish", "Elapsed", "Adjustment", "Adjusted", "Category", "CategoryPos", "FinishType" }.Delimited ('\t'));
					foreach (var crew in crews.OrderBy(cr => cr.FinishType).ThenBy(cr => cr.Elapsed)) 
					{
						ICategory primary;
						StringBuilder extras = new StringBuilder ();
						string overallpos = string.Empty;
						string categorypos = string.Empty;
						string genderpos = string.Empty;
						string foreignpos = string.Empty;
						if (crew is UnidentifiedCrew)
							primary = new TimeOnlyCategory ();
						else
							if (crew.Categories.Any (c => c is TimeOnlyCategory)) 
							{ 
								primary = crew.Categories.First (c => c is TimeOnlyCategory);
							} 
							else 
							{
								primary = crew.Categories.First (c => c is EventCategory);
								if (crew.FinishType == FinishType.Finished) 
								{
									overallpos = CategoryNotes(crew, c => c is OverallCategory, false, extras);
									categorypos = primary.Offered ? CategoryNotes (crew, c => c == primary, false, extras) : string.Empty; 
									genderpos = CategoryNotes(crew, c => c.EventType == EventType.MastersHandicapped, false, extras); 
									foreignpos =  CategoryNotes(crew, c => c.EventType == EventType.Foreign, true, extras); 
								}
							}

						if (!string.IsNullOrEmpty (crew.QueryReason))
							extras.Append (crew.QueryReason);

						if (!string.IsNullOrEmpty (crew.Citation))
							extras.Append (crew.Citation);

						string sequenceStart = string.Empty;
						string sequenceFinish = string.Empty;
						if (crew.FinishType == FinishType.Finished) {
							sequenceStart = crews.Count (c => c.StartTime <= crew.StartTime && c.StartTime > DateTime.MinValue).ToString();
							sequenceFinish = crews.Count (c => c.FinishTime <= crew.FinishTime && c.FinishTime > DateTime.MinValue).ToString();
						}
						string elapsed = (crew.FinishType == FinishType.Finished || crew.FinishType == FinishType.TimeOnly) ? crew.Elapsed.ToString ().Substring (3).Substring (0, 8) : crew.FinishType.ToString ();
						string adjustment = crew.FinishType == FinishType.Finished ? crew.Adjusted.ToString ().Substring (3).Substring (0, 8) : string.Empty;
						string adjusted = (crew.FinishType == FinishType.Finished && crew.Adjustment > TimeSpan.Zero) ? crew.Adjustment.ToString ().Substring (3) : string.Empty;
						var objects = new List<Tuple<string, Font>> { 
							new Tuple<string, Font> (overallpos, font),
							new Tuple<string, Font> (crew.StartNumber.ToString (), font),
							new Tuple<string, Font> (crew.Name, font),
							new Tuple<string, Font> (elapsed, font),
							new Tuple<string, Font> (adjustment, italic),
							new Tuple<string, Font> (adjusted, italic),
							new Tuple<string, Font> (primary.Name, primary.Offered ? font : italic),
							new Tuple<string, Font> (categorypos, font ),
							new Tuple<string, Font> (genderpos, font ),
							new Tuple<string, Font> (foreignpos, font ),
							new Tuple<string, Font> (extras.ToString(), italic ),
						};
						sb.AppendLine (new List<string>{ overallpos, crew.StartNumber.ToString(), crew.Name, sequenceStart, sequenceFinish, elapsed, adjustment, adjusted, primary.Name, categorypos, crew.FinishType.ToString() }.Delimited ('\t'));

						// TODO - actual category, for the purposes of adjustment 
						// chris - if multiple crews from the same club in the same category put the stroke's name - currently being overridden after manual observation 
						foreach (var l in objects)
							table.AddCell (new PdfPCell (new Phrase (l.Item1.TrimEnd (), l.Item2)) { Border = 0 }); 
					}
					using (System.IO.StreamWriter file = new System.IO.StreamWriter(ConfigurationManager.AppSettings["racecode"].ToString()+"-results.txt"))
					{
						file.Write(sb.ToString());
					}

					document.Add(table);
					document.Add (new Paragraph ("Categories shown in italics have not attracted sufficient entries to qualify for prizes.", italic));
					document.Add (new Paragraph ("The adjusted and foreign prizes are open to all indicated crews and will be awarded based on adjusted times as calculated according to the tables in the Rules of Racing", font));
					document.Add (new Paragraph (updated, font));

					document.AddTitle("Designed by www.vestarowing.co.uk");
					document.AddAuthor(string.Format("Chris Harrison, {0} Timing and Results", ConfigurationManager.AppSettings["racenamelong"]));
					document.AddKeywords(string.Format("{0}, {1}, Draw", ConfigurationManager.AppSettings["racenamelong"], racedate.Year));

					document.Close();
				}
			}
		}

		static string CategoryNotes(ICrew crew, Func<ICategory, bool> predicate, bool useDefault, StringBuilder stringBuilder)
		{
			ICategory cat = useDefault ? crew.Categories.FirstOrDefault (c => predicate (c)) : crew.Categories.First (c => predicate (c));
			if(cat == null) 
				return string.Empty;

			int position = crew.CategoryPosition (cat);
			if (position == 1)
				stringBuilder.AppendFormat ("{0} winner. ", cat.Name);
			return position.ToString ();
		}

	}
	
}
